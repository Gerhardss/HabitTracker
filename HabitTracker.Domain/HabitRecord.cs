using System;
using System.Collections.Generic;
using System.Text;

namespace HabitTracker.Domain
{
    public class HabitRecord   //ja ieradums izpildits
    {
        public Guid Id { get; set; }

        public Guid HabitID { get; set; }

        public DateTime CompletionDate { get; set; }


    }
}
