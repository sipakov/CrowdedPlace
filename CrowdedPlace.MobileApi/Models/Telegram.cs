using System.Collections.Generic;

namespace CrowdedPlace.MobileApi.Models
{
    public class Telegram
    {
        public string TelegramChatName { get; set; }
        
        public TelegramCredentials TelegramCredentials { get; set; }
    }
}