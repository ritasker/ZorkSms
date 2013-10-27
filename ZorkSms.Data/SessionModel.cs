using MongoRepository;
using System.Collections.Generic;

namespace ZorkSms.Data
{
    public class SessionModel : Entity
    {
        public string PhoneNumber { get; set; }
        public byte[] Data { get; set; }
        public List<string> Commands { get; set; }
    }
}
