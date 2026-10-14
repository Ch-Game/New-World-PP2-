namespace NewWorldPP2.World
{
    // Represents global aggregated world state that various systems read & modify.
    public class WorldState
    {
        // 0..100 percent visibility
        public int Visibility { get; set; } = 100;

        // Travel speed multiplier, 1.0 = normal
        public float TravelSpeedModifier { get; set; } = 1f;

        // Long-term consequence storage (very simplified)
        public int WorldStability { get; set; } = 50; // 0..100
    }
}
