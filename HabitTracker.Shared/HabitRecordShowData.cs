using System;
using System.Collections.Generic;
using System.Text;

namespace HabitTracker.Shared
{
    public class HabitRecordShowData  // arhiveto datu informacija
    {
        public Guid Id { get; set; }
        public Guid HabitId { get; set; }
        public DateTime CompletionDate { get; set; }
    }
}
