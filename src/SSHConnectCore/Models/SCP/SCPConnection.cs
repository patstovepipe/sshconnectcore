using SSHConnectCore.Configuration;
using SSHConnectCore.Models.SCP.SCPCommands;

namespace SSHConnectCore.Models
{
    public class SCPConnection : Connection
    {
        public SCPConnection(AppSettings appSettings) : base(appSettings) { }

        public bool DownloadCommand(object[] args)
        {
            SCPCommand command = new DownloadCommand();
            return RunCommand(command, args);
        }

        public bool UploadCommand()
        {
            SCPCommand command = new UploadCommand();
            return RunCommand(command);
        }

        private bool RunCommand(SCPCommand command, object[] args = null)
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
