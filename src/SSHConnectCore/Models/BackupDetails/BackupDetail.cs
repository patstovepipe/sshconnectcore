using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SSHConnectCore.Models.BackupDetails
{
    public enum BackupDirectory { Other, Roms };
    public enum FileSystemType { File, Directory };

    public class BackupDetail
    {
        public string BaseDirectory { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public BackupDirectory? BackupDirectory { get; set; }
        public string Name { get;  set; }
        public string ActualName { get;  set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public FileSystemType FileSystemType { get; set; }
        public bool? BackedUp { get; set; }
    }
}