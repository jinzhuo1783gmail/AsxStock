using Microsoft.Extensions.Logging;
using Stock.Asx.DataCenter.EFCore.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.DataCenter.Insert.Manual.Schedule
{
    public class ScheduleManager
    {

        private ILogger _logger;
        private List<ScheduleSetting> _scheduleSettings;
        public ScheduleManager(ILogger logger)
        {
            _logger = logger;

            using (var context = new CompanyContext())
            {
                _scheduleSettings = context.ScheduleSettings.ToList();
            }

        }

        public bool CanRunTask (string taskName)
        {
            DateTime today = DateTime.Today;
            DayOfWeek dayOfWeek = today.DayOfWeek;
        
            // Check if today is a weekend
            bool isWeekend = (dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday);

            if (isWeekend)
                return false;

            using (var context = new CompanyContext())
            {
                var taskScheduleSettings = _scheduleSettings.Where(s => s.TaskName == taskName).OrderBy(s => s.ScheduleTime);
                var taskHistory = context.ScheduleTaskHistories.Where(s => s.TaskName == taskName && s.ProcessDateTime >= DateTime.Now.Date).ToList();

                var validSchedule = taskScheduleSettings.Where(t => DateTime.Now >= DateTime.Now.Date.Add(t.ScheduleTime)).LastOrDefault();

                if  (validSchedule != null)
                {
                    if (!taskHistory.Any() || taskHistory.All(t => t.ProcessDateTime < DateTime.Now.Date.Add(validSchedule.ScheduleTime)))
                        return true;

                    return false;
                }

                return false;

            }
        }

        public bool AddTaskHistory (string taskName)
        {
            using (var context = new CompanyContext())
            {
                var history = new ScheduleTaskHistory() 
                { 
                    TaskName = taskName,
                    ProcessDateTime = DateTime.Now,
                    //LogHistory = default!,
                    UploadDate = DateTime.Now,
                };
                
                context.ScheduleTaskHistories.Add(history);
                context.SaveChanges();
            }

            return true;
        }

    }
}
