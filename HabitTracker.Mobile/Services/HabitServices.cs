using HabitTracker.ConsoleApp;
using HabitTracker.Domain; // var pieklut projekta failiem
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace HabitTracker.Mobile.Services
{
    public class HabitService
    {
        private readonly string _filePath;

        public HabitService()
        {
            _filePath = Path.Combine(FileSystem.AppDataDirectory, "habits.json"); // faila cels
        }

        public List<Habit> LoadHabits()
        {
            if (!File.Exists(_filePath))
            {
                var initialHabits = TestDataGenerator.GenerateHabits(); // izveido testa datus
                SaveHabits(initialHabits);
                return initialHabits;
            }

            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<Habit>>(json) ?? new List<Habit>();
        }

        public void SaveHabits(List<Habit> habits) // saglaba datus faila
        {
            var json = JsonSerializer.Serialize(habits);
            File.WriteAllText(_filePath, json);
        }
    }
}