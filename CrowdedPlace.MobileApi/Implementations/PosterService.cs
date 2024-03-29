using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amver.Domain.Models;
using CrowdedPlace.EfCli;
using CrowdedPlace.MobileApi.CustomExceptionMiddleware;
using CrowdedPlace.MobileApi.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Npgsql;
using CrowdedPlace.Libraries.Domain.Dto;
using CrowdedPlace.Libraries.Domain.Entities;
using CrowdedPlace.Libraries.Domain.Enums;
using IsolationLevel = System.Data.IsolationLevel;

namespace CrowdedPlace.MobileApi.Implementations
{
    public class PosterService : IPosterService
    {
        private readonly IContextFactory<ApplicationContext> _contextFactory;
        private readonly IDemonstrationService _demonstrationService;
        private readonly IDistanceCalculator _distanceCalculator;
        private const int DemonstrationDistanceInKilometers = 1;
        private readonly IStringLocalizer<AppResources> _stringLocalizer;
        private readonly IReverseGeoCodingPlaceGetter _reverseGeoCodingPlaceGetter;
        private readonly IPushNotifier _pushNotifier;
        private readonly ILogger<PosterService> _logger;

        public PosterService(IContextFactory<ApplicationContext> contextFactory,
            IDemonstrationService demonstrationService, IDistanceCalculator distanceCalculator, IStringLocalizer<AppResources> stringLocalizer, IReverseGeoCodingPlaceGetter reverseGeoCodingPlaceGetter, IPushNotifier pushNotifier, ILogger<PosterService> logger)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _demonstrationService =
                demonstrationService ?? throw new ArgumentNullException(nameof(demonstrationService));
            _distanceCalculator = distanceCalculator ?? throw new ArgumentNullException(nameof(distanceCalculator));
            _stringLocalizer = stringLocalizer ?? throw new ArgumentNullException(nameof(stringLocalizer));
            _reverseGeoCodingPlaceGetter = reverseGeoCodingPlaceGetter ?? throw new ArgumentNullException(nameof(reverseGeoCodingPlaceGetter));
            _pushNotifier = pushNotifier ?? throw new ArgumentNullException(nameof(pushNotifier));
            _logger = logger;
        }

        public async Task<Poster> AddPosterAsync(PosterIn posterIn, DateTime currentDateTime)
        {
            if (posterIn == null) throw new ArgumentNullException(nameof(posterIn));

            await using var context = _contextFactory.CreateContext();

            await using var transaction = await context.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead);

            try
            {
                var targetDevice = await context.Devices.FirstOrDefaultAsync(x => x.Id == posterIn.DeviceId);

                if (targetDevice == null)
                {
                    var isValidBaseDevice = Enum.TryParse(posterIn.BaseOs, out OperationSystems baseOs);
                    var newDevice = new Device
                    {
                        Id = posterIn.DeviceId,
                        CreatedDate = DateTime.UtcNow,
                        OsId = isValidBaseDevice ? (int)baseOs : (int)OperationSystems.Unknown,
                        IsLicenseActivated = true
                    };
                    await context.Devices.AddAsync(newDevice);
                }
                var actualDemonstrations = await _demonstrationService.GetActualDemonstrations(context);
    
                Poster newPoster = null;
                var isNewDemonstration = false;
                foreach (var actualDemonstration in actualDemonstrations)
                {
                    var distance = _distanceCalculator.GetDistanceInKilometers(posterIn.Latitude, posterIn.Longitude,
                        actualDemonstration.Latitude, actualDemonstration.Longitude);
                    if (!(distance < DemonstrationDistanceInKilometers)) continue;
                    newPoster = new Poster
                    {
                        Name = posterIn.Name,
                        Title = posterIn.Title,
                        Message = posterIn.Message,
                        Latitude = posterIn.Latitude,
                        Longitude = posterIn.Longitude,
                        DeviceId = posterIn.DeviceId,
                        DemonstrationId = actualDemonstration.Id,
                        CreatedDate = actualDemonstration.DemonstrationDate,
                        CreatedDateTime = currentDateTime
                    };
                    break;
                }

                Demonstration newDemonstration = new Demonstration();
                if (newPoster == null)
                {
                    isNewDemonstration = true;
                    var formattedAddress =
                        await _reverseGeoCodingPlaceGetter.GetAddressByGeoPosition(posterIn.Latitude, posterIn.Longitude, posterIn.Locale);
                    
                    newDemonstration =
                        await _demonstrationService.AddAsync(context, posterIn.Latitude, posterIn.Longitude,
                            currentDateTime, string.Empty, string.Empty, formattedAddress.FormattedAddress);
                    newPoster = new Poster
                    {
                        Name = posterIn.Name,
                        Title = posterIn.Title,
                        Message = posterIn.Message,
                        Latitude = posterIn.Latitude,
                        Longitude = posterIn.Longitude,
                        DeviceId = posterIn.DeviceId,
                        DemonstrationId = newDemonstration.Id,
                        CreatedDate = newDemonstration.DemonstrationDate,
                        CreatedDateTime = currentDateTime
                    };
                }

                var device = await context.Devices.AsNoTracking().FirstOrDefaultAsync(x => x.Id == posterIn.DeviceId);
                if (device == null) throw new ArgumentNullException(nameof(device));

                var activePosterCount = await context.Posters.Where(x =>
                    x.DeviceId == posterIn.DeviceId && x.CreatedDate > DateTime.UtcNow.Date.AddDays(-7)).CountAsync();
                
                if (activePosterCount >= 1)
                {
                    throw new ValidationException(_stringLocalizer["PosterConstraint"]);
                }

                var posterEntity = await context.Posters.AddAsync(newPoster);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                _logger.LogInformation($"New demonstration: {device.Id} added poster to new demonstration");
                var targetCountry = string.Empty;
                List<string> areaArray;
                if (newDemonstration.AreaName == null)
                {
                    areaArray = (await context.Demonstrations.FirstAsync(
                        x => x.Id == posterEntity.Entity.DemonstrationId)).AreaName.Split(",").ToList();
                }
                else
                {
                    areaArray = newDemonstration.AreaName.Split(",").ToList();  
                }
                
                if (areaArray.Any())
                {
                    targetCountry = areaArray.Last().Trim();
                    var isNumeric = int.TryParse(targetCountry, out var targetCountryStr);
                    if (isNumeric && areaArray.Count > 1)
                    {
                        targetCountry = areaArray[^2].Trim();
                    }
                }

                if (isNewDemonstration)
                {
                    var fcmTokensToLocale = await context.Devices
                        .Where(x => !x.IsNotSendNotifications && !string.IsNullOrEmpty(x.FcmToken) && x.Id != posterIn.DeviceId).ToDictionaryAsync(x => x.FcmToken, y => y.Locale);
                
                    var body = posterEntity.Entity.Title;
                    var localizedPushes = GenerateLocalizedPushes(fcmTokensToLocale, "NewDemonstrationPush", body, targetCountry);
                    foreach (var localizedPush in localizedPushes)
                    {
                        Task.Run(async () => await _pushNotifier.SendPushNotifications(localizedPush));  
                    }
                }
                else
                {
                    var fcmTokensToLocale = await context.Posters.Include(x => x.Device)
                        .Where(x => x.DemonstrationId == newPoster.DemonstrationId && x.DeviceId != posterIn.DeviceId && x.Device.FcmToken != null).ToDictionaryAsync(x => x.Device.FcmToken, y => y.Device.Locale);
                    var body = posterEntity.Entity.Title;
                    var localizedPushes = GenerateLocalizedPushes(fcmTokensToLocale, "NewPosterPush", body, targetCountry);
                    foreach (var localizedPush in localizedPushes)
                    {
                        Task.Run(async () => await _pushNotifier.SendPushNotifications(localizedPush));  
                    }
                }
                return posterEntity.Entity; 
            }
            catch (Exception ex)
            {
                if (ex.InnerException is PostgresException postgresException && postgresException.SqlState == 23505.ToString())
                {
                    await transaction.RollbackAsync();
                    throw new ValidationException(_stringLocalizer["PosterConstraint"]);
                }

                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<PosterOut>> GetFromActualDemonstrations(int postersCountInDemonstration)
        {
            await using var context = _contextFactory.CreateContext();

            var actualDemonstrations = (await _demonstrationService.GetActualDemonstrations(context)).ToDictionary(x=> x.Id, y=>y.IsExpired);
                 
            var actualDemonstrationIds =   actualDemonstrations.Select(x => x.Key).ToList();

            //evaluated locally! Transform to raw sql
            var targetPosters = (await context.Posters.AsNoTracking()
                    .Where(x => actualDemonstrationIds.Contains(x.DemonstrationId) && !x.IsDeleted).OrderByDescending(x=>x.CreatedDateTime)
                    .ToListAsync())
                .GroupBy(x => x.DemonstrationId)
                .Select(x => x.OrderByDescending(y => y.CreatedDate).Take(postersCountInDemonstration))
                .SelectMany(x => x).Select(x=> new PosterOut
                {
                    DeviceId = x.DeviceId,
                    DemonstrationId = x.DemonstrationId,
                    Name = x.Name,
                    Title = x.Title,
                    Latitude = x.Latitude,
                    Longitude = x.Longitude,
                    CreatedDate = x.CreatedDate,
                    CreatedDateTime = x.CreatedDateTime,
                    IsExpired = actualDemonstrations[x.DemonstrationId]
                }).ToList();

            return targetPosters;
        }

        public async Task<List<PosterOut>> GetPostersByDemonstrationId(int demonstrationId)
        {
            await using var context = _contextFactory.CreateContext();

            const int messageContentLength = 50;
            var targetPosters = await context.Posters.AsNoTracking().Where(x => x.DemonstrationId == demonstrationId && !x.IsDeleted).OrderByDescending(x=>x.CreatedDateTime).Select(x=> new PosterOut
                {
                    DeviceId = x.DeviceId,
                    CreatedDate = x.CreatedDate,
                    Name = x.Name,
                    Title = x.Title,
                    Message = x.Message.ToCharArray().Length > messageContentLength ? $"{x.Message.Substring(0, messageContentLength)}..." : x.Message,
                    DemonstrationId = x.DemonstrationId,
                    CreatedDateTime = x.CreatedDateTime
                })
                .ToListAsync();

            return targetPosters;
        }

        public async Task<PosterOut> GetPosterById(string deviceId, DateTime createdDate, int demonstrationId)
        {
            await using var context = _contextFactory.CreateContext();

            var targetPoster = await context.Posters.FindAsync(deviceId, createdDate, demonstrationId);

            var posterOut = new PosterOut
            {
                DeviceId = targetPoster.DeviceId,
                CreatedDate = targetPoster.CreatedDate,
                Name = targetPoster.Name,
                Title = targetPoster.Title,
                Message = targetPoster.Message,
                DemonstrationId = targetPoster.DemonstrationId
            };

            return posterOut;
        }

        public async Task<Poster> AddPosterToExistDemonstrationAsync(PosterIn posterIn, DateTime currentDateTime)
        {
            await using var context = _contextFactory.CreateContext();

            var demonstration = await context.Demonstrations.AsNoTracking().FirstOrDefaultAsync(x => x.Id == posterIn.DemonstrationId);

            if (demonstration == null) return new Poster();

            var targetDevice = await context.Devices.FirstOrDefaultAsync(x => x.Id == posterIn.DeviceId);

            if (targetDevice == null)
            {
                var isValidBaseDevice = Enum.TryParse(posterIn.BaseOs, out OperationSystems baseOs);
                var newDevice = new Device
                {
                    Id = posterIn.DeviceId,
                    CreatedDate = DateTime.UtcNow,
                    OsId = isValidBaseDevice ? (int)baseOs : (int)OperationSystems.Unknown,
                    IsLicenseActivated = true
                };
                await context.Devices.AddAsync(newDevice);
            }
            
            var newPoster = new Poster
            {
                Name = posterIn.Name,
                Title = posterIn.Title,
                Message = posterIn.Message,
                Latitude = posterIn.Latitude == default ? demonstration.Latitude : posterIn.Latitude,
                Longitude = posterIn.Longitude == default ? demonstration.Longitude : posterIn.Longitude,
                DeviceId = posterIn.DeviceId,
                DemonstrationId = posterIn.DemonstrationId,
                CreatedDate = demonstration.DemonstrationDate,
                CreatedDateTime = currentDateTime
            };
            
            var activePosterCount = await context.Posters.Where(x =>
                x.DeviceId == posterIn.DeviceId && x.CreatedDate > DateTime.UtcNow.Date.AddDays(-7)).CountAsync();
                
            if (activePosterCount >= 1)
            {
                throw new ValidationException(_stringLocalizer["PosterConstraint"]);
            }

            try
            {
                var posterEntity = await context.Posters.AddAsync(newPoster);
                await context.SaveChangesAsync();
                
                var targetCountry = demonstration.AreaName;
                var areaArray = demonstration.AreaName.Split(",").ToList();
                if (areaArray.Any())
                {
                    targetCountry = areaArray.Last().Trim();
                    var isNumeric = int.TryParse(targetCountry, out var targetCountryStr);
                    if (isNumeric && areaArray.Count > 1)
                    {
                        targetCountry = areaArray[^2].Trim();
                    }
                }
                
                var fcmTokensToLocale = await context.Posters.Include(x => x.Device)
                    .Where(x => x.DemonstrationId == newPoster.DemonstrationId && x.DeviceId != posterIn.DeviceId && x.Device.FcmToken != null).ToDictionaryAsync(x => x.Device.FcmToken, y => y.Device.Locale);
                
                var body = posterEntity.Entity.Title;
                var localizedPushes = GenerateLocalizedPushes(fcmTokensToLocale, "NewPosterPush", body, targetCountry);
                foreach (var localizedPush in localizedPushes)
                {
                    Task.Run(async () => await _pushNotifier.SendPushNotifications(localizedPush));  
                }
                _logger.LogInformation($"New poster in exist demonstration {posterIn.DemonstrationId}: {posterIn.DeviceId} added poster");
                return posterEntity.Entity;
            }
            catch (Exception ex)
            {
                if (ex.InnerException is PostgresException postgresException &&
                    postgresException.SqlState == 23505.ToString())
                {
                    throw new ValidationException(_stringLocalizer["PosterConstraint"]);
                }
                throw;
            }
        }
        
        public async Task<Poster> AddPosterToExpiredDemonstrationAsync(PosterIn posterIn, DateTime currentDateTime)
        {
            await using var context = _contextFactory.CreateContext();

            var demonstration = await context.Demonstrations.FirstOrDefaultAsync(x => x.Id == posterIn.DemonstrationId);

            if (demonstration == null) return new Poster();

            var targetDevice = await context.Devices.FirstOrDefaultAsync(x => x.Id == posterIn.DeviceId);

            if (targetDevice == null)
            {
                var isValidBaseDevice = Enum.TryParse(posterIn.BaseOs, out OperationSystems baseOs);
                var newDevice = new Device
                {
                    Id = posterIn.DeviceId,
                    CreatedDate = DateTime.UtcNow,
                    OsId = isValidBaseDevice ? (int)baseOs : (int)OperationSystems.Unknown,
                    IsLicenseActivated = true
                };
                await context.Devices.AddAsync(newDevice);
            }

            demonstration.DemonstrationDate = currentDateTime.Date;
            
            var newPoster = new Poster
            {
                Name = posterIn.Name,
                Title = posterIn.Title,
                Message = posterIn.Message,
                Latitude = posterIn.Latitude == default ? demonstration.Latitude : posterIn.Latitude,
                Longitude = posterIn.Longitude == default ? demonstration.Longitude : posterIn.Longitude,
                DeviceId = posterIn.DeviceId,
                DemonstrationId = posterIn.DemonstrationId,
                CreatedDate = currentDateTime.Date,
                CreatedDateTime = currentDateTime
            };
            
            var activePosterCount = await context.Posters.Where(x =>
                x.DeviceId == posterIn.DeviceId && x.CreatedDate > DateTime.UtcNow.Date.AddDays(-7)).CountAsync();
                
            if (activePosterCount >= 1)
            {
                throw new ValidationException(_stringLocalizer["PosterConstraint"]);
            }

            try
            {
                var posterEntity = await context.Posters.AddAsync(newPoster);
                await context.SaveChangesAsync();
                
                var targetCountry = demonstration.AreaName;
                var areaArray = demonstration.AreaName.Split(",").ToList();
                if (areaArray.Any())
                {
                    targetCountry = areaArray.Last().Trim();
                    var isNumeric = int.TryParse(targetCountry, out var targetCountryStr);
                    if (isNumeric && areaArray.Count > 1)
                    {
                        targetCountry = areaArray[^2].Trim();
                    }
                }
                
                var fcmTokensToLocale = await context.Posters.Include(x => x.Device)
                    .Where(x => x.DemonstrationId == newPoster.DemonstrationId && x.DeviceId != posterIn.DeviceId && x.Device.FcmToken != null).ToDictionaryAsync(x => x.Device.FcmToken, y => y.Device.Locale);
                
                var body = posterEntity.Entity.Title;
                var localizedPushes = GenerateLocalizedPushes(fcmTokensToLocale, "NewPosterPush", body, targetCountry);
                foreach (var localizedPush in localizedPushes)
                {
                   Task.Run(async () => await _pushNotifier.SendPushNotifications(localizedPush));  
                }
                _logger.LogInformation($"New poster in expire demonstration {posterIn.DemonstrationId}: {posterIn.DeviceId} added poster");
                return posterEntity.Entity;
            }
            catch (Exception ex)
            {
                if (ex.InnerException is PostgresException postgresException &&
                    postgresException.SqlState == 23505.ToString())
                {
                    throw new ValidationException(_stringLocalizer["PosterConstraint"]);
                }
                throw;
            }
        }

        public IEnumerable<Push> GenerateLocalizedPushes(Dictionary<string, string> fcmTokensToLocale, string localeKey, string body, string country)
        {
            var groupedByLocales = fcmTokensToLocale.GroupBy(x => x.Value);

            foreach (var groupedByLocale in groupedByLocales)
            {
                var targetLocale = groupedByLocale.Key;
                var push = new Push
                {
                    registration_ids = new List<string>(),
                    notification = new Notification
                    {
                        title =$"{Extensions.LocalizationExtension.GetString(_stringLocalizer, targetLocale, "NewPosterPush").Value} ({country})",
                        body = body,
                        content_available = true,
                        priority = "high",
                        //badge = 1,
                        sound = "default",
                        //icon = "ic_launcher_notification"
                    }
                };
                foreach (var keyValuePair in groupedByLocale)
                {
                    push.registration_ids.Add(keyValuePair.Key);
                }

                yield return push;
            }
        }
    }
}