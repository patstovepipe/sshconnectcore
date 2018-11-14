using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SSHConnectCore.Configuration;
using System.Linq;
using Renci.SshNet.Common;
using Renci.SshNet;
using System.Collections.Generic;
using SSHConnectCore.Models.BackupDetails;
using SSHConnectCore.Models.SSH;
using System.Reflection;
using System.Net.Sockets;
using SSHConnectCore.Models;
using SSHConnectCore.Models.SSH.SSHCommands;
using System.Runtime.CompilerServices;

namespace SSHConnectCore.Controllers
{
    public class SSHConnectController : Controller
    {
        private readonly SSHConnection sshConnection;

        public SSHConnectController(SSHConnection sshConnection)
        {
            this.sshConnection = sshConnection;
        }

        public IActionResult HasConnection()
        {
            try
            {
                return Json(sshConnection.HasConnection());
            }
            catch (SshConnectionException)
            {
                return Json(false);
            }
            catch (SshOperationTimeoutException)
            {
                return Json(false);
            }
            catch (AggregateException ex)
            {
                foreach (Exception x in ex.InnerExceptions)
                {
                    if (x.GetType().GetTypeInfo().BaseType == typeof(SocketException))
                    {
                        return Json(false);
                    }
                }

                throw ex;
            }
        }

        public IActionResult Restart()
        {
            try
            {
                sshConnection.RunCommand();
            }
            catch (SshConnectionException)
            {
                return Json("Success");
            }

            return Json("Error");
        }

        public IActionResult Shutdown()
        {
            try
            {
                sshConnection.RunCommand();
            }
            catch (SshConnectionException)
            {
                return Json("Success");
            }

            return Json("Error");
        }

        public IActionResult KillProcess(string id)
        {
            if (string.IsNullOrEmpty(id) || !sshConnection.killProcessList.Contains(id))
            {
                return Json("Error");
            }
            else
            {
                var result = sshConnection.RunCommand(new string[] { id });
                SshCommand sshCommandResult;

                if (result.GetType() == typeof(SshCommand))
                {
                    sshCommandResult = (SshCommand)result;

                    if (sshCommandResult.ExitStatus == 0)
                        return Json("Success");
                    else
                        return Json("Error");
                }
                else
                    return Json(result);
            }
        }

        public IActionResult KillProcessList()
        {
            return Json(sshConnection.killProcessList);
        }

        public IActionResult Download(string id)
        {
            return GetResult(id);
        }

        public IActionResult Upload(string id)
        {
            return GetResult(id);
        }

        public IActionResult Exists(string id)
        {
            return GetResult(id);
        }

        public IActionResult Diff(string id)
        {
            return GetResult(id);
        }

        private IActionResult GetResult(string id, [CallerMemberName] string callerMemberName = "")
        {
            var results = (List<bool>)sshConnection.RunCommand(BackupDetailsArgs(id), callerMemberName);

            if (results.All(r => r))
                return Json("Success");
            else
                return Json("Error");
        }

        // for testing exception handling
        public IActionResult Throw()
        {
            throw new InvalidOperationException("Test Exception");
        }

        private object[] BackupDetailsArgs(string id)
        {
            var backupDetailsList = new List<BackupDetail>();

            if (string.IsNullOrEmpty(id))
                backupDetailsList = BackupDetails.StoredBackupDetails();
            else
                backupDetailsList.Add(BackupDetails.StoredBackupDetails().Get(id));

            return new object[] { backupDetailsList };
        }
    }
}
