using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;

namespace SSHConnectCore.Models.BackupDetails
{
    public enum BackupDirectory { Other, Roms };
    public enum FileSystemType { File, Directory };

    public class BackupDetail
    {
        [Required]
        [Display(Name = "Base Directory")]
        public string BaseDirectory { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public BackupDirectory? BackupDirectory { get; set; }
        public string SavedName { get;  set; }
        [Required]
        [Display(Name = "Name")]
        public string ActualName { get;  set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public FileSystemType FileSystemType { get; set; }
        public bool? BackedUp { get; set; }
    }
}