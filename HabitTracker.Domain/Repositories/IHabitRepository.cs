using System;
using System.Collections.Generic;

namespace HabitTracker.Domain.Repositories
{
    public interface IHabitRepository
    {
        IEnumerable<Habit> GetAllHabits();
        Habit GetHabitById(Guid id);
        void AddHabit(Habit habit);
        void UpdateHabit(Habit habit);
        void DeleteOrArchiveHabit(Guid id);
    }
}