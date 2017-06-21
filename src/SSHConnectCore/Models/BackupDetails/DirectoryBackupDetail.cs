using System;

namespace SSHConnectCore.Models.BackupDetails
{
    public class DirectoryBackupDetail : BackupDetail
    {
        public string Directory { get; set; }
        public string ActualDirectory { get; set; }

        public override string Name { get { return this.Directory; } }
        public override string Type { get { return "directory"; } }
    }
}
