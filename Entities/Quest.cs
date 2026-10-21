namespace NewWorldPP2.Entities
{
    public enum QuestStatus
    {
        Active,
        Completed,
        Failed
    }

    public class Quest
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int RequiredEncounters { get; set; } = 1;
        public int CompletedEncounters { get; set; }
        public int GoldReward { get; set; }
        public Item? ItemReward { get; set; }
        public QuestStatus Status { get; set; } = QuestStatus.Active;

        public void RecordProgress()
        {
            if (Status != QuestStatus.Active) return;

            CompletedEncounters++;
            if (CompletedEncounters >= RequiredEncounters)
            {
                Status = QuestStatus.Completed;
            }
        }
    }
}
