using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OnlineDemonstrator.Libraries.Domain.Dto;
using OnlineDemonstrator.Libraries.Domain.Entities;
using OnlineDemonstrator.Libraries.Domain.Models;

namespace OnlineDemonstrator.MobileApi.Interfaces
{
    public interface IDeviceService
    {
        Task<BaseResult> AddAsync(DeviceIn deviceIn);

        Task<Device> GetAsync([FromBody, Required] DeviceIn deviceIn);
        
        Task<BaseResult> ShareAsync([FromQuery, Required] string deviceIn);
    }
}