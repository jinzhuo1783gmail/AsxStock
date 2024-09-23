using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stock.Asx.DataCenter.EFCore.Model;
using System.Collections.Generic;


namespace Stock.Asx.DataCenter.EFCore;
public class AnalysisContext : DbContext
{

    public DbSet<ScheduleSetting> ScheduleSettings { get; set; }

    public DbSet<ScheduleTaskHistory> ScheduleTaskHistories { get; set; }
    
    public DbSet<Announcement> Announcements { get; set; }

    public DbSet<SubCategorySummaryQuestion> SubCategorySummaryQuestions { get; set; }

    public AnalysisContext(DbContextOptions<AnalysisContext> options) : base(options)
    {
    }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=withouthammer.ddns.net,3628;Database=Stock.Asx.AnalysisCenter;User Id=sa;Password=Nbq4dcz123;TrustServerCertificate=True");
        optionsBuilder.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddFilter((category, level) => level == LogLevel.None)));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Announcement>()
            .Property(p => p.Id)
            ;
            //.ValueGeneratedNever();  // This tells EF not to expect SQL Server to generate the value
    }
}
