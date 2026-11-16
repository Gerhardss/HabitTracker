using HabitTracker.Domain;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace HabitTracker.Mobile.Services
{
    public class HabitService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public HabitService()
        {
            _httpClient = new HttpClient();
            string host = DeviceInfo.Platform == DevicePlatform.Android ? "10.0.2.2" : "localhost";
            _baseUrl = $"http://{host}:5294/api/habits";
        }
        public async Task<List<Habit>> GetHabitsAsync()
        {
            try
            {
                var habits = await _httpClient.GetFromJsonAsync<List<Habit>>(_baseUrl);
                return habits ?? new List<Habit>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"API kļūda ielādējot datus: {ex.Message}");
                return new List<Habit>();
            }
        }
        public async Task AddHabitAsync(Habit habit)
        {
            var response = await _httpClient.PostAsJsonAsync(_baseUrl, habit);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateHabitAsync(Habit habit)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/{habit.ID}", habit);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteOrArchiveHabitAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<Habit?> CompleteHabitAsync(Guid id)
        {
            var response = await _httpClient.PostAsync($"{_baseUrl}/{id}/complete", null);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Habit>();
        }
    }
}