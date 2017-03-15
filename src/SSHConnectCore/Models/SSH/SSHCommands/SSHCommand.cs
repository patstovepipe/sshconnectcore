using Renci.SshNet;

namespace SSHConnectCore.Models.SSH.SSHCommands
{
    abstract public class SSHCommand : Command
    {
        public string downloadDirectory { get; set; }

        public object Run(object[] args = null)
        {
            object result;
            using (SshClient client = new SshClient(server.host, server.port, server.username, server.password))
            {
                client.Connect();
                result = RunDetails(client, args);
                client.Disconnect();
            }
            return result;
        }

        abstract public object RunDetails(SshClient client, object[] args = null);
    }
}
