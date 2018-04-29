using Microsoft.Extensions.Options;
using SSHConnectCore.Configuration;

namespace SSHConnectCore.Models
{
    abstract public class Connection
    {
        protected AppSettings appSettings;
        protected RemoteServer server;

        public Connection(IOptions<AppSettings> settings, RemoteServer remoteServer)
        {
            this.appSettings = settings.Value;
            this.server = remoteServer;
        }
    }
}
