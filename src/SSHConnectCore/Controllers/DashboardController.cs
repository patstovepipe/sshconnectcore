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
            ViewBag.KillProcessSelectList = KillProcessSelectList();
            return View();
        }


        public IActionResult Shutdown()
        {
            return DoAction();
        }

        public IActionResult Restart()
        {
            return DoAction();
        }

        public IActionResult KillProcess(string id)
        {
            return DoAction();
        }

        public IActionResult DoAction()
        {
            string result = APICall();
            result = JsonConvert.DeserializeObject(result).ToString();
            var vm = SetMessage(result);

            return PartialView("MessagesPartial", vm);
        }

        private string APICall(string url = null)
        {
            using (var client = new HttpClient())
            {
                return client
                    .GetAsync(url ?? APIURL)
                    .Result
                    .Content.ReadAsStringAsync().Result;
            }
        }

        private MessageViewModel SetMessage(string result, string successMessage = "", string errorMessage = "")
        {
            var vm = new MessageViewModel();

            switch (result)
            {
                case "Success":
                    vm.Status = result;
                    vm.Details = successMessage;
                    break;
                case "Error":
                    vm.Status = result;
                    TempData["MessageDetails"] = errorMessage;
                    break;
                default:
                    vm.Status = "Error";
                    vm.Details = "An error ocurred.";
                    break;
            }

            return vm;
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
