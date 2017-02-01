using Renci.SshNet;
using Renci.SshNet.Common;
using SSHConnectCore.Configuration;
using SSHConnectCore.Utilities;
using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using SSHConnectCore.Models.Commands;

namespace SSHConnectCore.Models
{
    public class SSHConnection
    {
        public static string[] killProcessList;
        private AppSettings appSettings;
        private SSHServer server;
        private SshClient client;

        public SSHConnection(AppSettings appSettings)
        {
            killProcessList = appSettings.killprocesslist.Split(',');

            this.appSettings = appSettings;
            server = new SSHServer(this.appSettings);

            client = new SshClient(server.host, server.port, server.username, server.password);
        }

        public void Connect()
        {
            try
            {
                client.Connect();
            }
            catch (Exception ex)
            {
                Logger.Log(this.GetType().Name, ex.Message);
            }
        }

        public bool IsConnected()
        {
            if (client == null)
                client = new SshClient(server.host, server.port, server.username, server.password);
            return client.IsConnected;
        }

        public SshCommand RestartCommand()
        {
            Command command = new RestartCommand();
            return RunCommand(command);
        }

        public SshCommand ShutdownCommand()
        {
            Command command = new ShutdownCommand();
            return RunCommand(command);
        }

        public SshCommand KillProcessCommand(string[] args)
        {
            Command command = new KillProcessCommand();
            return RunCommand(command, args);
        }

        private SshCommand RunCommand(Command command, string[] args = null)
        {
            command.server = this.server;

            if (args != null)
                return command.Run(args);
            else
                return command.Run();
        }
    }
}
