using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.Asx.DataCenter.EFCore.Model
{
    public class YoutubeVideoCollectionSetting
    {
        public long Id { get; set; } // Primary Key, handled by EF Core convention

        public string Catergory { get; set; } // Nullable by default

        public string SubCatergory { get; set; } // Not nullable

        public string SectorName { get; set; } // Not nullable

        public string Symbol { get; set; } // Not nullable

        public string SearchFilterKeywords { get; set; } // Not nullable

        public int DateFrom { get; set; } // Not nullable, maps to SQL int

        public int MaxNumberOfVideos { get; set; } // Not nullable, maps to SQL int

        public bool IsActive { get; set; } // Not nullable, maps to SQL bit
    }
}
