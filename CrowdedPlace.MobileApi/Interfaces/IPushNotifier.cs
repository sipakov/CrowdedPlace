using System.Collections.Generic;
using System.Threading.Tasks;
using Amver.Domain.Models;

namespace CrowdedPlace.MobileApi.Interfaces
{
    public interface IPushNotifier
    { 
        Task SendPushNotifications(string title, string message, List<string> targetFcmTokens);

        Task SendPushNotifications(Push pushNotification);
    }
}