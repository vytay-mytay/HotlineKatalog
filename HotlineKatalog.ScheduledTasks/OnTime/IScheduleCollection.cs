using System;
using System.Collections.Generic;
using System.Text;

namespace HotlineKatalog.ScheduledTasks.OnTime
{
    public interface IScheduleCollection<T> where T: struct
    {
        bool Add(T entityId, DateTime date);

        void Remove(T entityId);
    }
}
