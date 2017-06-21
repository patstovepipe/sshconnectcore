using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SSHConnectCore.Configuration;
using SSHConnectCore.Models.BackupDetails;
using SSHConnectCore.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace SSHConnectCore.Controllers
{
    public class BackupController : Controller
    {
        private readonly AppSettings appSettings;

        public BackupController(IOptions<AppSettings> settings)
        {
            this.appSettings = settings.Value;
        }

        public IActionResult Index()
        {
            string serverDir = ServerDir();

            List<BackupDetail> backupDetails = new List<BackupDetail>();
            foreach(string file in Directory.GetFiles(serverDir))
            {
                var fileBackupDetail = new FileBackupDetail();
                fileBackupDetail.BaseDirectory = serverDir;
                fileBackupDetail.FileName = Path.GetFileName(file);
                backupDetails.Add(fileBackupDetail);
            }
            foreach (string dir in Directory.GetDirectories(serverDir))
            {
                var directoryBackupDetail = new DirectoryBackupDetail();
                directoryBackupDetail.BaseDirectory = serverDir;
                directoryBackupDetail.Directory = Path.GetFileName(dir);
                backupDetails.Add(directoryBackupDetail);
            }

            ViewBag.backupDetails = backupDetails;

            return View();
        }

        private string ServerDir()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return this.appSettings.windowsServerDirectory;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return this.appSettings.linuxServerDirectory;
            else
                throw new Exception("Windows or Linux platform not found.");
        }
    }
}