using System.Collections.Generic;
using Renci.SshNet;
using SSHConnectCore.Models.BackupDetails;
using System.IO;
using System.Linq;

namespace SSHConnectCore.Models.SSH.SSHCommands
{
    public class DiffCommand : SSHCommand
    {
        public override object RunDetails(SshClient client, object[] args = null)
        {
            var backupDetail = ((List<BackupDetail>)args[0]).FirstOrDefault();
            var results = new List<bool>();

            var source = Path.Combine(backupDetail.BaseDirectory, backupDetail.ActualName);
            var target = Path.Combine(this.downloadDirectory, "temp");
            var rsyncCommand = string.Format("sudo rsync -az {0} {1}", source, target);

            var result = client.RunCommand($"echo {server.password} | " + rsyncCommand);
            results.Add(result.ExitStatus == 0);

            return results;
        }
    }
}
