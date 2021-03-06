using System.Collections.Generic;
using Renci.SshNet;
using SSHConnectCore.Models.BackupDetails;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using SSHConnectCore.Configuration;

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
            var linuxServerDetails = "";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && Settings.appSettings.sshServer.host != "127.0.0.1")
            {
                target = $"{Settings.appSettings.api.host}:{target}";
                linuxServerDetails = $" --rsh=\"sshpass -p {Settings.appSettings.api.password} ssh -l {Settings.appSettings.api.username}\"";
            }

            var rsyncCommand = $"sudo rsync{linuxServerDetails} -az {source} {target}";
            var fullCommand = $"echo {server.password} | {rsyncCommand}";

            var result = client.RunCommand(fullCommand);
            results.Add(result.ExitStatus == 0);

            return results;
        }
    }
}
