using Microsoft.AspNetCore.Mvc;
using SSHConnectCore.Utilities;
using System.Linq;

namespace SSHConnectCore.Controllers
{
    public class LoggerController : Controller
    {
        public IActionResult Logs()
        {
            var lines = string.Join("", Logger.Logs().Select(l => l + "\n"));
            return Content(lines);
        }
    }
}
