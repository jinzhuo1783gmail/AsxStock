using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.Asx.DataCenter.EFCore.Model
{

        public class Price
        {
            public long Id { get; set; }
            public string Symbol { get; set; }
            public DateTime Date { get; set; }
            public double? Open { get; set; }
            public double? High { get; set; }
            public double? Low { get; set; }
            public double? Close { get; set; }
            public double? Volumn { get; set; }
            public double? CloseAdj { get; set; }
            public DateTime UploadDate { get; set; } = DateTime.Now;
        }

}
