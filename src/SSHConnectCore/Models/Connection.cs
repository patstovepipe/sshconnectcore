using Microsoft.Extensions.Options;
using SSHConnectCore.Configuration;

namespace SSHConnectCore.Models
{
    abstract public class Connection
    {
        protected RemoteServer server;

        public Connection(RemoteServer remoteServer)
        {
            this.server = remoteServer;
        }
    }
}
