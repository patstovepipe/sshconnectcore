﻿using Microsoft.AspNetCore.Mvc;
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
        public BackupController(IOptions<AppSettings> settings) : base(settings)
        {
            BackupDetails.SetAppSettings(settings.Value);
        }

        public IActionResult Index()
        {
            ViewBag.backupDetails = BackupDetails.List();

            return View();
        }

        [HttpGet]
        public IActionResult NewEdit(string savedName, string fileSystemType, string backupDirectory, string actualName)
        {
            var backupDetail = BackupDetails.StoredBackupDetails().Get(savedName, fileSystemType);

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
                var backupDetail = BackupDetails.StoredBackupDetails().Get(model.SavedName, null, model.FileSystemType);

                List<BackupDetail> storedBackupDetails;

                var newBackupDetail = false;
                if (backupDetail != null)
                    storedBackupDetails = BackupDetails.StoredBackupDetails().Exclude(model.SavedName, null, model.FileSystemType);
                else
                {
                    newBackupDetail = true;
                    storedBackupDetails = BackupDetails.StoredBackupDetails();
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

                    storedBackupDetails.Save();

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
            var detail = BackupDetails.StoredBackupDetails().Get(savedName, fileSystemType);
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
            var storedBackupDetails = BackupDetails.StoredBackupDetails().Exclude(savedName, fileSystemType);

            storedBackupDetails.Save();

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
    }
}