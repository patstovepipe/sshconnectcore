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
using SSHConnectCore.Utilities;
using System.Reflection;
using System.Net.Sockets;

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
                sshConnection.RestartCommand();
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
                sshConnection.ShutdownCommand();
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
                var result = sshConnection.KillProcessCommand(new string[] { id });
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

        public IActionResult Download()
        {
            var backupDetailsList = new List<BackupDetail>();

            var fileBackupDetails = new FileBackupDetail();
            fileBackupDetails.BaseDirectory = "/home/patrick/";
            fileBackupDetails.FileName = "test-download.txt";
            fileBackupDetails.ActualFileName = "test-download.txt";
            backupDetailsList.Add(fileBackupDetails);

            var directoryBackupDetails = new DirectoryBackupDetail();
            directoryBackupDetails.BaseDirectory = "/home/patrick/";
            directoryBackupDetails.Directory = "test";
            directoryBackupDetails.ActualDirectory = "test";
            backupDetailsList.Add(directoryBackupDetails);

            var args = new object[] { backupDetailsList };

            var results = (List<bool>)sshConnection.DownloadCommand(args);

            if (results.All(r => r))
                return Json("Success");
            else
                return Json("Error");
        }

        public IActionResult Upload()
        {
            var backupDetailsList = new List<BackupDetail>();

            var fileBackupDetails = new FileBackupDetail();
            fileBackupDetails.BaseDirectory = "/home/patrick/";
            fileBackupDetails.FileName = "test-upload.txt";
            fileBackupDetails.ActualFileName = "test-upload.txt";
            backupDetailsList.Add(fileBackupDetails);

            var directoryBackupDetails = new DirectoryBackupDetail();
            directoryBackupDetails.BaseDirectory = "/home/patrick/";
            directoryBackupDetails.Directory = "test-upload";
            directoryBackupDetails.ActualDirectory = "test-upload";
            backupDetailsList.Add(directoryBackupDetails);

            var args = new object[] { backupDetailsList };

            var results = (List<bool>)sshConnection.UploadCommand(args);

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
    }
}
