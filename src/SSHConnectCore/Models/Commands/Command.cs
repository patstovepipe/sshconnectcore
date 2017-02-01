using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSHConnectCore.Models.Commands
{
    abstract public class Command
    {
        public SSHServer server;

        public SshCommand Run(string[] args = null)
        {
            SshCommand result;
            using (SshClient client = new SshClient(server.host, server.port, server.username, server.password))
            {
                client.Connect();
                result = RunDetails(client, args);
                client.Disconnect();
            }
            return result;
        }

        abstract public SshCommand RunDetails(SshClient client, string[] args = null);
    }
}
