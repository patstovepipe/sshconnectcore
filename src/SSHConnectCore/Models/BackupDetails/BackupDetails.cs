﻿using Newtonsoft.Json;
using SSHConnectCore.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace SSHConnectCore.Models.BackupDetails
{
    public static class Extensions
    {
        public static BackupDetail Get(this List<BackupDetail> list, string id)
        {
            if (Guid.TryParse(id, out Guid result))
                return list.Get(result);

            return null;
        }

        public static BackupDetail Get(this List<BackupDetail> list, Guid id)
        {
            return list.Where(sbd => sbd.ID == id).FirstOrDefault();
        }

        public static List<BackupDetail> Exclude(this List<BackupDetail> list, string id)
        {
            if (Guid.TryParse(id, out Guid result))
                return list.Exclude(result);

            return null;
        }

        public static List<BackupDetail> Exclude(this List<BackupDetail> list, Guid id)
        {
            return list.Where(sbd => !(sbd.ID == id)).ToList();
        }

        public static void Save(this List<BackupDetail> list)
        {
            string strJson = JsonConvert.SerializeObject(list);

            File.WriteAllText(BackupDetails.backupDetailsFullFileName, strJson);
        }
    }

    public static class BackupDetails
    {
        private static AppSettings appSettings;
        private static string backupDetailsFileName = "backup_details.json";
        public static string backupDetailsFullFileName => Path.Combine(ServerDir(), backupDetailsFileName);

        public static void SetAppSettings(AppSettings settings)
        {
            if (appSettings == null)
                appSettings = settings;
        }

        public static List<BackupDetail> List()
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

        private static List<BackupDetail> ServerBackupDetails(BackupDirectory backupDirectory)
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

        public static List<BackupDetail> StoredBackupDetails()
        {
            var storedBackupDetails = System.IO.File.Exists(backupDetailsFullFileName)
                ? JsonConvert.DeserializeObject<List<BackupDetail>>(File.ReadAllText(backupDetailsFullFileName))
                : new List<BackupDetail>();

            return storedBackupDetails;
        }

        private static string ServerDir()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return appSettings.windowsServerDirectory;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return appSettings.linuxServerDirectory;
            else
                throw new Exception("Windows or Linux platform not found.");
        }
    }
}