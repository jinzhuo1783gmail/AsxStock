using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.Asx.DataCenter.EFCore.Model
{
    public class SubCategorySummaryQuestion
    {
        public long Id { get; set; }
        public string SubCategory { get; set; }
        public string Question { get; set; }

        public int Sequence { get; set; }
        public bool IsActive { get; set; }
        public DateTime UploadDate { get; set; }
    }
}
