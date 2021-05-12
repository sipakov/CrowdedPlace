using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Amver.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Npgsql;
using OnlineDemonstrator.EfCli;
using OnlineDemonstrator.Libraries.Domain.Dto;
using OnlineDemonstrator.Libraries.Domain.Entities;
using OnlineDemonstrator.Libraries.Domain.Enums;
using OnlineDemonstrator.MobileApi.CustomExceptionMiddleware;
using OnlineDemonstrator.MobileApi.Interfaces;
using IsolationLevel = System.Data.IsolationLevel;

namespace OnlineDemonstrator.MobileApi.Implementations
{
    public class PosterService : IPosterService
    {
        private readonly IContextFactory<ApplicationContext> _contextFactory;
        private readonly IDemonstrationService _demonstrationService;
        private readonly IDistanceCalculator _distanceCalculator;
        private const int DemonstrationDistanceInKilometers = 1;
        private readonly IStringLocalizer<AppResources> _stringLocalizer;
        private readonly IReverseGeoCodingPlaceGetter _reverseGeoCodingPlaceGetter;
        private readonly IConfiguration _config;

        public PosterService(IContextFactory<ApplicationContext> contextFactory,
            IDemonstrationService demonstrationService, IDistanceCalculator distanceCalculator, IStringLocalizer<AppResources> stringLocalizer, IReverseGeoCodingPlaceGetter reverseGeoCodingPlaceGetter, IConfiguration config)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _demonstrationService =
                demonstrationService ?? throw new ArgumentNullException(nameof(demonstrationService));
            _distanceCalculator = distanceCalculator ?? throw new ArgumentNullException(nameof(distanceCalculator));
            _stringLocalizer = stringLocalizer ?? throw new ArgumentNullException(nameof(stringLocalizer));
            _reverseGeoCodingPlaceGetter = reverseGeoCodingPlaceGetter ?? throw new ArgumentNullException(nameof(reverseGeoCodingPlaceGetter));
            _config = config ?? throw new ArgumentNullException(nameof(config));
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

                var targetCountry = newDemonstration.AreaName;
                var areaArray = newDemonstration.AreaName.Split(",").ToList();
                if (areaArray.Any())
                {
                    targetCountry = areaArray.Last().Trim();
                }
                var targetTitle = isNewDemonstration ? $"New demonstration in {targetCountry}" : "New poster";
                var targetMessage = isNewDemonstration ? posterEntity.Entity.Title : "";
                Task.Run(async ()=>await SendNotifications(targetTitle, targetMessage));
                
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

        private async Task SendNotifications(string title, string message)
        {
            await using var context = _contextFactory.CreateContext();
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var fcmToken = _config.GetSection("KeyApiGoogleNotifications").Value;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", fcmToken);
            
            var targetFcmTokens = context.Devices.Where(x=>!x.IsNotSendNotifications && !string.IsNullOrEmpty(x.FcmToken)).Select(x=>x.FcmToken).ToList();
            
            var push = new Push
            {
                registration_ids = targetFcmTokens,
                notification = new Notification
                {
                    title =title,
                    body = message,
                    content_available = true,
                    priority = "high",
                    //badge = 1,
                    sound = "default",
                    //icon = "ic_launcher_notification"
                },
                data = new Data()
            };
            var content = JsonConvert.SerializeObject(push);
            HttpContent httpContent = new StringContent(content, Encoding.UTF8, "application/json");
            const string url = "https://fcm.googleapis.com/fcm/send";
            _ = await httpClient.PostAsync(url, httpContent);  
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

            try
            {
                var posterEntity = await context.Posters.AddAsync(newPoster);
                await context.SaveChangesAsync();

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

            try
            {
                var posterEntity = await context.Posters.AddAsync(newPoster);
                await context.SaveChangesAsync();

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
    }
}