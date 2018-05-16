using System.Collections.Generic;
using Renci.SshNet;
using SSHConnectCore.Models.BackupDetails;
using System.IO;
using System;

namespace SSHConnectCore.Models.SSH.SSHCommands
{
    public class ExistsCommand : SSHCommand
    {
        public override object RunDetails(SshClient client, object[] args = null)
        {
            var backupDetailList = (List<BackupDetail>)args[0];
            var results = new List<bool>();

            foreach (var backupDetail in backupDetailList)
            {
                var flag = backupDetail.FileSystemType == FileSystemType.File ? "f" : "d";

                var target = Path.Combine(backupDetail.BaseDirectory, backupDetail.ActualName);

                var result = client.RunCommand($"[ -{flag} {target} ] && echo \"exists\"");

                backupDetail.ExistsOnRemote = result.Result.Contains("exists");
                backupDetail.RemoteLastCheck = DateTime.UtcNow;

                if (backupDetail.FileSystemType == FileSystemType.File && backupDetail.ExistsOnRemote)
                {
                    result = client.RunCommand($"md5sum {target}");
                    var checkSum = result.Result.Split(" ")[0];

                    backupDetail.RemoteMD5CheckSum = checkSum;
                }

                results.Add(result.ExitStatus == 0);
            }

            backupDetailList.Update();
            
            return results;
        }
    }
}
