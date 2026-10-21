namespace NewWorldPP2.Entities
{
    public class Enemy
    {
        public string Name { get; set; } = string.Empty;
        public int MaxHealth { get; set; }
        public int Health { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int GoldReward { get; set; }
        public Item? ItemReward { get; set; }
    }
}
