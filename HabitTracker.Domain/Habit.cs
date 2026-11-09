using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace HabitTracker.Domain
{
    public class Habit
    {// izveido datu logus 
        public Guid ID { get; set; }

        public string UserID { get; set; }

        private string _name; //atgriezama vertiba

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))  // lai nav tuksums
                {
                    throw new ArgumentException("Ieraduma nosaukums nedrikst but tukss!");
                }

                if (value.Length > 50)  // lai neparsniedz 50 rakstzimes
                {
                    throw new ArgumentException("Ieraduma nosaukums nedrikst parsniegt 50 simbolus!");
                }
                _name = value;
                UpdatedAt = DateTime.Now;
            }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && value.Length > 250) // lai neparsniedz 250 rakstzimes
                {
                    throw new ArgumentException("Apraksts nedrikst parsniegt 250 simbolus!");
                }

                _description = value;
                UpdatedAt = DateTime.Now;
            }
        }

        public bool IsArchived { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string Schedule { get; set; }



        public List<HabitRecord> Record { get; set; } = new List<HabitRecord>(); //glaba visus ieradumu izpildes ierakstus

        public bool IsCompletedToday()
        {
            return Record.Any(r => r.CompletionDate.Date == DateTime.Today.Date);
        }
        public bool IsCompleted => IsCompletedToday();
        public void MarkAsComplete(DateTime date)
        {
            if (IsArchived)
            {
                throw new Exception("Nevar atzimet ka pebeigtu! Ieradums ir arhivets!");
            }

            bool IsCompleted = false;
            foreach (var record in Record)
            {
                if (date.Date == record.CompletionDate.Date) { IsCompleted = true; break; } // lai nav dublikati
            }
            if (IsCompleted)
            {
                throw new Exception("Ieradums jau tika izpildits sodien!");

            }

            var newRecord = new HabitRecord  // izveido jaunu ierakstu
            {
                Id = Guid.NewGuid(),
                HabitID = this.ID,
                CompletionDate = date,
            };

            Record.Add(newRecord);

        }
    }
}
