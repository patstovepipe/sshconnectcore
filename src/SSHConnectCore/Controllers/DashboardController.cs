using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SSHConnectCore.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SSHConnectCore.Controllers
{
    public class DashboardController : Controller
    {
        private readonly AppSettings appSettings;

        private readonly HttpClient client = new HttpClient();

        private static string apiControllerName = "SSHConnect";

        private string CurrentURL => Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(Request);

        private string APIURL => CurrentURL.Replace(ControllerContext.RouteData.Values["controller"].ToString(), apiControllerName);

        public DashboardController(IOptions<AppSettings> settings)
        {
            this.appSettings = settings.Value;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult KillProcess(string id)
        {
            using (var client = new HttpClient())
            {
                var model = client
                            .GetAsync(APIURL)
                            .Result
                            .Content.ReadAsStringAsync().Result;
            }

            return RedirectToAction("Index");
        }
    }
}
