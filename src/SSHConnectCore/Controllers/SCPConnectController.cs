using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SSHConnectCore.Configuration;
using SSHConnectCore.Models;
using SSHConnectCore.Models.BackupDetails;
using System.Collections.Generic;

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
            var backupDetailsList = new List<BackupDetail>();

            var fileBackupDetails = new FileBackupDetail();
            fileBackupDetails.BaseDirectory = "/home/patrick/";
            fileBackupDetails.FileName = "test-download.txt";
            fileBackupDetails.ActualFileName = "test-download.txt";
            backupDetailsList.Add(fileBackupDetails);

            var directoryBackupDetails = new DirectoryBackupDetail();
            directoryBackupDetails.BaseDirectory = "/home/patrick/";
            directoryBackupDetails.Directory = "test";
            directoryBackupDetails.ActualDirectory = "test/.";
            backupDetailsList.Add(directoryBackupDetails);

            var args = new object[] { backupDetailsList };

            var result = scpConnection.DownloadCommand(args);
            return Json(result);
        }

        public IActionResult Upload()
        {
            var result = scpConnection.UploadCommand();
            return Json(result);
        }
    }
}
