using MongoRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZorkSms.Data
{
    public class SessionRepository : MongoRepository<SessionModel>
    {
        public SessionModel FindByPhoneNumber(string phoneNumber)
        {
            return this.SingleOrDefault(x => x.PhoneNumber == phoneNumber);
        }
    }
}
