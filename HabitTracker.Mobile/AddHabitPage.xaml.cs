using HabitTracker.Mobile.Services;

namespace HabitTracker.Mobile;

public partial class AddHabitPage : ContentPage
{
    private readonly ApiHabitService _apiService; // nomainīts uz ApiHabitService

    public AddHabitPage()
    {
        InitializeComponent();
        _apiService = new ApiHabitService();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false; //paslepj kludas pazinojumu pec katra meginajuma

        try
        {
            var newHabit = new HabitTracker.Domain.Habit // jauna ieraduma izveide
            {
                ID = Guid.NewGuid(),
                UserID = "user1",
                Name = NameEntry.Text ?? "",
                Description = DescriptionEntry.Text ?? "",
                Schedule = SchedulePicker.SelectedItem?.ToString() ?? "Katru dienu",
                CreatedAt = DateTime.Now,
                IsArchived = false
            };

            // ja viss katriba tad pievieno
            bool success = await _apiService.AddHabitAsync(newHabit);

            if (success)
            {
                await Navigation.PopModalAsync();
            }
            else
            {
                ErrorLabel.Text = "Neizdevās sazināties ar API serveri.";
                ErrorLabel.IsVisible = true;
            }
        }
        catch (Exception ex) // ja kluda ir nokerta
        {
            ErrorLabel.Text = ex.Message;
            ErrorLabel.IsVisible = true;
        }
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}