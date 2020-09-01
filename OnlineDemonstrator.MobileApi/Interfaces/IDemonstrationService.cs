using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using OnlineDemonstrator.EfCli;
using OnlineDemonstrator.Libraries.Domain.Dto;
using OnlineDemonstrator.Libraries.Domain.Entities;
using OnlineDemonstrator.Libraries.Domain.Models;

namespace OnlineDemonstrator.MobileApi.Interfaces
{
    public interface IDemonstrationService
    {
        Task<IEnumerable<DemonstrationOut>> GetActualDemonstrations(ApplicationContext context = null);

        Task<Demonstration> AddAsync(ApplicationContext context, double latitude, double longitude, DateTime currentDateTime, string countryName, string cityName, string areaName);

        Task<DemonstrationOut> GetNearestDemonstration(PointsIn pointsIn);
        
        Task<DemonstrationCountOut> GetDemonstrationCount();
    }
}