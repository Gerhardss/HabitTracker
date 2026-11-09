using HabitTracker.Mobile.Services;

namespace HabitTracker.Mobile;

public partial class EditHabitPage : ContentPage
{
    private readonly ApiHabitService _apiService; // Nomainīts uz ApiHabitService
    private readonly HabitTracker.Domain.Habit _currentHabit;

    public EditHabitPage(HabitTracker.Domain.Habit habit)
    {
        InitializeComponent(); // padod ieradumu
        _apiService = new ApiHabitService();
        _currentHabit = habit;

        NameEntry.Text = _currentHabit.Name; // info par ieradumu
        DescriptionEntry.Text = _currentHabit.Description;
        SchedulePicker.SelectedItem = _currentHabit.Schedule;

        DateLabel.Text = $"Izveidots: {_currentHabit.CreatedAt.ToString("dd.MM.yyyy")}";
        int completionCount = _currentHabit.Record != null ? _currentHabit.Record.Count : 0;
        StatsLabel.Text = $"Kopa izpildits: {completionCount} reizes";
    }

    private async void OnSaveClicked(object sender, EventArgs e) // datu mainisana
    {
        ErrorLabel.IsVisible = false;
        try
        {
            // Atjaunojam datus objektā tieši no ievades laukiem
            _currentHabit.Name = NameEntry.Text ?? "";
            _currentHabit.Description = DescriptionEntry.Text ?? "";
            _currentHabit.Schedule = SchedulePicker.SelectedItem?.ToString() ?? "Katru dienu";

            // Sūtām atjaunināto ieradumu uz serveri
            bool success = await _apiService.UpdateHabitAsync(_currentHabit);

            if (success)
            {
                await Navigation.PopModalAsync();
            }
            else
            {
                ErrorLabel.Text = "Neizdevās saglabāt izmaiņas serverī.";
                ErrorLabel.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            ErrorLabel.Text = ex.Message;
            ErrorLabel.IsVisible = true;
        }
    }

    private async void OnArchiveClicked(object sender, EventArgs e)  // arhivesana
    {
        var confirm = await DisplayAlert("Uzmanibu", "Vai velies arhivet so ieradumu?", "Ja", "Ne");
        if (confirm)
        {
            // Sūtām arhivēšanas pieprasījumu uz API, izmantojot ID
            bool success = await _apiService.ArchiveHabitAsync(_currentHabit.ID);

            if (success)
            {
                await Navigation.PopModalAsync();
            }
            else
            {
                await DisplayAlert("Kļūda", "Neizdevās arhivēt ieradumu serverī.", "Labi");
            }
        }
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}