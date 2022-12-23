using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CrowdedPlace.EfCli;
using CrowdedPlace.Libraries.Domain.Dto;
using CrowdedPlace.Libraries.Domain.Entities;

namespace CrowdedPlace.MobileApi.Interfaces
{
    public interface IDemonstrationService
    {
        Task<IEnumerable<DemonstrationOut>> GetActualDemonstrations(ApplicationContext context = null);

        Task<Demonstration> AddAsync(ApplicationContext context, double latitude, double longitude, DateTime currentDateTime, string countryName, string cityName, string areaName);

        Task<DemonstrationOut> GetNearestDemonstration(PointsIn pointsIn);
        
        Task<DemonstrationCountOut> GetDemonstrationCount();
    }
}