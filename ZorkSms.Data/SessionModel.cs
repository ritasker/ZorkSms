using MongoRepository;

namespace ZorkSms.Data
{
    public class SessionModel : Entity
    {
        public string PhoneNumber { get; set; }
        public byte[] Data { get; set; }
    }
}
