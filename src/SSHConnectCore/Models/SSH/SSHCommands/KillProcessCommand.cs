using Renci.SshNet;

namespace SSHConnectCore.Models.SSH.SSHCommands
{
    public class KillProcessCommand : SSHCommand
    {
        public override object RunDetails(SshClient client, object[] args)
        {
            return client.RunCommand("pkill " + args[0]);
        }
    }
}
