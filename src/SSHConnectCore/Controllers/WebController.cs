using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SSHConnectCore.Configuration;
using SSHConnectCore.Models.Dashboard;
using System.Net.Http;

namespace SSHConnectCore.Controllers
{
    public abstract class WebController : Controller
    {
        protected readonly AppSettings appSettings;

        protected static string apiControllerName = "SSHConnect";

        protected string CurrentURL => Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(Request);

        protected string APIURL => CurrentURL.Replace(ControllerContext.RouteData.Values["controller"].ToString(), apiControllerName);

        protected WebController (IOptions<AppSettings> settings)
        {
            this.appSettings = settings.Value;
        }

        protected virtual IActionResult DoAPIAction()
        {
            string result = APICall();
            result = JsonConvert.DeserializeObject(result).ToString();
            var vm = SetMessage(result);

            return PartialView("MessagesPartial", vm);
        }

        protected string APICall(string url = null)
        {
            using (var client = new HttpClient())
            {
                return client
                    .GetAsync(url ?? APIURL)
                    .Result
                    .Content.ReadAsStringAsync().Result;
            }
        }

        protected MessageViewModel SetMessage(string result, string successMessage = "", string errorMessage = "")
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
                    vm.Details = errorMessage;
                    break;
                default:
                    vm.Status = "Error";
                    vm.Details = "An error ocurred.";
                    break;
            }

            return vm;
        }
    }
}
