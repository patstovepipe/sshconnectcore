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

            var vm = new MessageViewModel();

            return View(vm);
        }


        public IActionResult Shutdown()
        {
            string result = APICall();
            result = JsonConvert.DeserializeObject(result).ToString();
            var vm = SetMessage(result);

            return PartialView("MessagesPartial", vm);
        }

        public IActionResult Restart()
        {
            string result = APICall();
            result = JsonConvert.DeserializeObject(result).ToString();
            var vm = SetMessage(result);

            return PartialView("MessagesPartial", vm);
        }

        public IActionResult KillProcess(string id)
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

            //switch (result)
            //{
            //    case "Success":
            //        TempData["MessageStatus"] = result;
            //        TempData["MessageDetails"] = successMessage;
            //        break;
            //    case "Error":
            //        TempData["MessageStatus"] = result;
            //        TempData["MessageDetails"] = errorMessage;
            //        break;
            //    default:
            //        TempData["MessageStatus"] = "Error";
            //        TempData["MessageDetails"] = "An error ocurred.";
            //        break;
            //}
        }

        private List<SelectListItem> KillProcessSelectList()
        {
            var killProcessListURL = APIURL + apiControllerName + "/KillProcessList";
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
