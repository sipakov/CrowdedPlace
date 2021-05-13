using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Amver.Domain.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OnlineDemonstrator.EfCli;
using OnlineDemonstrator.MobileApi.Interfaces;

namespace OnlineDemonstrator.MobileApi.Implementations
{
    public class PushNotifier : IPushNotifier
    {
        private readonly IContextFactory<ApplicationContext> _contextFactory;
        private readonly IConfiguration _config;

        public PushNotifier(IContextFactory<ApplicationContext> contextFactory, IConfiguration config)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public async Task SendPushNotifications(string title, string message, List<string> targetFcmTokens)
        {
            await using var context = _contextFactory.CreateContext();
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var fcmToken = _config.GetSection("KeyApiGoogleNotifications").Value;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", fcmToken);
            
            var push = new Push
            {
                registration_ids = targetFcmTokens,
                notification = new Notification
                {
                    title = title,
                    body = message,
                    content_available = true,
                    priority = "high",
                    //badge = 1,
                    sound = "default",
                    //icon = "ic_launcher_notification"
                },
                data = new Data()
            };
            var content = JsonConvert.SerializeObject(push);
            HttpContent httpContent = new StringContent(content, Encoding.UTF8, "application/json");
            const string url = "https://fcm.googleapis.com/fcm/send";
            _ = await httpClient.PostAsync(url, httpContent);
        }
    }
}