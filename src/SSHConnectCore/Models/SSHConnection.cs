using Renci.SshNet;
using SSHConnectCore.Configuration;
using SSHConnectCore.Models.Commands;

namespace SSHConnectCore.Models
{
    public class SSHConnection
    {
        public string[] killProcessList;
        private AppSettings appSettings;
        private SSHServer server;

        public SSHConnection(AppSettings appSettings)
        {
            this.appSettings = appSettings;
            this.killProcessList = this.appSettings.killprocesslist.Split(',');
            this.server = new SSHServer(this.appSettings);
        }

        public bool HasConnection()
        {
            bool hasConnection = false;
            using (SshClient client = new SshClient(server.host, server.port, server.username, server.password))
            {
                client.Connect();
                hasConnection = client.IsConnected;
                client.Disconnect();
            }

            return hasConnection;
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
