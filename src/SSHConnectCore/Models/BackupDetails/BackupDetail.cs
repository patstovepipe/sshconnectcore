using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.ComponentModel.DataAnnotations;

namespace SSHConnectCore.Models.BackupDetails
{
    public enum BackupDirectory { Other, Roms };
    public enum FileSystemType { File, Directory };

    [Serializable]
    public class BackupDetail
    {
        public Guid ID { get; set; }
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