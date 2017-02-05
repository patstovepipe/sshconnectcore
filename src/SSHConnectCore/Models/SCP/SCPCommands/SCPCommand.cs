using Renci.SshNet;
using SSHConnectCore.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSHConnectCore.Models.SCP.SCPCommands
{
    abstract public class SCPCommand : Command
    {
        public bool Run(string[] args = null)
        {
            var result = false;

            try
            {
                using (ScpClient client = new ScpClient(server.host, server.port, server.username, server.password))
                {
                    client.Connect();
                    RunDetails(client, args);
                    client.Disconnect();
                }
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Logger.Log(this.GetType().Name, ex.Message);
            }
            return result;
        }

        abstract public void RunDetails(ScpClient client, string[] args = null);
    }
}
