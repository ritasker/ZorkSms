using Nancy;

namespace ZorkSms.Web.Modules
{
    public class ApiModule : NancyModule
    {
        public ApiModule() : base("/api")
        {
            Get["/ping"] = o => HttpStatusCode.OK;
        }
    }
}