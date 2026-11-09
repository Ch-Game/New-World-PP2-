namespace NewWorldPP2.Services.World
{
    // Identifies the kind of action available at a world location.
    public enum WorldInteractionType
    {
        Npc,
        Quest,
        Service,
        Chest,
        Door,
        Sign,
        Resource
    }

    // Describes an interactive object or activity at a location.
    public class WorldInteraction
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public WorldInteractionType Type { get; set; }
    }
}
