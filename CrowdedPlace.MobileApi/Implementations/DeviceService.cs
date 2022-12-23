using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CrowdedPlace.EfCli;
using CrowdedPlace.MobileApi.Interfaces;
using CrowdedPlace.Libraries.Domain.Dto;
using CrowdedPlace.Libraries.Domain.Entities;
using CrowdedPlace.Libraries.Domain.Enums;
using CrowdedPlace.Libraries.Domain.Models;

namespace CrowdedPlace.MobileApi.Implementations
{
    public class DeviceService : IDeviceService
    {
        private readonly IContextFactory<ApplicationContext> _contextFactory;
        private readonly ILogger<DeviceService> _logger;

        public DeviceService(IContextFactory<ApplicationContext> contextFactory, ILogger<DeviceService> logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        public async Task<BaseResult> AddAsync([FromBody, Required] DeviceIn deviceIn)
        {
            if (deviceIn == null) throw new ArgumentNullException(nameof(deviceIn));

            await using var context = _contextFactory.CreateContext();
            var isValidBaseDevice = Enum.TryParse(deviceIn.BaseOs, out OperationSystems baseOs);

            var targetDevice = await context.Devices.FirstOrDefaultAsync(x => x.Id == deviceIn.DeviceId);

            if (targetDevice != null)
            {
                targetDevice.FcmToken = deviceIn.FcmToken;
                targetDevice.LastVisitDate = DateTime.UtcNow;
                targetDevice.Locale = deviceIn.Locale;
                await context.SaveChangesAsync();
                _logger.LogInformation($"Login: {deviceIn.DeviceId} with locale {deviceIn.Locale}");
            }
            else
            {
                var device = new Device
                {
                    Id = deviceIn.DeviceId,
                    CreatedDate = DateTime.UtcNow,
                    LastVisitDate = DateTime.UtcNow,
                    FcmToken = deviceIn.FcmToken,
                    OsId = isValidBaseDevice ? (int)baseOs : (int)OperationSystems.Unknown,
                    Locale = deviceIn.Locale
                };
                await context.Devices.AddAsync(device);
                await context.SaveChangesAsync();  
                _logger.LogInformation($"Registration: {deviceIn.DeviceId} with locale {deviceIn.Locale}");
            }
            
            return new BaseResult();
        }

        public async Task<Device> GetAsync([FromBody, Required] DeviceIn deviceIn)
        {
            if (deviceIn == null) throw new ArgumentNullException(nameof(deviceIn));

            await using var context = _contextFactory.CreateContext();

            var targetDevice = await context.Devices.AsNoTracking().FirstOrDefaultAsync(x => x.Id == deviceIn.DeviceId);

            return targetDevice;
        }

        public async Task<BaseResult> ShareAsync(string deviceIn)
        {
            if (string.IsNullOrEmpty(deviceIn)) throw new ArgumentNullException(nameof(deviceIn));  
            
            await using var context = _contextFactory.CreateContext();

            var targetDevice = await context.Devices.FirstOrDefaultAsync(x => x.Id == deviceIn);
            if (targetDevice == null)
            {
                throw new ArgumentNullException(nameof(deviceIn));      
            }

            targetDevice.SharedCount += 1;
            await context.SaveChangesAsync();
            return new BaseResult();
        }
    }
}