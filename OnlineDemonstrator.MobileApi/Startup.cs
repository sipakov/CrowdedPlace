using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using OnlineDemonstrator.EfCli;
using OnlineDemonstrator.Libraries.Domain.Models;
using OnlineDemonstrator.MobileApi.CustomExceptionMiddleware.Extensions;
using OnlineDemonstrator.MobileApi.Implementations;
using OnlineDemonstrator.MobileApi.Interfaces;

namespace OnlineDemonstrator.MobileApi
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.TryAddTransient<IContextFactory<ApplicationContext>, ApplicationContextFactory>();
            services.TryAddTransient<IPosterService, PosterService>();
            services.TryAddTransient<IDemonstrationService, DemonstrationService>();
            services.TryAddTransient<IDistanceCalculator, DistanceCalculator>();
            services.TryAddTransient<IDeviceService, DeviceService>();
            services.TryAddTransient<IObjectionableReasonService, ObjectionableReasonService>();
            services.TryAddTransient<IReverseGeoCodingPlaceGetter, ReverseGeoCodingPlaceGetter>();
            
            var supportedCultures = new[]
            {
                new CultureInfo("ru"),
                new CultureInfo("en")
            };

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("en");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                options.RequestCultureProviders = new List<IRequestCultureProvider>
                {
                    new AcceptLanguageHeaderRequestCultureProvider()
                };
            });

            services.AddLocalization(options => options.ResourcesPath = "Localization");
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.KnownProxies.Add(IPAddress.Parse("84.201.184.247"));
            });

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRequestLocalization();
            app.UseCustomExceptionMiddleware();
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            app.UseStaticFiles();
            app.UseRouting();
            app.UseRequestLocalization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}