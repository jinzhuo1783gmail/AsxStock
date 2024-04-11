using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Stock.Asx.DataCenter.EFCore.Model
{
    public class SectorIndustryInvestment
    {
        public long Id { get; set; }
        public string Catergory { get; set; }
        public string SubCatergory { get; set; }
        public string SectorName { get; set; }
        public double InvestmentInOut { get; set; }
        public DateTime UploadDate { get; set; }
        
    }
}
