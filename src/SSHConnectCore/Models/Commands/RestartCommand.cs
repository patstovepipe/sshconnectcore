using Renci.SshNet;

namespace SSHConnectCore.Models.Commands
{
    public class RestartCommand : Command
    {
        public override SshCommand RunDetails(SshClient client, string[] args = null)
        {
            return client.RunCommand($"echo {server.password} | sudo -S shutdown -r now");
        }
    }
}
