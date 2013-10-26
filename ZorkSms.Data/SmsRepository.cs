using MongoRepository;
using ZorkSms.Data.Models;

namespace ZorkSms.Data
{
    public class SmsRepository : MongoRepository<SmsMessage>
    {
    }
}
