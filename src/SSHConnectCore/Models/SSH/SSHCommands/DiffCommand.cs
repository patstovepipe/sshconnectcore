using System.Collections.Generic;
using Renci.SshNet;
using SSHConnectCore.Models.BackupDetails;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

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

            // If the API is hosted on a linux server we need to add some extra details
            var settings = BackupDetails.BackupDetails.appSettings;
            var linuxServerDetails = "";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                target = $"{settings.api.host}:{target}";
                linuxServerDetails = $"--rsh=\"sshpass -p {settings.api.password} ssh -l {settings.api.username}\"";
            }

            var rsyncCommand = $"sudo rsync {linuxServerDetails} -az {source} {target}";

            var result = client.RunCommand($"echo {server.password} | " + rsyncCommand);
            results.Add(result.ExitStatus == 0);

            return results;
        }
    }
}