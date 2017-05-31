using Microsoft.AspNetCore.Mvc;
using SSHConnectCore.Utilities;
using System.Linq;

namespace SSHConnectCore.Controllers
{
    public class LoggerController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Lines = Logger.Logs();
            return View();
        }

        public IActionResult Logs()
        {
            var lines = string.Join("", Logger.Logs().Select(l => l + "\n"));
            return Content(lines);
        }
    }
}
