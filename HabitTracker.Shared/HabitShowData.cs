namespace HabitTracker.Shared
{
    public class HabitShowData // visi dati kuri pieder ieradumam
    {
        public Guid ID { get; set; }

        public string UserID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsArchived { get; set; }

        public string Schedule { get; set; }

    }
}
