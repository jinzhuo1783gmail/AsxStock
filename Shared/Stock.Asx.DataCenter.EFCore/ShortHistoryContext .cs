using Microsoft.EntityFrameworkCore;
using Stock.Asx.DataCenter.EFCore.Model;
using System.Collections.Generic;

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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=withouthammer.ddns.net,3627;Database=Stock.Asx.DataCenter;User Id=sa;Password=returnNull1;TrustServerCertificate=True");
    }
}
