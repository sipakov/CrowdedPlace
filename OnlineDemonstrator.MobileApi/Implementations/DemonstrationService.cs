using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnlineDemonstrator.EfCli;
using OnlineDemonstrator.Libraries.Domain.Dto;
using OnlineDemonstrator.Libraries.Domain.Entities;
using OnlineDemonstrator.MobileApi.Interfaces;

namespace OnlineDemonstrator.MobileApi.Implementations
{
    public class DemonstrationService : IDemonstrationService
    {
        
        private readonly IContextFactory<ApplicationContext> _contextFactory;
        private readonly IDistanceCalculator _distanceCalculator;

        public DemonstrationService(IContextFactory<ApplicationContext> contextFactory, IDistanceCalculator distanceCalculator)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _distanceCalculator = distanceCalculator ?? throw new ArgumentNullException(nameof(distanceCalculator));
        }

        public async Task<IEnumerable<DemonstrationOut>> GetActualDemonstrations(ApplicationContext context = null)
        {
            context ??= _contextFactory.CreateContext();
            const int expDay = 7;
            var currentDate = DateTime.UtcNow.Date;
            var actualDate = currentDate.AddDays(-expDay);

            var actualDemonstrations = await context.Demonstrations.AsNoTracking().Where(x=>!x.IsDeleted).OrderByDescending(x=>x.DemonstrationDate).Select(x=> new DemonstrationOut()
            {
                Id = x.Id,
                DemonstrationDate = x.DemonstrationDate,
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                CountryName = x.CountryName,
                DetailName = $"{x.CityName}, {x.AreaName}",
                IsExpired = (actualDate - x.DemonstrationDate).Days > expDay
            }).ToListAsync();

            return actualDemonstrations;
        }

        public async Task<Demonstration> AddAsync(ApplicationContext context, double latitude, double longitude, DateTime currentDateTime, string countryName, string cityName, string areaName)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var currentDate = currentDateTime.Date;
            
            var newDemonstration = new Demonstration
            {
                DemonstrationDate = currentDate,
                Latitude = latitude,
                Longitude = longitude,
                CountryName = countryName,
                CityName = string.IsNullOrEmpty(cityName) ? latitude.ToString() : cityName,
                AreaName = string.IsNullOrEmpty(areaName) ? longitude.ToString() : areaName
            };

            var demonstrationEntity = await context.Demonstrations.AddAsync(newDemonstration);
            await context.SaveChangesAsync();
            return demonstrationEntity.Entity;
        }
        
                
        public async Task<DemonstrationOut> GetNearestDemonstration(PointsIn pointsIn)
        {
            if (pointsIn == null) throw new ArgumentNullException(nameof(pointsIn));
            
            await using var context = _contextFactory.CreateContext();
            
            const int expDay = 7;
            var currentDate = DateTime.UtcNow.Date;
            var actualDate = currentDate.AddDays(-expDay);

            var actualDemonstrations = await context.Demonstrations.AsNoTracking()
                .Where(x => x.DemonstrationDate >= actualDate && !x.IsDeleted).ToListAsync();
             
            var targetDemonstration = new DemonstrationOut();
            var sortedList = new SortedList<double, Demonstration>();
            foreach (var actualDemonstration in actualDemonstrations)
            {
                var distance = _distanceCalculator.GetDistanceInKilometers(pointsIn.Latitude, pointsIn.Longitude,
                    actualDemonstration.Latitude, actualDemonstration.Longitude);
                sortedList.Add(distance, actualDemonstration);
            }

            if (!sortedList.Any()) return targetDemonstration;
            
            var nearestDemonstration = sortedList.First();
            if (nearestDemonstration.Key > pointsIn.RadiusForLookingDemo) return targetDemonstration;

            var postersCount = context.Posters
                .AsNoTracking().Count(x => x.DemonstrationId == nearestDemonstration.Value.Id && !x.IsDeleted);

            targetDemonstration.Id = nearestDemonstration.Value.Id;
            targetDemonstration.Latitude = nearestDemonstration.Value.Latitude;
            targetDemonstration.Longitude = nearestDemonstration.Value.Longitude;
            targetDemonstration.CountryName = nearestDemonstration.Value.CountryName;
            targetDemonstration.DetailName = nearestDemonstration.Value.CityName;
            targetDemonstration.PostersCount = postersCount;
            
            return targetDemonstration;
        }
    }
}