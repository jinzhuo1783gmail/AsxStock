

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Stock.Asx.DataCenter.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stock.DataCenter.Analysis.Instance;
using Stock.DataCenter.Analysis.Announcements;
using Microsoft.Extensions.Options;

var connectionString = "";

IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                // add DB
                connectionString = "Server=withouthammer.ddns.net,3627;Database=Stock.Asx.DataCenter;User Id=sa;Password=returnNull1;TrustServerCertificate=True";
                services.AddDbContext<CompanyContext>(op => op.UseSqlServer(connectionString, so => so.CommandTimeout(60)), ServiceLifetime.Transient, ServiceLifetime.Transient);
                connectionString = "Server=withouthammer.ddns.net,3628;Database=Stock.Asx.AnalysisCenter;User Id=sa;Password=Nbq4dcz123;TrustServerCertificate=True";
                services.AddDbContext<AnalysisContext>(op => op.UseSqlServer(connectionString, so => so.CommandTimeout(60)), ServiceLifetime.Transient, ServiceLifetime.Transient);

                // Add logging
                services.AddLogging(builder =>
                {
                    builder.AddConfiguration(config.GetSection("Logging"));
                    builder.AddConsole();
                });

                services.AddSingleton<AnalysisAnnouncements>();

            });




var build = host.Build();

var serviceProvider = build.Services;
var scope = serviceProvider.CreateScope();

await EntryRun.Run(scope);

