namespace NewWorldPP2.Entities
{
    // Tracks the current state of a quest.
    public enum QuestStatus
    {
        Active,
        Completed,
        Failed
    }

    // Stores the requirements, rewards, and progress for a quest.
    public class Quest
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int RequiredEncounters { get; set; } = 1;
        public int CompletedEncounters { get; set; }
        public int GoldReward { get; set; }
        public int ExperienceReward { get; set; } = 50;
        public Item? ItemReward { get; set; }
        public QuestStatus Status { get; set; } = QuestStatus.Active;
        public bool RewardsClaimed { get; set; }

        // Advances the quest and marks it complete once its requirement is met.
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
