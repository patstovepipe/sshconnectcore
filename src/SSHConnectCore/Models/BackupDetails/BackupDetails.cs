using DiffMatchPatch;
using Newtonsoft.Json;
using SSHConnectCore.Configuration;
using SSHConnectCore.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

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

        public static string GetHTMLDiff(this List<BackupDetail> list, string id)
        {
            var backupDetail = list.Get(id);
            var localFileLocation = Path.Combine(BackupDetails.ServerDir(), backupDetail.BackupDirectory.ToString(), backupDetail.SavedName);
            var remoteFileLocation = Path.Combine(BackupDetails.ServerDir(), "temp", backupDetail.ActualName);

            var localFileContents = File.ReadAllText(localFileLocation);
            var remoteFileContents = File.ReadAllText(remoteFileLocation);

            diff_match_patch dmp = new diff_match_patch();
            List<Diff> diff = dmp.diff_main(localFileContents, remoteFileContents);
            dmp.diff_cleanupSemantic(diff);
            var html = dmp.diff_prettyHtml(diff).Replace("&para;", "");

            return html;
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

        public static void Save(this List<BackupDetail> list, string fullFileName = null)
        {
            string strJson = JsonConvert.SerializeObject(list);

            File.WriteAllText(fullFileName ?? BackupDetails.backupDetailsFullFileName, strJson);
        }

        public static void Update(this List<BackupDetail> list)
        {
            var storedBackupDetails = BackupDetails.StoredBackupDetails();

            list.ForEach(sbd => storedBackupDetails.UpdateSingle(sbd));

            storedBackupDetails.Save();
        }

        public static void UpdateSingle(this List<BackupDetail> list, BackupDetail newBackupDetail)
        {
            var oldBackupDetail = list.Where(sbd => sbd.ID == newBackupDetail.ID).FirstOrDefault();
            if (oldBackupDetail != null)
            {
                var index = list.IndexOf(oldBackupDetail);
                list[index] = newBackupDetail;
            }
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

        public static List<BackupDetail> List(SearchViewModel model)
        {
            var fileSystemType = FileSystemType_TryParse(model.FileSystemType);
            var backupDirectory = BackupDirectory_TryParse(model.BackupDirectory);

            return List().Where(d =>
                (model.FileSystemType == "All" || d.FileSystemType == fileSystemType)
                && (model.BackupDirectory == "All" || d.BackupDirectory == backupDirectory)
                && (string.IsNullOrEmpty(model.BaseDirectory) || d.BaseDirectory.Contains(model.BaseDirectory, StringComparison.OrdinalIgnoreCase))
                && (string.IsNullOrEmpty(model.ActualName) || d.ActualName.Contains(model.ActualName, StringComparison.OrdinalIgnoreCase))
            ).ToList();
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
                    if (!string.IsNullOrWhiteSpace(detail.MD5CheckSum))
                        found.MD5CheckSum = detail.MD5CheckSum;
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
                fileBackupDetail.MD5CheckSum = CalculateMD5(file);
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

        public static List<BackupDetail> StoredBackupDetails(string fullFileName = null)
        {
            var storedBackupDetails = System.IO.File.Exists(fullFileName ?? backupDetailsFullFileName)
                ? JsonConvert.DeserializeObject<List<BackupDetail>>(File.ReadAllText(fullFileName ?? backupDetailsFullFileName))
                : new List<BackupDetail>();

            return storedBackupDetails;
        }

        public static string ServerDir()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return appSettings.windowsServerDirectory;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return appSettings.linuxServerDirectory;
            else
                throw new Exception("Windows or Linux platform not found.");
        }

        public static FileSystemType FileSystemType_TryParse(string fileSystemType)
        {
            return Enum.TryParse(fileSystemType, out FileSystemType result) ? result : FileSystemType.File;
        }

        public static BackupDirectory BackupDirectory_TryParse(string backupDirectory)
        {
            return Enum.TryParse(backupDirectory, out BackupDirectory result) ? result : BackupDirectory.Other;
        }

        private static string CalculateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }
}