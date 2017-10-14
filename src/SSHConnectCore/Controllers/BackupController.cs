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
            ViewBag.backupDetails = BackupDetails();

            return View();
        }

        [HttpGet]
        public IActionResult NewEdit(string id)
        {
            var backupDetail = StoredBackupDetails().Where(sbd => sbd.SavedName == id).FirstOrDefault();
            if (backupDetail != null)
            {
                ViewBag.PageType = "Edit";
                return View("NewEdit", backupDetail);
            }

            ViewBag.PageType = "New";
            return View("NewEdit");
        }

        [HttpPost]
        public IActionResult NewEdit(BackupDetail model)
        {
            if (ModelState.IsValid)
            {
                var backupDetail = StoredBackupDetails().Where(sbd => sbd.SavedName == model.SavedName).FirstOrDefault();

                List<BackupDetail> storedBackupDetails;

                if (backupDetail != null)
                {
                    storedBackupDetails = StoredBackupDetails().Where(sbd => sbd.SavedName != model.SavedName).ToList();
                }
                else
                {
                    storedBackupDetails = StoredBackupDetails();
                    model.SavedName = model.ActualName;
                }

                // Check if file/folder is already recorded
                if (storedBackupDetails.Where(sbd => sbd.BaseDirectory == model.BaseDirectory 
                    && sbd.ActualName == model.ActualName && sbd.FileSystemType == model.FileSystemType).Count() == 0)
                {
                    var sameNameList = storedBackupDetails.Where(sbd => sbd.ActualName == model.ActualName)
                        .OrderBy(sbd => sbd.NameCount).ToList();

                    storedBackupDetails.RemoveAll(sbd => sbd.ActualName == model.ActualName);
                    model.BackedUp = false;
                    sameNameList.Add(model);

                    for (int i = 0; i < sameNameList.Count(); i++)
                    {
                        var _backupDetail = sameNameList[i];

                        if (i == 0)
                        {
                            _backupDetail.NameCount = 1;
                        }
                        else
                        {
                            _backupDetail.NameCount = i + 1; ;
                            _backupDetail.SavedName = _backupDetail.NameCount + "-" + _backupDetail.ActualName;
                        }

                        sameNameList[i] = _backupDetail;
                    }

                    //model.NameCount = 1;
                    //if (sameNameList.Count() >= 1)
                    //{
                    //    model.NameCount = sameNameList.OrderByDescending(sn => sn.NameCount).First().NameCount + 1;
                    //    model.SavedName = model.NameCount + "-" + model.ActualName;
                    //}

                    
                    storedBackupDetails.AddRange(sameNameList);
                    //storedBackupDetails.Add(model);
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

        //[HttpGet]
        //public IActionResult Edit(string id)
        //{
        //    var backupDetail = StoredBackupDetails().Where(sbd => sbd.SavedName == id).FirstOrDefault();
        //    ViewBag.PageType = "Edit";
        //    return View("NewEdit", backupDetail);
        //}

        //[HttpPost]
        //public IActionResult Edit(BackupDetail model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var storedBackupDetails = StoredBackupDetails().Where(sbd => sbd.SavedName != model.SavedName).ToList();
        //        if (storedBackupDetails.Where(sbd => sbd.BaseDirectory == model.BaseDirectory
        //            && sbd.ActualName == model.ActualName).Count() == 0)
        //        {
        //            var sameNameList = storedBackupDetails.Where(sbd => sbd.ActualName == model.ActualName)
        //                .OrderBy(sbd => sbd.NameCount).ToList();

        //            storedBackupDetails.RemoveAll(sbd => sbd.ActualName == model.ActualName);

        //            for (int i = 0; i < sameNameList.Count(); i++)
        //            {
        //                var backupDetail = sameNameList[i];

        //                if (i == 0)
        //                {
        //                    backupDetail.NameCount = 1;
        //                }
        //                else
        //                {
        //                    backupDetail.NameCount = i + 1; ;
        //                    backupDetail.SavedName = backupDetail.NameCount + "-" + backupDetail.SavedName;
        //                }

        //                sameNameList[i] = backupDetail;
        //            }

        //            model.NameCount = 1;
        //            if (sameNameList.Count() >= 1)
        //            {
        //                model.NameCount = sameNameList.OrderByDescending(sn => sn.NameCount).First().NameCount + 1;
        //                model.SavedName = model.NameCount + "-" + model.SavedName;
        //            }

        //            model.BackedUp = false;
        //            storedBackupDetails.AddRange(sameNameList);
        //            storedBackupDetails.Add(model);
        //            SaveBackupDetails(storedBackupDetails);

        //            return RedirectToAction("Index");
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", "This file is already recorded, search for and edit the file.");
        //        }
        //        model.BackedUp = false;
        //    }

        //    return View("NewEdit", model);
        //}

        public IActionResult Delete(string id)
        {
            var storedBackupDetails = StoredBackupDetails().Where(sbd => sbd.SavedName != id).ToList();

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
            var backupDetailsFile = Path.Combine(ServerDir(), "backup_details.json");

            var storedBackupDetails = System.IO.File.Exists(backupDetailsFile) 
                ? JsonConvert.DeserializeObject<List<BackupDetail>>(System.IO.File.ReadAllText(backupDetailsFile)) 
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
                var found = storedBackupDetails.Find(d => d.SavedName == detail.SavedName);

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

            System.IO.File.WriteAllText(Path.Combine(ServerDir(), "backup_details.json"), strJson);
        }
    }
}