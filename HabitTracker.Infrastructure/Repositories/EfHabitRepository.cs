using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using HabitTracker.Domain;
using HabitTracker.Domain.Repositories;

namespace HabitTracker.Infrastructure.Repositories
{
    public class EfHabitRepository : IHabitRepository
    {
        private readonly AppDbContext _context;

        public EfHabitRepository(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Habit> GetAllHabits() // ielade visus ierakstus
        {
            return _context.Habits
                .Include(h => h.Record)
                .ToList();
        }

        public Habit? GetHabitById(Guid id) // jautajuma zime lai null butu deriga atbilde
        {
            return _context.Habits
                .Include(h => h.Record)
                .FirstOrDefault(h => h.ID == id);
        }

        public void AddHabit(Habit habit)
        {
            _context.Habits.Add(habit);
            _context.SaveChanges();
        }

        public void UpdateHabit(Habit habit)
        {
            var existingDates = new HashSet<DateTime>(); 
            foreach (var rec in habit.Record.ToList())
            {
                var dateOnly = rec.CompletionDate.Date;
                if (!existingDates.Add(dateOnly))
                {
                    habit.Record.Remove(rec); // nonem dublikatu ja ir tads
                }
                else
                {
                    rec.CompletionDate = dateOnly;
                }
            }

            _context.Habits.Update(habit);
            _context.SaveChanges();
        }

        public void DeleteOrArchiveHabit(Guid id)
        {
            var habit = _context.Habits.FirstOrDefault(h => h.ID == id);
            if (habit != null)
            {
                habit.IsArchived = true;
                habit.UpdatedAt = DateTime.Now;
                _context.SaveChanges();
            }
        }
    }
}