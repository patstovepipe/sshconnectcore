using Renci.SshNet;
using SSHConnectCore.Configuration;
using SSHConnectCore.Models.SSH.SSHCommands;

namespace SSHConnectCore.Models.SSH
{
    public class SSHConnection : Connection
    {
        public string[] killProcessList;

        public SSHConnection(AppSettings appSettings) : base(appSettings)
        {
            this.killProcessList = this.appSettings.killProcessList.Split(',');
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

        public object RestartCommand()
        {
            SSHCommand command = new RestartCommand();
            return RunCommand(command);
        }

        public object ShutdownCommand()
        {
            SSHCommand command = new ShutdownCommand();
            return RunCommand(command);
        }

        public object KillProcessCommand(object[] args)
        {
            SSHCommand command = new KillProcessCommand();
            return RunCommand(command, args);
        }

        public object DownloadCommand(object[] args)
        {
            SSHCommand command = new DownloadCommand();
            return RunCommand(command, args);
        }

        public object UploadCommand(object[] args)
        {
            SSHCommand command = new UploadCommand();
            return RunCommand(command, args);
        }

        private object RunCommand(SSHCommand command, object[] args = null)
        {
            command.downloadDirectory = this.appSettings.downloadDirectory;
            command.server = this.server;

            if (args != null)
                return command.Run(args);
            else
                return command.Run();
        }
    }
}
