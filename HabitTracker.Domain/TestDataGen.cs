using HabitTracker.Domain;
using System.Collections.Generic;
using System;

namespace HabitTracker.ConsoleApp
{
    public static class TestDataGenerator
    {
        public static List<Habit> GenerateHabits()
        {
            var habits = new List<Habit>(); // saraksts kura glabas ieradumus

            //Datus uzgenereja gemini

            var habit1 = new Habit
            {
                ID = Guid.NewGuid(),
                UserID = "user1",
                Name = "Dzert udeni",
                Description = "2 litri diena",
                Schedule = "Katru dienu",
                IsArchived = false,
                CreatedAt = DateTime.Now.AddDays(-10)
            };
            habit1.MarkAsComplete(DateTime.Today);   // lai butu datu dazadiba

            var habit2 = new Habit
            {
                ID = Guid.NewGuid(),
                UserID = "user1",
                Name = "Macities spanu valodu",
                Description = "Duolingo",
                Schedule = "Katru dienu",
                IsArchived = true,
                CreatedAt = DateTime.Now.AddDays(-30)
            };

            var habit3 = new Habit
            {
                ID = Guid.NewGuid(),
                UserID = "user2",
                Name = "Skriet",
                Description = "5 km parka",
                Schedule = "3x nedela",
                IsArchived = false,
                CreatedAt = DateTime.Now.AddDays(-5)
            };
            var habit4 = new Habit
            {
                ID = Guid.NewGuid(),
                UserID = "user2",
                Name = "Lasit gramatu",
                Description = "Vismaz 20 lapaspuses pirms miega",
                Schedule = "Katru vakaru",
                IsArchived = false,
                CreatedAt = DateTime.Now.AddDays(-2)
            };

            habit4.MarkAsComplete(DateTime.Today.AddDays(-1));

            var habit5 = new Habit
            {
                ID = Guid.NewGuid(),
                UserID = "user3",
                Name = "Meditet",
                Description = "10 minutes rita klusuma",
                Schedule = "Darba dienas",
                IsArchived = true, 
                CreatedAt = DateTime.Now.AddDays(-60)
            };

            habits.Add(habit1); // pievieno ieradumu sarakstam
            habits.Add(habit2);
            habits.Add(habit3);
            habits.Add(habit4);
            habits.Add(habit5);

            return habits;
        }
    }
}