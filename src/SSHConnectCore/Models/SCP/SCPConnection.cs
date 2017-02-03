using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Renci.SshNet;
using SSHConnectCore.Configuration;
using System.IO;

namespace SSHConnectCore.Models
{
    public class SCPConnection : Connection
    {
        public SCPConnection(AppSettings appSettings) : base(appSettings) { }

        public void Backup()
        {
            using (ScpClient client = new ScpClient(server.host, server.port, server.username, server.password))
            {
                client.Connect();
                FileInfo file = new FileInfo(AppContext.BaseDirectory + "\\test.txt");
                using (var fs = file.Open(FileMode.Create, FileAccess.ReadWrite))
                {
                    client.Download("/home/patrick/test.txt", fs);
                }
                client.Disconnect();
            }
        }
    }
}
