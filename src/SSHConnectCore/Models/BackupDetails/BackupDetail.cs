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
        public string SavedName { get; set; }
        [Required]
        [Display(Name = "Name")]
        public string ActualName { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public FileSystemType FileSystemType { get; set; }
        public bool BackedUp { get; set; }
        public bool ExistsOnRemote { get; set; }
        public DateTime? RemoteLastCheck { get; set; }
        public string MD5CheckSum { get; set; }
        public string RemoteMD5CheckSum { get; set; }

        [JsonIgnore]
        public bool BackupDetailsCreated => !string.IsNullOrWhiteSpace(this.BaseDirectory);
        [JsonIgnore]
        public bool IsFile => this.FileSystemType == FileSystemType.File;
        [JsonIgnore]
        public bool HasCheckSums => !string.IsNullOrWhiteSpace(this.MD5CheckSum) && !string.IsNullOrWhiteSpace(this.RemoteMD5CheckSum);
        [JsonIgnore]
        public bool? SameCheckSum => HasCheckSums ? this.MD5CheckSum.Equals(this.RemoteMD5CheckSum) : (bool?)null;

        public override bool Equals(object other)
        {
            var toCompareWith = other as BackupDetail;
            if (toCompareWith == null)
                return false;
            return this.ID == toCompareWith.ID
                && this.BaseDirectory == toCompareWith.BaseDirectory
                && this.BackupDirectory == toCompareWith.BackupDirectory
                && this.SavedName == toCompareWith.SavedName
                && this.ActualName == toCompareWith.ActualName
                && this.FileSystemType == toCompareWith.FileSystemType;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}