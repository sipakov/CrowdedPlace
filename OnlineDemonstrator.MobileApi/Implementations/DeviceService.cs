using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineDemonstrator.EfCli;
using OnlineDemonstrator.Libraries.Domain.Dto;
using OnlineDemonstrator.Libraries.Domain.Entities;
using OnlineDemonstrator.Libraries.Domain.Enums;
using OnlineDemonstrator.Libraries.Domain.Models;
using OnlineDemonstrator.MobileApi.Interfaces;

namespace OnlineDemonstrator.MobileApi.Implementations
{
    public class DeviceService : IDeviceService
    {
        private readonly IContextFactory<ApplicationContext> _contextFactory;

        public DeviceService(IContextFactory<ApplicationContext> contextFactory)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
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
    }
}