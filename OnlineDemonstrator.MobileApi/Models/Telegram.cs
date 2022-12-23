using System.Collections.Generic;

namespace OnlineDemonstrator.MobileApi.Models
{
    public class Telegram
    {
        public string TelegramChatName { get; set; }
        
        public TelegramCredentials TelegramCredentials { get; set; }
    }
}