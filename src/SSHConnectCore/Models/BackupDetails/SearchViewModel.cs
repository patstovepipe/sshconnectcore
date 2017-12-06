using SSHConnectCore.Models.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSHConnectCore.Models.BackupDetails
{
    public class SearchViewModel
    {
        public List<BackupDetail> BackupDetails { get; set; }
        public MessageViewModel MessageViewModel { get; set; }
    }
}
