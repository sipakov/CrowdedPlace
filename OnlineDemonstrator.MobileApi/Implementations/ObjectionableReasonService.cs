using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using OnlineDemonstrator.EfCli;
using OnlineDemonstrator.Libraries.Domain.Entities;
using OnlineDemonstrator.Libraries.Domain.Models;
using OnlineDemonstrator.MobileApi.CustomExceptionMiddleware;
using OnlineDemonstrator.MobileApi.Interfaces;

namespace OnlineDemonstrator.MobileApi.Implementations
{
    public class ObjectionableReasonService : IObjectionableReasonService
    {
        private readonly IContextFactory<ApplicationContext> _contextFactory;
        private readonly IStringLocalizer<AppResources> _stringLocalizer;

        public ObjectionableReasonService(IContextFactory<ApplicationContext> contextFactory, IStringLocalizer<AppResources> stringLocalizer)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _stringLocalizer = stringLocalizer;
        }

        public async Task<BaseResult> AddAsync(ObjectionableContent objectionableContent)
        {
            await using var context = _contextFactory.CreateContext();

            var targetObjectionableContent = await context.ObjectionableContents.AsNoTracking().FirstOrDefaultAsync(x =>
                x.DeviceId == objectionableContent.DeviceId &&
                x.ObjectionableDeviceId == objectionableContent.ObjectionableDeviceId &&
                x.ObjectionablePosterCreatedDate == objectionableContent.ObjectionablePosterCreatedDate);

            if (targetObjectionableContent != null)
            {
                throw new ValidationException(_stringLocalizer["ReportIsAlreadyAComplaint"]);
            }

            await context.ObjectionableContents.AddAsync(objectionableContent);
            await context.SaveChangesAsync();
            return new BaseResult();
        }
    }
}