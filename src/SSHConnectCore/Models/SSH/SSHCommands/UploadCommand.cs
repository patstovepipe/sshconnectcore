using Renci.SshNet;
using SSHConnectCore.Models.BackupDetails;
using System.Collections.Generic;

namespace SSHConnectCore.Models.SSH.SSHCommands
{
    public class UploadCommand : SSHCommand
    {
        public override object RunDetails(SshClient client, object[] args = null)
        {
            var backupDetailList = (List<BackupDetail>)args[0];
            var results = new List<bool>();

            foreach (var backupDetails in backupDetailList)
            {
                if (backupDetails.Type == BackupDetail.FileSystemType.File)
                {
                    var source = this.downloadDirectory + backupDetails.ActualName;
                    var target = backupDetails.BaseDirectory;
                    var rsyncCommand = string.Format("sudo rsync -az {0} {1}", source, target);

                    var result = client.RunCommand($"echo {server.password} | " + rsyncCommand);
                    results.Add(result.ExitStatus == 0);
                }
                else if (backupDetails.Type == BackupDetail.FileSystemType.Directory)
                {
                    var source = this.downloadDirectory + backupDetails.ActualName;
                    var target = backupDetails.BaseDirectory;
                    var rsyncCommand = string.Format("sudo rsync -az {0} {1}", source, target);

                    var result = client.RunCommand($"echo {server.password} | " + rsyncCommand);
                    results.Add(result.ExitStatus == 0);
                }
            }

            return results;
        }
    }
}
