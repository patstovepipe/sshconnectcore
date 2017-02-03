using Renci.SshNet;
using SSHConnectCore.Configuration;
using SSHConnectCore.Models.Commands;

namespace SSHConnectCore.Models
{
    public class SSHConnection : Connection
    {
        public string[] killProcessList;

        public SSHConnection(AppSettings appSettings) : base(appSettings)
        {
            this.killProcessList = this.appSettings.killprocesslist.Split(',');
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
            SSHCommand command = new RestartCommand();
            return RunCommand(command);
        }

        public SshCommand ShutdownCommand()
        {
            SSHCommand command = new ShutdownCommand();
            return RunCommand(command);
        }

        public SshCommand KillProcessCommand(string[] args)
        {
            SSHCommand command = new KillProcessCommand();
            return RunCommand(command, args);
        }

        private SshCommand RunCommand(SSHCommand command, string[] args = null)
        {
            command.server = this.server;

            if (args != null)
                return command.Run(args);
            else
                return command.Run();
        }
    }
}
