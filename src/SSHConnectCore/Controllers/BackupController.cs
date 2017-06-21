using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
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
            var serverBackupDetails = ServerBackupDetails();

            var storedBackupDetails = StoredBackupDetails();

            ViewBag.backupDetails = serverBackupDetails;

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

        private List<BackupDetail> ServerBackupDetails()
        {
            string serverDir = ServerDir();

            List<BackupDetail> serverBackupDetails = new List<BackupDetail>();
            foreach (string file in Directory.GetFiles(serverDir))
            {
                var fileBackupDetail = new BackupDetail();
                fileBackupDetail.BaseDirectory = serverDir;
                fileBackupDetail.Name = Path.GetFileName(file);
                serverBackupDetails.Add(fileBackupDetail);
            }
            foreach (string dir in Directory.GetDirectories(serverDir))
            {
                var directoryBackupDetail = new BackupDetail();
                directoryBackupDetail.BaseDirectory = serverDir;
                directoryBackupDetail.Name = Path.GetFileName(dir);
                serverBackupDetails.Add(directoryBackupDetail);
            }

            return serverBackupDetails;
        }

        private List<BackupDetail> StoredBackupDetails()
        {
            var backupDetailsFile = Path.Combine(ServerDir(), "backup_details.json");

            var storedBackupDetails = JsonConvert.DeserializeObject<List<BackupDetail>>(System.IO.File.ReadAllText(backupDetailsFile));

            return storedBackupDetails;
        }

        private void saveToFile()
        {
            var x = new BackupDetail();
            x.BaseDirectory = "/home/patrick/";
            x.BackupFolder = "other";
            x.Name = "test-download.txt";
            x.ActualName = "test-download.txt";
            x.Type = BackupDetail.FileSystemType.File;

            var bdl = new List<BackupDetail>();

            bdl.Add(x);

            string strJson = JsonConvert.SerializeObject(bdl);

            System.IO.File.WriteAllText(Path.Combine(ServerDir(), "backup_details.json"), strJson);
        }
    }
}