using SSHConnectCore.Configuration;

namespace SSHConnectCore.Models
{
    abstract public class Connection
    {
        protected AppSettings appSettings;
        protected RemoteServer server;

        public Connection(AppSettings appSettings)
        {
            this.appSettings = appSettings;
            this.server = new RemoteServer(this.appSettings);
        }
    }
}
