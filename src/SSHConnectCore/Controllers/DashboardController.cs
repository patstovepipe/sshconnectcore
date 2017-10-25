using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SSHConnectCore.Configuration;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using SSHConnectCore.Models.Dashboard;

namespace SSHConnectCore.Controllers
{
    public class DashboardController : WebController
    {
        public DashboardController(IOptions<AppSettings> settings) : base(settings)
        {
        }

        public IActionResult Index()
        {
            ViewBag.KillProcessSelectList = KillProcessSelectList();
            return View();
        }


        public IActionResult Shutdown()
        {
            return DoAPIAction();
        }

        public IActionResult Restart()
        {
            return DoAPIAction();
        }

        public IActionResult KillProcess(string id)
        {
            return DoAPIAction();
        }

        private List<SelectListItem> KillProcessSelectList()
        {
            var currentURL = CurrentURL.Length <= CurrentURL.IndexOf('/', 7) + 1 ? CurrentURL : CurrentURL.Remove(CurrentURL.IndexOf('/', 7) + 1);
            var killProcessListURL = currentURL + apiControllerName + "/KillProcessList";
            string result = APICall(killProcessListURL);
            List<string> killProcessList = JsonConvert.DeserializeObject<List<string>>(result);

            List<SelectListItem> killProcessSelectList = new List<SelectListItem>();
            foreach (string p in killProcessList)
            {
                killProcessSelectList.Add(new SelectListItem
                {
                    Text = p,
                    Value = p
                });
            }

            return killProcessSelectList;
        }
    }
}
