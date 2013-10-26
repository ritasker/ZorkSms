namespace ZorkSms.Core.Models
{
    public class SmsMessage
    {
        public string To { get; set; } 
        public string From { get; set; } 
        public string Content { get; set; }
        public string Msg_ID { get; set; }
        public string Keyword { get; set; } 
    }
}