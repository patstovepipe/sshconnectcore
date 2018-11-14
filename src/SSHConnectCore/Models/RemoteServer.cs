using Microsoft.Extensions.Options;
using SSHConnectCore.Configuration;
using System;

namespace SSHConnectCore.Models
{
    public class RemoteServer
    {
        public string host { get; set; }
        public int port { get; set; }
        public string username { get; set; }
        public string password { get; set; }

        public RemoteServer()
        {
            this.host = Settings.appSettings.sshServer.host; ;
            this.port = Convert.ToInt32(Settings.appSettings.sshServer.port);
            this.username = Settings.appSettings.sshServer.username;
            this.password = Settings.appSettings.sshServer.password;
        }
    }
}
