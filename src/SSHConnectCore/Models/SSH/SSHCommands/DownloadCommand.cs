using System.Collections.Generic;
using Renci.SshNet;
using SSHConnectCore.Models.Backup;

namespace SSHConnectCore.Models.SSH.SSHCommands
{
    public class DownloadCommand : SSHCommand
    {
        public override object RunDetails(SshClient client, object[] args = null)
        {
            var backupDetailsList = (List<BackupDetails>)args[0];
            var results = new List<bool>();

            foreach (var tmpBackupDetails in backupDetailsList)
            {
                if (tmpBackupDetails.GetType() == typeof(FileBackupDetails))
                {
                    var backupDetails = (FileBackupDetails)tmpBackupDetails;

                    var source = backupDetails.BaseDirectory + backupDetails.ActualFileName;
                    var target = this.downloadDirectory;
                    var rsyncCommand = string.Format("sudo rsync -az {0} {1}", source, target);

                    var result = client.RunCommand($"echo {server.password} | " + rsyncCommand);
                    results.Add(result.ExitStatus == 0);
                }
                else if (tmpBackupDetails.GetType() == typeof(DirectoryBackupDetails))
                {
                    var backupDetails = (DirectoryBackupDetails)tmpBackupDetails;

                    var source = backupDetails.BaseDirectory + backupDetails.ActualDirectory;
                    var target = this.downloadDirectory;
                    var rsyncCommand = string.Format("sudo rsync -az {0} {1}", source, target);

                    var result = client.RunCommand($"echo {server.password} | " + rsyncCommand);
                    results.Add(result.ExitStatus == 0);
                }
            }

            return results;
        }
    }
}
