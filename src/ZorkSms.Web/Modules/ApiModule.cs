using System.Collections.Generic;
using System.Linq;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Responses;
using ZorkSms.Data;
using ZorkSms.Data.Models;

namespace ZorkSms.Web.Modules
{
    public class ApiModule : NancyModule
    {
        private readonly SmsRepository _smsRepository;

        public ApiModule(SmsRepository smsRepository) : base("/api")
        {
            _smsRepository = smsRepository;
            Get["/ping"] = o => HttpStatusCode.OK;
            Post["/ReceiveSms"] = o =>
            {
                var smsMessage = this.Bind<SmsMessage>();

                _smsRepository.Add(smsMessage);

                return HttpStatusCode.OK;
            };

            Get["/Messages"] = o =>
            {
                IList<SmsMessage> smsMessages = _smsRepository.Collection.FindAll().ToList();
                return new JsonResponse(smsMessages, new DefaultJsonSerializer());
            };
        }
    }
}