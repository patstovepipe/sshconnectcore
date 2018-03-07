using System;
using Xunit;
using SSHConnectCore.Models.BackupDetails;
using System.Collections.Generic;
using System.Linq;

namespace SSHConnectCore.Tests
{
    public class BackupDetailsUnitTest
    {
        [Fact]
        public void PassingTest()
        {
            var testList = BackupDetailsTestList();
            var firstGuid = testList.First().ID;
            var count = testList.Count();
            testList = testList.Exclude(firstGuid);

            Assert.Equal(count - 1, testList.Count());
        }

        [Fact]
        public void FailingTest()
        {
            var testList = BackupDetailsTestList();
            var firstGuid = testList.First().ID;
            var count = testList.Count();
            testList = testList.Exclude(firstGuid);

            Assert.Equal(count, testList.Count());
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