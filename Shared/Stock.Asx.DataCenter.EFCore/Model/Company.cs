using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Stock.Asx.DataCenter.EFCore.Model
{
    public class Company
    {
        public long Id { get; set; }
        public string Symbol { get; set; }
        public string? CompanyName { get; set; }

        public string? Catergory { get; set; }

        public string? SubCatergory { get; set; }

        public string? SectorName { get; set; }
        public bool Analysis { get; set; }
        public long TotalShares { get; set; } = default;
        public double LastPrice { get; set; } = default;

        public DateTime UploadDate { get; set; }
    }
}
