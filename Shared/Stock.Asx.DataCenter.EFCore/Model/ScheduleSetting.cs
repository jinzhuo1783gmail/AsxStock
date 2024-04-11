using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.Asx.DataCenter.EFCore.Model
{
    public class ScheduleSetting
    {
        public long Id { get; set; }
        public string TaskName { get; set; }
        public TimeSpan ScheduleTime { get; set; }
        public bool IsActive { get; set; }
        public DateTime UploadDate { get; set; }
    }
}
