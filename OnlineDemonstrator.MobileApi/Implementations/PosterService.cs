using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Npgsql;
using OnlineDemonstrator.EfCli;
using OnlineDemonstrator.Libraries.Domain.Dto;
using OnlineDemonstrator.Libraries.Domain.Entities;
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
        private const int DemonstrationDistanceInKilometers = 3;
        private readonly IStringLocalizer<AppResources> _stringLocalizer;

        public PosterService(IContextFactory<ApplicationContext> contextFactory,
            IDemonstrationService demonstrationService, IDistanceCalculator distanceCalculator, IStringLocalizer<AppResources> stringLocalizer)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _demonstrationService =
                demonstrationService ?? throw new ArgumentNullException(nameof(demonstrationService));
            _distanceCalculator = distanceCalculator ?? throw new ArgumentNullException(nameof(distanceCalculator));
            _stringLocalizer = stringLocalizer ?? throw new ArgumentNullException(nameof(stringLocalizer));
        }

        public async Task<Poster> AddPosterAsync(PosterIn posterIn, DateTime currentDateTime)
        {
            if (posterIn == null) throw new ArgumentNullException(nameof(posterIn));

            await using var context = _contextFactory.CreateContext();

            await using var transaction = await context.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead);

            try
            {
                var actualDemonstrations = await _demonstrationService.GetActualDemonstrations(context);

                Poster newPoster = null;

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

                if (newPoster == null)
                {
                    var newDemonstration =
                        await _demonstrationService.AddAsync(context, posterIn.Latitude, posterIn.Longitude,
                            currentDateTime, posterIn.CountryName, posterIn.CityName, posterIn.AreaName);
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

             var actualDemonstrations = (await _demonstrationService.GetActualDemonstrations(context)).Select(x => x.Id)
                 .ToList();

            //evaluated locally! Transform to raw sql
            var targetPosters = (await context.Posters.AsNoTracking()
                    .Where(x => actualDemonstrations.Contains(x.DemonstrationId)).OrderByDescending(x=>x.CreatedDateTime)
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
                    CreatedDateTime = x.CreatedDateTime
                }).ToList();

            return targetPosters;
        }

        public async Task<List<PosterOut>> GetPostersByDemonstrationId(int demonstrationId)
        {
            await using var context = _contextFactory.CreateContext();

            const int messageContentLength = 50;
            var targetPosters = await context.Posters.AsNoTracking().Where(x => x.DemonstrationId == demonstrationId).OrderByDescending(x=>x.CreatedDateTime).Select(x=> new PosterOut
                {
                    DeviceId = x.DeviceId,
                    CreatedDate = x.CreatedDate,
                    Name = x.Name,
                    Title = x.Title,
                    Message = x.Message.ToCharArray().Length > messageContentLength ? $"{x.Message.Substring(0, messageContentLength)}..." : x.Message
                })
                .ToListAsync();

            return targetPosters;
        }

        public async Task<PosterOut> GetPosterById(Guid deviceId, DateTime createdDate)
        {
            await using var context = _contextFactory.CreateContext();

            var targetPoster = await context.Posters.FindAsync(deviceId, createdDate);

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
    }
}