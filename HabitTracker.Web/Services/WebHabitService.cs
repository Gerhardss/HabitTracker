using System.Net.Http.Json;
using HabitTracker.Domain;

namespace HabitTracker.Web.Services
{
    public class WebHabitService
    {
        private readonly HttpClient _httpClient;

        public WebHabitService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Habit>> GetAllHabitsAsync() // no api dabu ieradumus
        {
            var result = await _httpClient.GetFromJsonAsync<List<Habit>>("api/habits");
            return result ?? new List<Habit>();
        }

        public async Task<Habit?> GetHabitByIdAsync(Guid id) // dabu info par vienu ieradumu
        {
            return await _httpClient.GetFromJsonAsync<Habit>($"api/habits/{id}");
        }

        public async Task AddHabitAsync(Habit habit) // jauna ieraduma izveidosana
        {
            var response = await _httpClient.PostAsJsonAsync("api/habits", habit);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateHabitAsync(Habit habit) // labosana
        {
            var response = await _httpClient.PutAsJsonAsync($"api/habits/{habit.ID}", habit);
            response.EnsureSuccessStatusCode();
        }

        public async Task ArchiveHabitAsync(Guid id) // arhivesana
        {
            var response = await _httpClient.DeleteAsync($"api/habits/{id}");
            response.EnsureSuccessStatusCode();
        }
        public async Task CompleteHabitAsync(Guid id) // izpildits sodien
        {
            var response = await _httpClient.PostAsync($"api/habits/{id}/complete", null);
            response.EnsureSuccessStatusCode();
        }
    }
}