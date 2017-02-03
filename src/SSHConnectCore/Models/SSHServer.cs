using SSHConnectCore.Configuration;
using System;

namespace SSHConnectCore.Models
{
    public class SSHServer
    {
        public string host { get; set; }
        public int port { get; set; }
        public string username { get; set; }
        public string password { get; set; }

        public SSHServer(AppSettings appSettings)
        {
            this.host = appSettings.sshServer.host; ;
            this.port = Convert.ToInt32(appSettings.sshServer.port);
            this.username = appSettings.sshServer.username;
            this.password = appSettings.sshServer.password;
        }
    }
}
