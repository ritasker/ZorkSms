using MongoRepository;
using System.Collections.Generic;

namespace ZorkSms.Data
{
    public class SessionModel : Entity
    {
        public string PhoneNumber { get; set; }
        public List<string> Commands { get; set; }
    }
}
