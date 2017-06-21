namespace SSHConnectCore.Models.BackupDetails
{
    public abstract class BackupDetail
    {
        public string BaseDirectory { get; set; }
        public abstract string Name { get; }
        public abstract string Type { get; }
    }
}