using System;
using System.Collections.Generic;
using System.Text;

namespace HabitTracker.Shared
{
    public class HabitCreateRequest // kad izveido prasa sos datus
    {
        public string UserID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Schedule {  get; set; }

    }
}
