using Renci.SshNet;

namespace SSHConnectCore.Models.Commands
{
    public class KillProcessCommand : Command
    {
        public override SshCommand RunDetails(SshClient client, string[] args)
        {
            return client.RunCommand("pkill " + args[0]);
        }
    }
}
