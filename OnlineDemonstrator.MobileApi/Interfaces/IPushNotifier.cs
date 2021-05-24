using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineDemonstrator.MobileApi.Interfaces
{
    public interface IPushNotifier
    { 
        Task SendPushNotifications(string title, string message, List<string> targetFcmTokens);
    }
}