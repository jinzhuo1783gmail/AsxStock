using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.Asx.DataCenter.ShortHistories.AsxCompanyGroup
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class AsxCompany
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("name_full")]
        public string NameFull { get; set; }

        [JsonProperty("name_short")]
        public string NameShort { get; set; }

        [JsonProperty("name_abbrev")]
        public string NameAbbrev { get; set; }

        [JsonProperty("principal_activities")]
        public string PrincipalActivities { get; set; }

        [JsonProperty("industry_group_name")]
        public string IndustryGroupName { get; set; }

        [JsonProperty("sector_name")]
        public string SectorName { get; set; }

        [JsonProperty("listing_date")]
        public DateTime? ListingDate { get; set; }

        [JsonProperty("delisting_date")]
        public DateTime? DelistingDate { get; set; }

        [JsonProperty("web_address")]
        public string WebAddress { get; set; }

        [JsonProperty("mailing_address")]
        public string MailingAddress { get; set; }

        [JsonProperty("phone_number")]
        public string PhoneNumber { get; set; }

        [JsonProperty("fax_number")]
        public string FaxNumber { get; set; }

        [JsonProperty("registry_name")]
        public string RegistryName { get; set; }

        [JsonProperty("registry_address")]
        public string RegistryAddress { get; set; }

        [JsonProperty("registry_phone_number")]
        public string RegistryPhoneNumber { get; set; }

        [JsonProperty("foreign_exempt")]
        public bool ForeignExempt { get; set; }

        [JsonProperty("fiscal_year_end")]
        public string FiscalYearEnd { get; set; }

        [JsonProperty("primary_share_code")]
        public string PrimaryShareCode { get; set; }

        [JsonProperty("recent_announcement")]
        public bool RecentAnnouncement { get; set; }

        [JsonProperty("products")]
        public List<string> Products { get; set; }

        [JsonProperty("latest_annual_reports")]
        public List<AnnualReport> LatestAnnualReports { get; set; }

        [JsonProperty("primary_share")]
        public PrimaryShare PrimaryShare { get; set; }
    }

    public class AnnualReport
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("document_release_date")]
        public DateTime DocumentReleaseDate { get; set; }

        [JsonProperty("document_date")]
        public DateTime DocumentDate { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("relative_url")]
        public string RelativeUrl { get; set; }

        [JsonProperty("header")]
        public string Header { get; set; }

        [JsonProperty("market_sensitive")]
        public bool MarketSensitive { get; set; }

        [JsonProperty("number_of_pages")]
        public int NumberOfPages { get; set; }

        [JsonProperty("size")]
        public string Size { get; set; }

        [JsonProperty("legacy_announcement")]
        public bool LegacyAnnouncement { get; set; }

        [JsonProperty("issuer_code")]
        public string IssuerCode { get; set; }

        [JsonProperty("issuer_short_name")]
        public string IssuerShortName { get; set; }

        [JsonProperty("issuer_full_name")]
        public string IssuerFullName { get; set; }
    }

    public class PrimaryShare
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("isin_code")]
        public string IsinCode { get; set; }

        [JsonProperty("desc_full")]
        public string DescFull { get; set; }

        [JsonProperty("last_price")]
        public double LastPrice { get; set; }

        [JsonProperty("open_price")]
        public double OpenPrice { get; set; }

        [JsonProperty("day_high_price")]
        public double DayHighPrice { get; set; }

        [JsonProperty("day_low_price")]
        public double DayLowPrice { get; set; }

        [JsonProperty("change_price")]
        public double ChangePrice { get; set; }

        [JsonProperty("change_in_percent")]
        public string ChangeInPercent { get; set; }

        [JsonProperty("volume")]
        public long Volume { get; set; }

        [JsonProperty("bid_price")]
        public double BidPrice { get; set; }

        [JsonProperty("offer_price")]
        public double OfferPrice { get; set; }

        [JsonProperty("previous_close_price")]
        public double PreviousClosePrice { get; set; }

        [JsonProperty("previous_day_percentage_change")]
        public string PreviousDayPercentageChange { get; set; }

        [JsonProperty("year_high_price")]
        public double YearHighPrice { get; set; }

        [JsonProperty("number_of_shares")]
        public long NumberOfShares { get; set; }
    }
}