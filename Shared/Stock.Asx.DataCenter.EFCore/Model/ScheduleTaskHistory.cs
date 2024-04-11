using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.Asx.DataCenter.EFCore.Model
{
    public class ScheduleTaskHistory
    {
        public long Id { get; set; }
        public string TaskName { get; set; }
        public DateTime ProcessDateTime { get; set; }
        public byte[] LogHistory { get; set; } = Array.Empty<byte>();
        public DateTime UploadDate { get; set; }
    }
}
