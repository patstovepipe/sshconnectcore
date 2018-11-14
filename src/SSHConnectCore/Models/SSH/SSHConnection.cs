using Microsoft.Extensions.Options;
using Renci.SshNet;
using SSHConnectCore.Configuration;
using SSHConnectCore.Models.SSH.SSHCommands;
using System;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SSHConnectCore.Models.SSH
{
    public class SSHConnection : Connection
    {
        public string[] killProcessList;

        public SSHConnection(RemoteServer server) : base(server)
        {
            this.killProcessList = Settings.appSettings.killProcessList.Split(',');
        }

        public bool HasConnection()
        {
            bool hasConnection = false;
            using (SshClient client = new SshClient(server.host, server.port, server.username, server.password))
            {
                client.ConnectionInfo.Timeout = TimeSpan.FromSeconds(1);
                client.Connect();
                hasConnection = client.IsConnected;
                client.Disconnect();
            }
            return hasConnection;
        }

        public object RunCommand(object[] args = null, [CallerMemberName] string callerMemberName = "")
        {
            var cmd = CommandFactory.Get(callerMemberName);

            cmd.downloadDirectory = Settings.appSettings.downloadDirectory;
            cmd.server = this.server;

            try
            {
                if (args != null)
                    return cmd.Run(args);
                else
                    return cmd.Run();
            }
            catch (AggregateException ex)
            {
                foreach (Exception x in ex.InnerExceptions)
                {
                    if (x.GetType().GetTypeInfo().BaseType == typeof(SocketException))
                    {
                        return "Error";
                    }
                }

                throw ex;
            }
        }
    }
}
