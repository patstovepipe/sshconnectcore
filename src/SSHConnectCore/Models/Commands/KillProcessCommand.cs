using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Renci.SshNet;
using System.IO;

namespace SSHConnectCore.Models.Commands
{
    public class KillProcessCommand : Command
    {
        public override SshCommand RunDetails(SshClient client, string[] args)
        {
            return client.RunCommand("pkill " + args[0]);
        }
    }
}
