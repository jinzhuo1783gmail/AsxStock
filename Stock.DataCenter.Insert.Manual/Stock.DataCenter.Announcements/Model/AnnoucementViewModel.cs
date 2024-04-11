using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class AnnoucementViewModel
{
    [JsonProperty("id")]
    public long Id { get; set; }

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

public class RootViewModel
{
    [JsonProperty("data")]
    public List<AnnoucementViewModel> Data { get; set; }

    [JsonProperty("paging_next_url")]
    public string PagingNextUrl { get; set; }
}