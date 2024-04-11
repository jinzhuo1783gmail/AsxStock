public class ShortHistory
{
    public long Id { get; set; }
    public string Symbol { get; set; }
    public string CompanyName { get; set; }
    public string ProductClass { get; set; }
    public long? TotalIssued { get; set; }
    public long? TotalShort { get; set; }
    public decimal Percentage { get; set; }
    public DateTime ShortDate { get; set; }
    public DateTime UploadDate { get; set; }
}
