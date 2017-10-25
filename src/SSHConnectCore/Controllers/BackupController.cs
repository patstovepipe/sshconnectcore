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
    public class BackupController : WebController
    {
        private string backupDetailsFileName = "backup_details.json";
        private string backupDetailsFullFileName => Path.Combine(ServerDir(), backupDetailsFileName);

        public BackupController(IOptions<AppSettings> settings) : base(settings)
        {
        }

        public IActionResult Index()
        {
            ViewBag.backupDetails = BackupDetails();

            return View();
        }

        [HttpGet]
        public IActionResult NewEdit(string savedName, string fileSystemType, string backupDirectory, string actualName)
        {
            var backupDetail = BackupDetailFirstOrDefault(savedName, fileSystemType);

            if (backupDetail != null)
            {
                ViewBag.PageType = "Edit";
                return View("NewEdit", backupDetail);
            }
            else
            {
                backupDetail = new BackupDetail();
                backupDetail.FileSystemType = Enum.TryParse(fileSystemType, out FileSystemType result) ? result : FileSystemType.File;
                backupDetail.BackupDirectory = Enum.TryParse(backupDirectory, out BackupDirectory result2) ? result2 : BackupDirectory.Other;
                backupDetail.ActualName = actualName;
            }

            ViewBag.PageType = "New";
            return View("NewEdit", backupDetail);
        }

        [HttpPost]
        [ActionName("NewEdit")]
        public IActionResult NewEditPost(BackupDetail model)
        {
            if (ModelState.IsValid)
            {
                var backupDetail = BackupDetailFirstOrDefault(model.SavedName, null, model.FileSystemType);

                List<BackupDetail> storedBackupDetails;

                var newBackupDetail = false;
                if (backupDetail != null)
                    storedBackupDetails = BackupDetailListRemove(model.SavedName, null, model.FileSystemType);
                else
                {
                    newBackupDetail = true;
                    storedBackupDetails = StoredBackupDetails();
                }

                // Check if file/folder is already recorded
                if (storedBackupDetails.Where(sbd => sbd.BaseDirectory == model.BaseDirectory
                    && sbd.ActualName == model.ActualName && sbd.FileSystemType == model.FileSystemType).Count() == 0)
                {
                    model.BackedUp = false;

                    if (newBackupDetail)
                    {
                        string name = "";

                        if (model.FileSystemType == FileSystemType.File)
                        {
                            if (!storedBackupDetails.Exists(sbd => sbd.SavedName == model.ActualName && sbd.FileSystemType == model.FileSystemType))
                                name = model.ActualName;
                            else
                            {
                                var counter = 2;
                                name = counter + "-" + model.ActualName;

                                while (model.FileSystemType == FileSystemType.File && (System.IO.File.Exists(Path.Combine(ServerDir(), BackupDirectory.Other.ToString(), name))
                                        || storedBackupDetails.Exists(sbd => sbd.SavedName == name && sbd.FileSystemType == model.FileSystemType)))
                                {
                                    counter++;
                                    name = counter + "-" + model.ActualName;
                                }
                            }
                        }
                        else
                        {
                            if (!storedBackupDetails.Exists(sbd => sbd.SavedName == model.ActualName && sbd.FileSystemType == model.FileSystemType))
                                name = model.ActualName;
                            else
                            {
                                var counter = 2;
                                name = counter + "-" + model.ActualName;

                                while (model.FileSystemType == FileSystemType.Directory && (System.IO.Directory.Exists(Path.Combine(ServerDir(), BackupDirectory.Other.ToString(), name))
                                    || storedBackupDetails.Exists(sbd => sbd.SavedName == name && sbd.FileSystemType == model.FileSystemType)))
                                {
                                    counter++;
                                    name = counter + "-" + model.ActualName;
                                }
                            }
                        }

                        if (string.IsNullOrEmpty(name))
                            throw new Exception("Saved name is empty or null.");
                        else
                        {
                            model.SavedName = name;
                            storedBackupDetails.Add(model);
                        }
                    }
                    else
                    {
                        storedBackupDetails.Add(model);
                    }

                    SaveBackupDetails(storedBackupDetails);

                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", $"This {model.FileSystemType.ToString().ToLower()} is already recorded, search for and edit the {model.FileSystemType.ToString().ToLower()}.");
                }
            }

            return View("NewEdit", model);
        }

        public IActionResult Download(string savedName, string fileSystemType)
        {

            return DoAPIAction();
        }

        protected override IActionResult DoAPIAction()
        {
            string url = APIURL;

            string result = APICall();
            result = JsonConvert.DeserializeObject(result).ToString();
            var vm = SetMessage(result);

            return PartialView("MessagesPartial", vm);
        }

        public IActionResult Delete(string savedName, string fileSystemType)
        {
            var storedBackupDetails = BackupDetailListRemove(savedName, fileSystemType);

            SaveBackupDetails(storedBackupDetails);

            return RedirectToAction("Index");
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

        private List<BackupDetail> ServerBackupDetails(BackupDirectory backupDirectory)
        {
            string serverDir = Path.Combine(ServerDir(), backupDirectory.ToString());

            List<BackupDetail> serverBackupDetails = new List<BackupDetail>();
            foreach (string file in Directory.GetFiles(serverDir))
            {
                var fileBackupDetail = new BackupDetail();
                fileBackupDetail.SavedName = Path.GetFileName(file);
                fileBackupDetail.ActualName = fileBackupDetail.SavedName;
                fileBackupDetail.FileSystemType = FileSystemType.File;
                fileBackupDetail.BackupDirectory = backupDirectory;
                serverBackupDetails.Add(fileBackupDetail);
            }
            foreach (string dir in Directory.GetDirectories(serverDir))
            {
                var directoryBackupDetail = new BackupDetail();
                directoryBackupDetail.SavedName = Path.GetFileName(dir);
                directoryBackupDetail.ActualName = directoryBackupDetail.SavedName;
                directoryBackupDetail.FileSystemType = FileSystemType.Directory;
                directoryBackupDetail.BackupDirectory = backupDirectory;
                serverBackupDetails.Add(directoryBackupDetail);
            }

            return serverBackupDetails;
        }

        private List<BackupDetail> StoredBackupDetails()
        {
            var storedBackupDetails = System.IO.File.Exists(backupDetailsFullFileName)
                ? JsonConvert.DeserializeObject<List<BackupDetail>>(System.IO.File.ReadAllText(backupDetailsFullFileName))
                : new List<BackupDetail>();

            return storedBackupDetails;
        }

        private List<BackupDetail> BackupDetails()
        {
            var serverBackupDetails = ServerBackupDetails(BackupDirectory.Other);
            serverBackupDetails.AddRange(ServerBackupDetails(BackupDirectory.Roms));

            var storedBackupDetails = StoredBackupDetails();

            foreach (var detail in serverBackupDetails.ToList())
            {
                var found = storedBackupDetails.Find(d => d.SavedName == detail.SavedName && d.FileSystemType == detail.FileSystemType);

                if (found != null)
                {
                    serverBackupDetails.Remove(detail);
                    found.BackedUp = true;
                    serverBackupDetails.Add(found);
                    storedBackupDetails.Remove(found);
                }
            }

            serverBackupDetails.AddRange(storedBackupDetails);

            return serverBackupDetails.OrderBy(d => d.SavedName).ToList();
        }

        private void SaveBackupDetails(List<BackupDetail> backupDetailsList)
        {
            string strJson = JsonConvert.SerializeObject(backupDetailsList);

            System.IO.File.WriteAllText(backupDetailsFullFileName, strJson);
        }

        private BackupDetail BackupDetailFirstOrDefault(string savedName, string fileSystemTypeString = null, FileSystemType fileSystemType = FileSystemType.File)
        {
            var fst = fileSystemTypeString == null ? fileSystemType : (Enum.TryParse(fileSystemTypeString, out FileSystemType result) ? result : FileSystemType.File);

            return StoredBackupDetails().Where(sbd => sbd.SavedName == savedName && sbd.FileSystemType == fst).FirstOrDefault();
        }

        private List<BackupDetail> BackupDetailListRemove(string savedName, string fileSystemTypeString = null, FileSystemType fileSystemType = FileSystemType.File)
        {
            var fst = fileSystemTypeString == null ? fileSystemType : (Enum.TryParse(fileSystemTypeString, out FileSystemType result) ? result : FileSystemType.File);

            return StoredBackupDetails().Where(sbd => !(sbd.SavedName == savedName && sbd.FileSystemType == fst)).ToList();
        }
    }
}