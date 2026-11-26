// mobilas lietotnes darbosanas bez timekla servisa, neobligatais md1 
// veidots kopa ar gemini
// saglaba lokalaja kesatmina, kamer ir tiessaiste. Kad beidzas savienojums tad nem failus no kesatminas un ...
// pectam visas darbibas saglaba tur. Kad atkal savienojums ar API tad izsuta visas darbibas kuras bija bezsaiste

using System.Net.Http.Json;
using System.Text.Json;
using HabitTracker.Domain;

namespace HabitTracker.Mobile
{
    public class ApiHabitService
    {
        private readonly HttpClient _httpClient;

        private readonly string _baseUrl = "http://localhost:5294/api/habits";
        private static readonly string _cacheFilePath = Path.Combine(FileSystem.AppDataDirectory, "habits_cache.json");  //  kesatminas fails
        private static readonly string _queueFilePath = Path.Combine(FileSystem.AppDataDirectory, "pending_actions.json");  // offline fails

        private static readonly object _lock = new object();
        private static List<Habit>? _memoryCache;
        private static List<PendingHabitAction>? _memoryQueue;

        public bool IsOffline { get; private set; } // sis nosaka vai ir offlaina vai nav
        public int PendingActionsCount => LoadQueue().Count;

        public ApiHabitService()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(1); // 1s limits lai neuzkartos
        }

        public async Task<List<Habit>> GetHabitsAsync()     // dabu visus ieradumus
        {
            try
            {
                await SyncPendingActionsAsync();  //  aizsuta to kas bija offlaina uz api

                var habits = await _httpClient.GetFromJsonAsync<List<Habit>>(_baseUrl);
                var result = habits ?? new List<Habit>();

                SaveCache(result); // saglaba kesatmina
                IsOffline = false;
                return result;
            }
            catch (Exception)
            {
                IsOffline = true;
                return LoadCache().ToList(); // ja api nav pieejams tad atgriez kesatminas failu
            }
        }

        public async Task<bool> AddHabitAsync(Habit habit) // pievieno 
        {
            if (habit.ID == Guid.Empty) // pieskir lokalu id lai atpazist offline rezima
            {
                habit.ID = Guid.NewGuid();
            }

            try
            {
                var response = await _httpClient.PostAsJsonAsync(_baseUrl, habit);
                if (response.IsSuccessStatusCode)
                {
                    IsOffline = false;
                    return true;
                }
            }
            catch (Exception)
            {
            }

            IsOffline = true;
            var cached = LoadCache(); // saglaba lokali
            cached.Add(habit);
            SaveCache(cached);

            EnqueueAction(new PendingHabitAction  // sinhronizacijas rinda
            {
                ActionType = "ADD",
                HabitId = habit.ID,
                HabitData = habit
            });

            return true;
        }

        public async Task<bool> UpdateHabitAsync(Habit habit) // labo
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/{habit.ID}", habit);
                if (response.IsSuccessStatusCode)
                {
                    IsOffline = false;
                    return true;
                }
            }
            catch (Exception)
            {
            }

            IsOffline = true;
            var cached = LoadCache();  // offline rezima atjauno datus
            var index = cached.FindIndex(h => h.ID == habit.ID);
            if (index >= 0)
            {
                cached[index] = habit;
                SaveCache(cached);
            }

            EnqueueAction(new PendingHabitAction // sinhronizacijas rinda
            {
                ActionType = "UPDATE",
                HabitId = habit.ID,
                HabitData = habit
            });

            return true;
        }

        public async Task<bool> ArchiveHabitAsync(Guid id) // arhive
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/{id}");
                if (response.IsSuccessStatusCode)
                {
                    IsOffline = false;
                    return true;
                }
            }
            catch (Exception)
            {
            }

            IsOffline = true;
            var cached = LoadCache();
            var existing = cached.FirstOrDefault(h => h.ID == id); // arhivesana offlaina
            if (existing != null)
            {
                existing.IsArchived = true;
                SaveCache(cached);
            }

            EnqueueAction(new PendingHabitAction // sinhronizacijas rinda
            {
                ActionType = "ARCHIVE",
                HabitId = id
            });

            return true;
        }

        public async Task<bool> CompleteHabitAsync(Guid id) // atzime ka izpilditu
        {
            try
            {
                var response = await _httpClient.PostAsync($"{_baseUrl}/{id}/complete", null);
                if (response.IsSuccessStatusCode)
                {
                    IsOffline = false;
                    return true;
                }
            }
            catch (Exception)
            {
            }

            IsOffline = true;
            var cached = LoadCache();
            var existing = cached.FirstOrDefault(h => h.ID == id);  // pievieno sodienas izpildi lokali
            if (existing != null)
            {
                existing.Record ??= new List<HabitRecord>();
                if (!existing.IsCompletedToday())
                {
                    existing.Record.Add(new HabitRecord
                    {
                        Id = Guid.NewGuid(),
                        HabitID = id,
                        CompletionDate = DateTime.UtcNow
                    });
                }
                SaveCache(cached);
            }

            EnqueueAction(new PendingHabitAction // ieliek rinda
            {
                ActionType = "COMPLETE",
                HabitId = id
            });

            return true;
        }

        public async Task<int> SyncPendingActionsAsync()  // aizsuta no offline rezima datus uz api
        {                                                    // kad savienojums atjaunojas
            var queue = LoadQueue().ToList();
            if (queue.Count == 0)
            {
                return 0;
            }

            int syncedCount = 0;
            var remaining = new List<PendingHabitAction>();

            for (int i = 0; i < queue.Count; i++)
            {
                var action = queue[i];
                try
                {
                    HttpResponseMessage response = action.ActionType switch // darbibas veidu parbaude un pieprasijuma izveide
                    {
                        "ADD" when action.HabitData != null => await _httpClient.PostAsJsonAsync(_baseUrl, action.HabitData),
                        "UPDATE" when action.HabitData != null => await _httpClient.PutAsJsonAsync($"{_baseUrl}/{action.HabitId}", action.HabitData),
                        "ARCHIVE" => await _httpClient.DeleteAsync($"{_baseUrl}/{action.HabitId}"),
                        "COMPLETE" => await _httpClient.PostAsync($"{_baseUrl}/{action.HabitId}/complete", null),
                        _ => new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                    };

                    if (response.IsSuccessStatusCode)
                    {
                        syncedCount++;
                    }
                    else
                    {
                        remaining.Add(action);
                    }
                }
                catch (Exception)
                {
                    // saglaba gan neizsutito, gan visas parejas rindas darbibas
                    remaining.AddRange(queue.Skip(i));
                    SaveQueue(remaining);
                    throw;
                }
            }

            SaveQueue(remaining);
            return syncedCount;
        }

        private List<Habit> LoadCache() // nolasa lokalos ieradumus no json faila
        {
            lock (_lock)
            {
                if (_memoryCache != null)
                {
                    return _memoryCache;
                }

                try
                {
                    if (File.Exists(_cacheFilePath))
                    {
                        var json = File.ReadAllText(_cacheFilePath);
                        _memoryCache = JsonSerializer.Deserialize<List<Habit>>(json) ?? new List<Habit>();
                        return _memoryCache;
                    }
                }
                catch (Exception)
                {
                }

                _memoryCache = new List<Habit>();
                return _memoryCache;
            }
        }

        private void SaveCache(List<Habit> habits) // saglaba lokalaja json faila
        {
            lock (_lock)
            {
                _memoryCache = habits;
                try
                {
                    var json = JsonSerializer.Serialize(habits);
                    File.WriteAllText(_cacheFilePath, json);
                }
                catch (Exception)
                {
                }
            }
        }

        private List<PendingHabitAction> LoadQueue() // nolasa darbibas no faila pec kartas
        {
            lock (_lock)
            {
                if (_memoryQueue != null)
                {
                    return _memoryQueue;
                }

                try
                {
                    if (File.Exists(_queueFilePath))
                    {
                        var json = File.ReadAllText(_queueFilePath);
                        _memoryQueue = JsonSerializer.Deserialize<List<PendingHabitAction>>(json) ?? new List<PendingHabitAction>();
                        return _memoryQueue;
                    }
                }
                catch (Exception)
                {
                }

                _memoryQueue = new List<PendingHabitAction>();
                return _memoryQueue;
            }
        }

        private void SaveQueue(List<PendingHabitAction> queue) // saglaba offline darbibas faila
        {
            lock (_lock)
            {
                _memoryQueue = queue;
                try
                {
                    var json = JsonSerializer.Serialize(queue);
                    File.WriteAllText(_queueFilePath, json);
                }
                catch (Exception)
                {
                }
            }
        }

        private void EnqueueAction(PendingHabitAction action) // pievieno darbibu sinhronizacijas rindai
        {
            lock (_lock)
            {
                var queue = LoadQueue();
                queue.Add(action);
                SaveQueue(queue);
            }
        }

        private class PendingHabitAction // offline darbibas modelis
        {
            public string ActionType { get; set; } = string.Empty;
            public Guid HabitId { get; set; }
            public Habit? HabitData { get; set; }
        }
    }
}