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
        
        [HttpGet("share")]
        public async Task<ActionResult<BaseResult>> ShareAsync([FromQuery, Required] string deviceIn)
        {
            if (!ModelState.IsValid) return BadRequest();

            return await _deviceService.ShareAsync(deviceIn);
        }
        
        [HttpGet("getMetaDataApp")]
        public ActionResult<MetaDataOut> GetActualLinkToTheApp()
        {
            var metaData = new MetaDataOut
            {
                LinkToAppStore = "https://apps.apple.com/ru/app/online-demonstrator/id1511424258"
            };
           
            return metaData;
        }
    }
}