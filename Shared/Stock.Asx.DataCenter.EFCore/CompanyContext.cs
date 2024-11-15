using Microsoft.EntityFrameworkCore;
using Stock.Asx.DataCenter.EFCore.Model;
using System.Collections.Generic;

namespace Stock.Asx.DataCenter.EFCore;
public class CompanyContext : DbContext
{
    public DbSet<ShortHistory> ShortHistories { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Price> Prices { get; set; }

    public DbSet<ScheduleSetting> ScheduleSettings { get; set; }

    public DbSet<ScheduleTaskHistory> ScheduleTaskHistories { get; set; }
    
    public DbSet<Announcement> Announcements { get; set; }
    
    public DbSet<SectorIndustryInvestment> SectorIndustryInvestmentFlowInOut { get; set; }

    public DbSet<IndustrySubCatInvestmentFlowInOut> IndustrySubCatInvestmentsFlowInOut { get; set; }

    public DbSet<SocialMediaYoutubeVideo> SocialMediaYoutubeVideos { get; set; }

    public DbSet<YoutubeVideoCollectionSetting> YoutubeVideoCollectionSettings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

        if (environment == "Development")
        {
            optionsBuilder.UseSqlServer("Server=withouthammer.ddns.net,3627;Database=Stock.Asx.DataCenter;User Id=sa;Password=returnNull1;TrustServerCertificate=True");
        }
        else
        {
            optionsBuilder.UseSqlServer("Server=withouthammer.ddns.net,3111;Database=Stock.Asx.DataCenter;User Id=sa;Password=returnNull1!;TrustServerCertificate=True");
        }

        
    }

    public CompanyContext() : base()
    {
    }
}
