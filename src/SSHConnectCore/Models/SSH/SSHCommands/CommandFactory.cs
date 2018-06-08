using System;

namespace SSHConnectCore.Models.SSH.SSHCommands
{
    public class CommandFactory
    {
        public static SSHCommand Get(string callerMemberName)
        {
            var className = $"SSHConnectCore.Models.SSH.SSHCommands.{callerMemberName}Command";
            Type t = Type.GetType(className);

            if (t != null && t.IsSubclassOf(typeof(SSHCommand)))
            {
                return (SSHCommand)Activator.CreateInstance(t);
            }

            throw new Exception("Could not create command object from caller member name");
        }
    }
}
