using System;
using Xunit;
using SSHConnectCore.Models.BackupDetails;
using System.Collections.Generic;
using System.Linq;
using SSHConnectCore.Extensions;

namespace SSHConnectCore.Tests
{
    public class BackupDetailsUnitTest
    {
        private static readonly Lazy<List<BackupDetail>> lazyBackupDetailsTestList = new Lazy<List<BackupDetail>>(() => BackupDetailsTestList());
        private static List<BackupDetail> backupDetailsTestList => lazyBackupDetailsTestList.Value.ToList();

        [Fact]
        public void ExcludeTest()
        {
            var testList = backupDetailsTestList;
            var firstGuid = testList.First().ID;
            var count = testList.Count();
            testList = testList.Exclude(firstGuid);

            Assert.Equal(count - 1, testList.Count());
        }

        [Fact]
        public void GetTest()
        {
            var testList = backupDetailsTestList;

            var first = new BackupDetail();
            first = testList.First().DeepCopy();

            var gotObject = testList.Get(first.ID);

            Assert.Equal(first, gotObject);
        }

        [Fact]
        public void SaveTest()
        {
            var testList = backupDetailsTestList;
            var fileName = "backup_details_test.json";

            testList.Save(fileName);

            var savedList = BackupDetails.StoredBackupDetails(fileName);

            Assert.Equal(testList, savedList);
        }

        private static List<BackupDetail> BackupDetailsTestList()
        {
            var backupDetails = new List<BackupDetail>();

            for (int i = 0; i < 10; i++)
            {
                var backupDetail = new BackupDetail();
                backupDetail.ActualName = $"test{i}";
                backupDetail.BackupDirectory = i % 2 == 0 ? BackupDirectory.Other : BackupDirectory.Roms;
                backupDetail.BaseDirectory = $"test/dir{i}";
                backupDetail.FileSystemType = i % 2 != 0 ? FileSystemType.Directory : FileSystemType.File;
                backupDetail.ID = Guid.NewGuid();
                backupDetail.SavedName = $"test{i}";

                backupDetails.Add(backupDetail);
            }

            return backupDetails;
        }
    }
}