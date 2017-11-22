using Renci.SshNet;
using SSHConnectCore.Models.BackupDetails;
using System.Collections.Generic;
using System.IO;

namespace SSHConnectCore.Models.SSH.SSHCommands
{
    public class UploadCommand : SSHCommand
    {
        public override object RunDetails(SshClient client, object[] args = null)
        {
            var backupDetailList = (List<BackupDetail>)args[0];
            var results = new List<bool>();

            foreach (var backupDetail in backupDetailList)
            {
                if (backupDetail.FileSystemType == FileSystemType.File)
                {
                    var source = Path.Combine(this.downloadDirectory, backupDetail.BackupDirectory.ToString(), backupDetail.ActualName).Replace('\\', '/');
                    var target = Path.Combine(backupDetail.BaseDirectory);
                    var rsyncCommand = string.Format("sudo rsync -az --perms --chmod=777 {0} {1}", source, target);

                    var result = client.RunCommand($"echo {server.password} | " + rsyncCommand);
                    results.Add(result.ExitStatus == 0);
                }
                else if (backupDetail.FileSystemType == FileSystemType.Directory)
                {
                    var source = Path.Combine(this.downloadDirectory, backupDetail.BackupDirectory.ToString(), backupDetail.ActualName).Replace('\\', '/');
                    var target = Path.Combine(backupDetail.BaseDirectory);
                    var rsyncCommand = string.Format("sudo rsync -az --perms --chmod=777 {0} {1}", source, target);

                    var result = client.RunCommand($"echo {server.password} | " + rsyncCommand);
                    results.Add(result.ExitStatus == 0);
                }
            }

            return results;
        }
    }
}
