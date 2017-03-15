using System.Collections.Generic;
using Renci.SshNet;
using System.IO;
using SSHConnectCore.Models.Backup;

namespace SSHConnectCore.Models.SCP.SCPCommands
{
    public class DownloadCommand : SCPCommand
    {
        public override void RunDetails(ScpClient client, object[] args = null)
        {
            var backupDetailsList = (List<BackupDetails>)args[0];

            foreach(var tmpBackupDetails in backupDetailsList)
            {
                if (tmpBackupDetails.GetType() == typeof(FileBackupDetails))
                {
                    var backupDetails = (FileBackupDetails)tmpBackupDetails;

                    FileInfo file = new FileInfo(this.downloadDirectory + backupDetails.FileName);

                    using (var fs = file.Open(FileMode.Create, FileAccess.ReadWrite))
                    {
                        client.Download(backupDetails.BaseDirectory + backupDetails.ActualFileName, fs);
                    }
                }
                else if (tmpBackupDetails.GetType() == typeof(DirectoryBackupDetails))
                {
                    var backupDetails = (DirectoryBackupDetails)tmpBackupDetails;

                    DirectoryInfo directory = new DirectoryInfo(this.downloadDirectory + backupDetails.Directory);
                    if (!directory.Exists)
                        directory.Create();
                    else
                    {
                        foreach (var file in directory.GetFiles("*", SearchOption.AllDirectories))
                            file.Attributes &= ~FileAttributes.ReadOnly;
                    }

                    client.Download(backupDetails.BaseDirectory + backupDetails.ActualDirectory, directory);
                }
            }
        }
    }
}
