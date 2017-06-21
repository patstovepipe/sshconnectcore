using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SSHConnectCore.Models.BackupDetails
{
    public class BackupDetail
    {
        public enum FileSystemType { File, Directory };

        public string BaseDirectory { get; set; }
        public string BackupFolder { get; set; }
        public string Name { get;  set; }
        public string ActualName { get;  set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public FileSystemType Type { get; set; }
    }
}