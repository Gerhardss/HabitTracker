// mi izmantoju gemini, pie koda fragmentiem ir pielikts, ja tas ir mi generets
// uzspiezot uz balta fona var atvert redigesanas un informacijas apskates sadalu
using HabitTracker.Mobile.Services;

namespace HabitTracker.Mobile
{
    public partial class MainPage : ContentPage
    {
        private readonly ApiHabitService _habitService; // nomainits uz ApiHabitService

        public MainPage()
        {
            InitializeComponent();
            _habitService = new ApiHabitService();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadHabitsAsync();
        }

        private async Task LoadHabitsAsync()
        {
            var allHabits = await _habitService.GetHabitsAsync();

            if (allHabits == null)
            {
                await DisplayAlert("Kļūda", "API nav sasniedzams no mobilās lietotnes.", "Labi");
                return;
            }

            var habits = allHabits.Where(h => !h.IsArchived).ToList(); // ielade nearhivetus ieradumus
            HabitsList.ItemsSource = null;
            HabitsList.ItemsSource = habits;

            int completedToday = habits.Count(h => h.IsCompleted);
            TodayProgressLabel.Text = $"Šodien izpildīti: {completedToday} no {habits.Count}";
        }

        private async void OnRefreshClicked(object sender, EventArgs e)
        {
            RefreshButton.IsEnabled = false;
            RefreshButton.Text = "Ielādē...";

            try
            {
                await LoadHabitsAsync();
            }
            finally
            {
                RefreshButton.Text = "↻ Atjaunot";
                RefreshButton.IsEnabled = true;
            }
        }

        private async void OnRefreshing(object sender, EventArgs e)
        {
            try
            {
                await LoadHabitsAsync();
            }
            finally
            {
                HabitsRefreshView.IsRefreshing = false;
            }
        }

        private async void OnCompleteButtonClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var habit = button?.CommandParameter as HabitTracker.Domain.Habit;

            if (habit != null)
            {
                // izveido sarakstu ja tukss
                if (habit.Record == null)
                {
                    habit.Record = new List<HabitTracker.Domain.HabitRecord>();
                }

                if (!habit.IsCompletedToday()) //parbauda vai nav izpildits sodien
                {
                    // (Lokālā datuma pievienošana vairs nav jāraksta, jo API to izdara savā galā)
                    bool success = await _habitService.CompleteHabitAsync(habit.ID);

                    if (success)
                    {
                        // saglaba (vairs nav jāsaglabā failā, tāpēc izsaucam tikai OnAppearing)

                        // no 48 lidz 60 rindinai ir gemini kods
                        // man bija await DisplayAlert bet visu laiku meta error
                        await LoadHabitsAsync();

                        await Application.Current.MainPage.DisplayAlert("Gatavs!", "Ieradums veiksmigi atzimets ka izpildits.", "Labi");
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Pazinojums", "Ieradums sodien jau ir izpildits", "Labi");
                }
            }
        }

        private async void OnAddNewClicked(object sender, EventArgs e) //jauna ieraduma pievienosana
        {
            await Navigation.PushModalAsync(new AddHabitPage());
        }

        private async void OnHabitSelected(object sender, SelectionChangedEventArgs e) // ieraduma apskate un redigesana
        {
            if (e.CurrentSelection.FirstOrDefault() is HabitTracker.Domain.Habit selectedHabit)
            {
                HabitsList.SelectedItem = null;

                await Navigation.PushModalAsync(new EditHabitPage(selectedHabit));
            }
        }

        private async void OnHabitTapped(object sender, TappedEventArgs e)
        { // gemini ieteica so izmantot, jo uz android nevareja atvert ieraduma apskati un redigesanas logu

            var element = sender as VisualElement;

            if (element?.BindingContext is HabitTracker.Domain.Habit selectedHabit)
            {
                await Navigation.PushModalAsync(new EditHabitPage(selectedHabit));
            }
        }
    }
}