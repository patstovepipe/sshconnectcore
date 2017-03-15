using System.Collections.Generic;
using Renci.SshNet;
using System.IO;
using SSHConnectCore.Models.BackupDetails;
namespace SSHConnectCore.Models.SCP.SCPCommands
{
    public class DownloadCommand : SCPCommand
    {
        public override void RunDetails(ScpClient client, object[] args = null)
        {
            var backupDetailList = (List<BackupDetail>)args[0];

            foreach(var tmpBackupDetails in backupDetailList)
            {
                if (tmpBackupDetails.GetType() == typeof(FileBackupDetail))
                {
                    var backupDetails = (FileBackupDetail)tmpBackupDetails;

                    FileInfo file = new FileInfo(this.downloadDirectory + backupDetails.FileName);

                    using (var fs = file.Open(FileMode.Create, FileAccess.ReadWrite))
                    {
                        client.Download(backupDetails.BaseDirectory + backupDetails.ActualFileName, fs);
                    }
                }
                else if (tmpBackupDetails.GetType() == typeof(DirectoryBackupDetail))
                {
                    var backupDetails = (DirectoryBackupDetail)tmpBackupDetails;

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
