using System.Net.Http.Json;
using HabitTracker.Domain;

namespace HabitTracker.Mobile
{
    public class ApiHabitService
    {
        private readonly HttpClient _httpClient;

        private readonly string _baseUrl = "http://localhost:5294/api/habits";

        public ApiHabitService()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(5); // 5s limits lai neuzkartos
        }

        public async Task<List<Habit>> GetHabitsAsync()     // dabu visus ieradumus
        {
            try
            {
                var habits = await _httpClient.GetFromJsonAsync<List<Habit>>(_baseUrl);
                return habits ?? new List<Habit>();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> AddHabitAsync(Habit habit) // pievieno 
        {
            var response = await _httpClient.PostAsJsonAsync(_baseUrl, habit);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateHabitAsync(Habit habit) // labo
        {
            var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/{habit.ID}", habit);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ArchiveHabitAsync(Guid id) // arhive
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CompleteHabitAsync(Guid id) // atzime ka izpilditu
        {
            var response = await _httpClient.PostAsync($"{_baseUrl}/{id}/complete", null);
            return response.IsSuccessStatusCode;
        }
    }
}