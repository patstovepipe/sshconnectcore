using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SSHConnectCore.Configuration;
using SSHConnectCore.Models.BackupDetails;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace SSHConnectCore.Controllers
{
    public class BackupController : WebController
    {
        public BackupController(IOptions<AppSettings> settings) : base(settings)
        {
            BackupDetails.SetAppSettings(settings.Value);
        }

        [HttpGet]
        public IActionResult Index()
        {

            var model = new SearchViewModel();
            model.BackupDetails = BackupDetails.List();

            return View(model);
        }

        [HttpPost]
        public IActionResult Index(SearchViewModel model)
        {
            model.BackupDetails = BackupDetails.List(model);
            return View("Index", model);
        }

        [HttpGet]
        public IActionResult NewEdit(string id, string fileSystemType, string backupDirectory, string actualName)
        {
            var backupDetail = BackupDetails.StoredBackupDetails().Get(id);

            if (backupDetail != null)
            {
                ViewBag.PageType = "Edit";
                return View("NewEdit", backupDetail);
            }
            else
            {
                backupDetail = new BackupDetail();
                backupDetail.FileSystemType = BackupDetails.FileSystemType_TryParse(fileSystemType);
                backupDetail.BackupDirectory = BackupDetails.BackupDirectory_TryParse(backupDirectory);
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
                var backupDetail = BackupDetails.StoredBackupDetails().Get(model.ID);

                List<BackupDetail> storedBackupDetails;

                var newBackupDetail = false;
                if (backupDetail != null)
                    storedBackupDetails = BackupDetails.StoredBackupDetails().Exclude(model.ID);
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

                                while (model.FileSystemType == FileSystemType.File && (System.IO.File.Exists(Path.Combine(BackupDetails.ServerDir(), model.BackupDirectory.ToString(), name))
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

                                while (model.FileSystemType == FileSystemType.Directory && (System.IO.Directory.Exists(Path.Combine(BackupDetails.ServerDir(), model.BackupDirectory.ToString(), name))
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
                            model.ID = Guid.NewGuid();
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

        public IActionResult Download(string id)
        {
            return DoAPIAction();
        }

        public IActionResult Upload(string id)
        {
            return DoAPIAction();
        }

        public IActionResult Delete(string id)
        {
            var storedBackupDetails = BackupDetails.StoredBackupDetails().Exclude(id);

            storedBackupDetails.Save();

            return RedirectToAction("Index");
        }

        public IActionResult Exists(string id)
        {
            return DoAPIAction();
        }

        public IActionResult Diff(string id)
        {
            string result = APICall();

            result = JsonConvert.DeserializeObject(result).ToString();

            if (result.Equals("Success", StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.Diff = BackupDetails.StoredBackupDetails().GetHTMLDiff(id);
                Console.WriteLine(ViewBag.Diff);

                return View("DiffDisplay");
            }

            var vm = SetMessage(result);

            var model = new SearchViewModel();
            model.BackupDetails = BackupDetails.List();
            model.MessageViewModel = vm;

            return View("Index", model);
        }

        protected override IActionResult DoAPIAction()
        {
            string result = APICall();

            result = JsonConvert.DeserializeObject(result).ToString();
            var vm = SetMessage(result);

            var model = new SearchViewModel();
            model.BackupDetails = BackupDetails.List();
            model.MessageViewModel = vm;

            return View("Index", model);
        }
    }
}