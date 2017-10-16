using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SSHConnectCore.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SSHConnectCore.Controllers
{
    public abstract class WebController : Controller
    {
        protected readonly AppSettings appSettings;

        protected static string apiControllerName = "SSHConnect";

        protected string CurrentURL => Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(Request);

        protected string APIURL => CurrentURL.Replace(ControllerContext.RouteData.Values["controller"].ToString(), apiControllerName);

        public WebController (IOptions<AppSettings> settings)
        {
            this.appSettings = settings.Value;
        }
    }
}
