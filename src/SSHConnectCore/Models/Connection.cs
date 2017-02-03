using SSHConnectCore.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
