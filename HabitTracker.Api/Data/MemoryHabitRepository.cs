using HabitTracker.Domain;
using HabitTracker.Domain.Repositories;

namespace HabitTracker.Api.Data // tas kas paradas ar api timekli
{
    public class InMemoryHabitRepository : IHabitRepository
    {
        private readonly List<Habit> _habits = new List<Habit>();

        public InMemoryHabitRepository()
        {
        }

        public IEnumerable<Habit> GetAllHabits()
        {
            return _habits;
        }

        public Habit GetHabitById(Guid id)  // ieradumu dabu pec id
        {
            return _habits.FirstOrDefault(h => h.ID == id);
        }

        public void AddHabit(Habit habit)
        {
            _habits.Add(habit);
        }

        public void UpdateHabit(Habit habit)  // datu atjauninasana
        {
            var existingHabit = GetHabitById(habit.ID);
            if (existingHabit != null)
            {
                existingHabit.Name = habit.Name;
                existingHabit.Description = habit.Description;
                existingHabit.Schedule = habit.Schedule;
                existingHabit.Record = habit.Record;
                existingHabit.IsArchived = habit.IsArchived;
            }
        }

        public void DeleteOrArchiveHabit(Guid id)
        {
            var existingHabit = GetHabitById(id);
            if (existingHabit != null)
            {
                existingHabit.IsArchived = true;
            }
        }
    }
}