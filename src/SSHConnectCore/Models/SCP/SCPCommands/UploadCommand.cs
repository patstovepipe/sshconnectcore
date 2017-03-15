using System;
using Renci.SshNet;
using System.IO;

namespace SSHConnectCore.Models.SCP.SCPCommands
{
    public class UploadCommand : SCPCommand
    {
        public override void RunDetails(ScpClient client, object[] args = null)
        {
            FileInfo file = new FileInfo(AppContext.BaseDirectory + "\\test-upload.txt");
            client.Upload(file, "/test-upload.txt");
        }
    }
}