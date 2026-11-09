using NewWorldPP2.Entities;

namespace NewWorldPP2.Services.World
{
    // Identifies the kind of interaction represented by an encounter.
    public enum EncounterType
    {
        Combat,
        Npc,
        Treasure,
        Trap
    }

    // Holds the description, participants, and rewards for a world encounter.
    public class Encounter
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public EncounterType Type { get; set; }
        public Enemy? Enemy { get; set; }
        public Item? ItemReward { get; set; }
        public int GoldReward { get; set; }
        public Quest? Quest { get; set; }
    }
}
