using Microsoft.Extensions.DependencyInjection;
using Stock.DataCenter.Analysis.Announcements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.DataCenter.Analysis.Instance
{
    public static class EntryRun
    {
        public static async Task Run(IServiceScope scope) 
        {
            var analysisAnnouncements = scope.ServiceProvider.GetService<AnalysisAnnouncements>();

            // await analysisAnnouncements.SyncAnnouncements();
            await analysisAnnouncements.ConvertAnnouncementsIntoText();

            
        
        }
    }
}
