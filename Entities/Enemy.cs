namespace NewWorldPP2.Entities
{
    // Stores the combat statistics and rewards for an enemy.
    public class Enemy
    {
        public string Name { get; set; } = string.Empty;
        public int MaxHealth { get; set; }
        public int Health { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public Stats Stats { get; set; } = new Stats();
        public int GoldReward { get; set; }
        public int ExperienceReward { get; set; }
        public Item? ItemReward { get; set; }
    }
}
