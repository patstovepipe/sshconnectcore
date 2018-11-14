using System.Collections.Generic;
using Renci.SshNet;
using SSHConnectCore.Models.BackupDetails;
using System.IO;
using System.Runtime.InteropServices;
using SSHConnectCore.Configuration;

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
                var source = Path.Combine(backupDetail.BaseDirectory, backupDetail.ActualName);
                var target = Path.Combine(this.downloadDirectory, backupDetail.BackupDirectory.ToString());

                // If the API is hosted on a linux server we need to add some extra details
                var linuxServerDetails = "";
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                { 
                    target = $"{Settings.appSettings.api.host}:{target}";
                    linuxServerDetails = $"--rsh=\"sshpass -p {Settings.appSettings.api.password} ssh -l {Settings.appSettings.api.username}\"";
                }

                var rsyncCommand = $"sudo rsync {linuxServerDetails} -az {source} {target}";

                var result = client.RunCommand($"echo {server.password} | " + rsyncCommand);
                results.Add(result.ExitStatus == 0);
            }

            return results;
        }
    }
}
