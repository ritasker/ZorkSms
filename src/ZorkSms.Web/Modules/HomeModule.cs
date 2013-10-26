using Nancy;

namespace ZorkSms.Web.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get["/"] = _ => { return View["Index"]; };
        }
    }
}