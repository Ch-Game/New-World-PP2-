namespace NewWorldPP2.Services.World
{
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

    public class WorldInteraction
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public WorldInteractionType Type { get; set; }
    }
}
