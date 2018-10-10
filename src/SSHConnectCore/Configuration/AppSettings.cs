/*
To edit appsettings.json locally run this command (which untracks file)
    git update-index --assume-unchanged path/to/file
To start tracking file again
    git update-index --no-assume-unchanged path/to/file
*/
namespace SSHConnectCore.Configuration
{
    public class AppSettings
    {
        public SSHServer sshServer { get; set; }
        public API api { get; set; }
        public string killProcessList { get; set; }
        public string downloadDirectory { get; set; }
        public string linuxServerDirectory { get; set; }
        public string windowsServerDirectory { get; set; }
    }

    // Raspberry Pi
    public class SSHServer
    {
        public string host { get; set; }
        public string port { get; set; }
        public string username { get; set; }
        public string password { get; set; }
    }

    // Server hosting this
    public class API
    {
        public string host { get; set; }
        public string username { get; set; }
        public string password { get; set; }
    }
}
