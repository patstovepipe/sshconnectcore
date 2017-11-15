namespace SSHConnectCore.Models.BackupDetails
{
    public class FileBackupDetail
    {
        public string FileName { get; set; }
        public string ActualFileName { get; set; }

        //public override string Name { set { this.FileName = value; } }
        //public override string ActualName {  set { this.ActualFileName = value; } }
        //public override string Type { get { return "file"; } }
    }
}
