using System;
using System.Collections.Generic;
using System.Text;

namespace HotlineKatalog.ScheduledTasks.Schedule.Cron
{
    public delegate void CrontabFieldAccumulator(int start, int end, int interval);
}
