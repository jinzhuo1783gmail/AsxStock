using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.DataCenter.SocialMedia
{
    public class YoutubeSearchResponse
    {
        public string Kind { get; set; }
        public string Etag { get; set; }
        public string NextPageToken { get; set; }
        public string RegionCode { get; set; }
        public PageInfo PageInfo { get; set; }
        public List<Item> Items { get; set; }
    }

    public class PageInfo
    {
        public int TotalResults { get; set; }
        public int ResultsPerPage { get; set; }
    }

    public class Item
    {
        public string Kind { get; set; }
        public string Etag { get; set; }
        public Id Id { get; set; }
        public Snippet Snippet { get; set; }
    }

    public class Id
    {
        public string Kind { get; set; }
        public string VideoId { get; set; }
    }

    public class Snippet
    {
        public DateTime PublishedAt { get; set; }
        public string ChannelId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Thumbnails Thumbnails { get; set; }
        public string ChannelTitle { get; set; }
        public string LiveBroadcastContent { get; set; }
        public DateTime PublishTime { get; set; }
    }

    public class Thumbnails
    {
        public Thumbnail Default { get; set; }
        public Thumbnail Medium { get; set; }
        public Thumbnail High { get; set; }
    }

    public class Thumbnail
    {
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
