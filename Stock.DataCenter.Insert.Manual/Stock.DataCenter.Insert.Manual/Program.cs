using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;
using Stock.Asx.DataCenter.ShortHistories;
using Stock.DataCenter.Announcements;
using Stock.DataCenter.Prices;
using Stock.DataCenter.Insert.Manual.Schedule;



using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .AddFilter("Microsoft", LogLevel.Warning)
        .AddFilter("System", LogLevel.Warning)
        .AddFilter("YourNamespace", LogLevel.Debug)
        .AddConsole();
});

ILogger logger = loggerFactory.CreateLogger<Program>();


var scheduleManager = new ScheduleManager(logger);

logger.LogInformation($"load stock start.... {DateTime.Now.ToString()}");

while (true)
{
    bool modified = false;  
    
    if (scheduleManager.CanRunTask("CompanyInformation"))
    {
        ShortHistories.InsertOrUpdateCompanyFromAsxApi(logger);
        scheduleManager.AddTaskHistory("CompanyInformation");
        modified = true;
    }

    if (scheduleManager.CanRunTask("ShortList"))
    {
        ShortHistories.DownloadAndInsertShort(logger);
        scheduleManager.AddTaskHistory("ShortList");
        modified = true;
    }


    if (scheduleManager.CanRunTask("CheckCompany"))
    {
        ShortHistories.CheckAndAppendCompany(logger);
        scheduleManager.AddTaskHistory("CheckCompany");
        modified = true;
    }

    if (scheduleManager.CanRunTask("PatchCompany"))
    {
        ShortHistories.CheckAndAmendExistingCompanyInformation(logger);
        scheduleManager.AddTaskHistory("PatchCompany");
        modified = true;
    }


    if (scheduleManager.CanRunTask("Annoucement"))
    {
        Announcements.DownloadAndInsertAnnoucnements(logger);
        scheduleManager.AddTaskHistory("Annoucement");
        modified = true;
    }


    if (scheduleManager.CanRunTask("GetQuote"))
    {
        PriceQuote.GetQuote(logger);
        PriceQuote.CalculateInvestmentMovement(logger);
        scheduleManager.AddTaskHistory("GetQuote");
        modified = true;
    }

    if (scheduleManager.CanRunTask("HistoricalPrice"))
    {
        HistoricalPrice.GetHistoricalPrices(logger);
        scheduleManager.AddTaskHistory("HistoricalPrice");
        modified = true;
    }

    if (modified)
    {
        logger.LogInformation($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Finalize processing...");
    }
}





    



