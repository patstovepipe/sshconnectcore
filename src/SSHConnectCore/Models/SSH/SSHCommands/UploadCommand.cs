﻿using Renci.SshNet;
using SSHConnectCore.Configuration;
using SSHConnectCore.Models.BackupDetails;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

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
                var source = Path.Combine(this.downloadDirectory, backupDetail.BackupDirectory.ToString(), backupDetail.ActualName).Replace('\\', '/');
                var target = Path.Combine(backupDetail.BaseDirectory);

                // If the API is hosted on a linux server we need to add some extra details
                var linuxServerDetails = "";
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && Settings.appSettings.sshServer.host != "127.0.0.1")
                { 
                    source = $"{Settings.appSettings.api.host}:{source}";
                    linuxServerDetails = $" --rsh=\"sshpass -p {Settings.appSettings.api.password} ssh -l {Settings.appSettings.api.username}\"";
                }

                var rsyncCommand = $"sudo rsync{linuxServerDetails} -az --perms --chmod=777 {source} {target}";
                var fullCommand = $"echo {server.password} | {rsyncCommand}";

                var result = client.RunCommand(fullCommand);
                results.Add(result.ExitStatus == 0);
            }

            return results;
        }
    }
}
