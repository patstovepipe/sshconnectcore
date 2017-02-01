namespace SSHConnectCore.Configuration
{
    public class AppSettings
    {
        public SSHServer sshServer { get; set; }
        public API api { get; set; }
        public string killprocesslist { get; set; }
    }

    public class SSHServer
    {
        public string host { get; set; }
        public string port { get; set; }
        public string username { get; set; }
        public string password { get; set; }
    }

    public class API
    {
        public string username { get; set; }
        public string password { get; set; }
    }
}
