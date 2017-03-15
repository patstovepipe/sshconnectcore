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
                result = (SshCommand)sshConnection.RestartCommand();
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
                result = (SshCommand)sshConnection.ShutdownCommand();
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
                var result = (SshCommand)sshConnection.KillProcessCommand(new string[] { id });
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
