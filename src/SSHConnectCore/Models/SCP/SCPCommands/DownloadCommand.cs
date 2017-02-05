﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Renci.SshNet;
using System.IO;

namespace SSHConnectCore.Models.SCP.SCPCommands
{
    public class DownloadCommand : SCPCommand
    {
        public override void RunDetails(ScpClient client, string[] args = null)
        {
            FileInfo file = new FileInfo(AppContext.BaseDirectory + "\\test.txt");
            using (var fs = file.Open(FileMode.Create, FileAccess.ReadWrite))
            {
                client.Download("/test.txt", fs);
            }
        }
    }
}
