using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OnlineDemonstrator.Libraries.Domain.Dto;
using OnlineDemonstrator.Libraries.Domain.Entities;
using OnlineDemonstrator.Libraries.Domain.Models;
using OnlineDemonstrator.MobileApi.Interfaces;

namespace OnlineDemonstrator.MobileApi.Controllers
{
    [Route("[controller]")]
    public class DeviceController : ControllerBase
    {
        private readonly IDeviceService _deviceService;

        public DeviceController(IDeviceService deviceService)
        {
            _deviceService = deviceService ?? throw new ArgumentNullException(nameof(deviceService));
        }

        [HttpPost("add")]
        public async Task<ActionResult<BaseResult>> AddAsync([FromBody, Required] DeviceIn deviceIn)
        {
            if (!ModelState.IsValid) return BadRequest();

            return await _deviceService.AddAsync(deviceIn);
        }
        
        [HttpPost("get")]
        public async Task<ActionResult<Device>> GetAsync([FromBody, Required] DeviceIn deviceIn)
        {
            if (!ModelState.IsValid) return BadRequest();

            return await _deviceService.GetAsync(deviceIn);
        }
    }
}