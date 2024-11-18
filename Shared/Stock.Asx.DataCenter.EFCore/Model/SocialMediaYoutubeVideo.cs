using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.Asx.DataCenter.EFCore.Model
{
    public class SocialMediaYoutubeVideo
    {
        public long Id { get; set; } // Primary Key, EF Core will treat it as such by convention for "Id"

        public string Catergory { get; set; } // Nullable by default

        public string SubCatergory { get; set; } // Not nullable

        public string SectorName { get; set; } // Not nullable

        public string Symbol { get; set; } // Not nullable

        public string Title { get; set; } // Not nullable

        public string Description { get; set; } = string.Empty;

        public string VideoId { get; set; } // Not nullable

        public string Subtitle { get; set; } = string.Empty;

        public string Enrich { get; set; } = string.Empty; 

        public string Sentiment { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime ReleaseDate { get; set; } // Not nullable

        public DateTime CreateDate { get; set; } // Not nullable
    }
}
