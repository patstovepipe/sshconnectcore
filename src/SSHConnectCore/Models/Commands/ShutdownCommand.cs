﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Renci.SshNet;

namespace SSHConnectCore.Models.Commands
{
    public class ShutdownCommand : Command
    {
        public override SshCommand RunDetails(SshClient client, string[] args = null)
        {
            return client.RunCommand($"echo {server.password} | sudo -S shutdown now");
        }
    }
}