using Nancy;
using Nancy.ModelBinding;

namespace ZorkSms.Web.Modules
{
    public class ApiModule : NancyModule
    {
        public ApiModule() : base("/api")
        {
            Get["/ping"] = o => HttpStatusCode.OK;
            Post["/ReceiveSms"] = o =>
            {
                this.Bind<SmsMessage>();
                return HttpStatusCode.OK
            } 
        }
    }
}