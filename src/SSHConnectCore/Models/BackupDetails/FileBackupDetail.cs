using System;

namespace SSHConnectCore.Models.BackupDetails
{
    public class FileBackupDetail : BackupDetail
    {
        public string FileName { get; set; }
        public string ActualFileName { get; set; }

        public override string Name { get { return this.FileName; } }
        public override string Type { get { return "file"; } }
    }
}
