namespace Stock.Asx.DataCenter.EFCore.Model
{
    public class Announcement
    {
        public long Id { get; set; }
        public long AsxDocumentId { get; set; }
        public string Symbol { get; set; }
        public string FileURL { get; set; }
        public byte[] FileContent { get; set; } = Array.Empty<byte>();
        public string FileText { get; set; }
        public string FileSummary1 { get; set; }
        public string FileSummary2 { get; set; }
        public string FileSummary3 { get; set; }
        public DateTime ReleaseDate { get; set; }
        public DateTime UploadDate { get; set; }
    }
}
