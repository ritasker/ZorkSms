using System.Linq;
using MongoRepository;
using ZorkSms.Data.Models;

namespace ZorkSms.Data
{
    public class SmsRepository : MongoRepository<SmsMessage>
    {
        public SmsMessage FindByMessageId(string messageId)
        {
            return this.FirstOrDefault(x => x.Msg_ID == messageId);
        }
    }
}
