using Renci.SshNet;

namespace SSHConnectCore.Models.SSH.SSHCommands
{
    public class RestartCommand : SSHCommand
    {
        public override object RunDetails(SshClient client, object[] args = null)
        {
            return client.RunCommand($"echo {server.password} | sudo -S shutdown -r now");
        }
    }
}
