using Stock.Asx.DataCenter.EFCore.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.Asx.DataCenter.ShortHistories
{
    public class Schedules
    {
        private readonly List<ScheduleSetting> settings;
        public Schedules() 
        { 
            using (var context = new CompanyContext())
            {
                settings = context.ScheduleSettings.Where(a => a.IsActive).ToList();
            }
        }

        //public bool ShouldTrigger (string taskName, DateTime currentTime)
        //{
        //    using (var context = new CompanyContext())
        //    {
                
        //    }
        //}
    }
}
