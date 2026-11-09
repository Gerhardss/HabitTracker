using System;
using System.Collections.Generic;
using System.Text;

namespace HabitTracker.Shared
{
    public class HabitUpdateRequest  // ja atjaunina datus ieradumam
    {
        public string Name {  get; set; }
        public string Description { get; set; }
        public string Schedule {  get; set; }
        public bool IsArchived { get; set; }
    }
}
