using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CrowdedPlace.Libraries.Domain.Dto;
using CrowdedPlace.Libraries.Domain.Entities;
using CrowdedPlace.Libraries.Domain.Models;

namespace CrowdedPlace.MobileApi.Interfaces
{
    public interface IDeviceService
    {
        Task<BaseResult> AddAsync(DeviceIn deviceIn);

        Task<Device> GetAsync([FromBody, Required] DeviceIn deviceIn);
        
        Task<BaseResult> ShareAsync([FromQuery, Required] string deviceIn);
    }
}