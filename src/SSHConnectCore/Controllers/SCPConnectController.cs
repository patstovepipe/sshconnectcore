using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SSHConnectCore.Configuration;
using SSHConnectCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSHConnectCore.Controllers
{
    public class SCPConnectController : Controller
    {
        private SCPConnection scpConnection;
        private readonly AppSettings appSettings;

        public SCPConnectController(IOptions<AppSettings> settings)
        {
            this.appSettings = settings.Value;
            this.scpConnection = new SCPConnection(this.appSettings);
        }

        public IActionResult Download()
        {
            var result = scpConnection.DownloadCommand();
            return Json(result);
        }
    }
}
