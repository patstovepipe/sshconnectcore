using System.Collections.Generic;
using Renci.SshNet;
using SSHConnectCore.Models.BackupDetails;
using System.IO;

namespace SSHConnectCore.Models.SSH.SSHCommands
{
    public class DownloadCommand : SSHCommand
    {
        public override object RunDetails(SshClient client, object[] args = null)
        {
            var backupDetailList = (List<BackupDetail>)args[0];
            var results = new List<bool>();

            foreach (var backupDetail in backupDetailList)
            {
                if (backupDetail.FileSystemType == FileSystemType.File)
                {
                    var source = Path.Combine(backupDetail.BaseDirectory, backupDetail.ActualName);
                    var target = Path.Combine(this.downloadDirectory, backupDetail.BackupDirectory.ToString());
                    var rsyncCommand = string.Format("sudo rsync -az {0} {1}", source, target);

                    var result = client.RunCommand($"echo {server.password} | " + rsyncCommand);
                    results.Add(result.ExitStatus == 0);
                }
                else if (backupDetail.FileSystemType == FileSystemType.Directory)
                {
                    var source = Path.Combine(backupDetail.BaseDirectory, backupDetail.ActualName);
                    var target = Path.Combine(this.downloadDirectory, backupDetail.BackupDirectory.ToString());
                    var rsyncCommand = string.Format("sudo rsync -az {0} {1}", source, target);

                    var result = client.RunCommand($"echo {server.password} | " + rsyncCommand);
                    results.Add(result.ExitStatus == 0);
                }
            }

            return results;
        }
    }
}
