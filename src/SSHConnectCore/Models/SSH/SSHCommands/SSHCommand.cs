using Renci.SshNet;

namespace SSHConnectCore.Models.Commands
{
    abstract public class SSHCommand : Command
    {
        public SshCommand Run(string[] args = null)
        {
            SshCommand result;
            using (SshClient client = new SshClient(server.host, server.port, server.username, server.password))
            {
                client.Connect();
                result = RunDetails(client, args);
                client.Disconnect();
            }
            return result;
        }

        abstract public SshCommand RunDetails(SshClient client, string[] args = null);
    }
}
