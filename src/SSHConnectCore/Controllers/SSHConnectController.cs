using System;
using Microsoft.AspNetCore.Mvc;
using SSHConnectCore.Models;
using Microsoft.Extensions.Options;
using SSHConnectCore.Configuration;
using System.Linq;
using Renci.SshNet.Common;
using Renci.SshNet;

namespace SSHConnectCore.Controllers
{
    public class SSHConnectController : Controller
    {
        private SSHConnection sshConnection;
        private readonly AppSettings appSettings;

        public SSHConnectController(IOptions<AppSettings> settings)
        {
            this.appSettings = settings.Value;
            this.sshConnection = new SSHConnection(this.appSettings);
        }

        public IActionResult HasConnection()
        {
            return Json(sshConnection.HasConnection());
        }

        public IActionResult Restart()
        {
            SshCommand result;

            try
            {
                result = sshConnection.RestartCommand();
            }
            catch (SshConnectionException)
            {
                return Json("Success");
            }

            return Json(result.Result);
        }

        public IActionResult Shutdown()
        {
            SshCommand result;

            try
            {
                result = sshConnection.ShutdownCommand();
            }
            catch (SshConnectionException)
            {
                return Json("Success");
            }

            return Json(result.Result);
        }

        public IActionResult KillProcess(string id)
        {
            if (string.IsNullOrEmpty(id) || !sshConnection.killProcessList.Contains(id))
            {
                return Json("Error");
            }
            else
            {
                var result = sshConnection.KillProcessCommand(new string[] { id });
                if (result.ExitStatus == 0)
                    return Json("Success");
                else
                    return Json("Error");
            }
        }

        public IActionResult KillProcessList()
        {
            return Json(sshConnection.killProcessList);
        }

        // for testing exception handling
        public IActionResult Throw()
        {
            throw new InvalidOperationException("Test Exception");
        }
    }
}
