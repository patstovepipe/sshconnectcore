using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Renci.SshNet;
using SSHConnectCore.Configuration;
using System.IO;
using SSHConnectCore.Models.SCP.SCPCommands;

namespace SSHConnectCore.Models
{
    public class SCPConnection : Connection
    {
        public SCPConnection(AppSettings appSettings) : base(appSettings) { }

        public bool DownloadCommand()
        {
            SCPCommand command = new DownloadCommand();
            return RunCommand(command);
        }

        public bool UploadCommand()
        {
            SCPCommand command = new UploadCommand();
            return RunCommand(command);
        }

        private bool RunCommand(SCPCommand command, string[] args = null)
        {
            command.server = this.server;

            if (args != null)
                return command.Run(args);
            else
                return command.Run();
        }
    }
}
