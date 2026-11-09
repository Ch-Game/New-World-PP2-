

namespace NewWorldPP2.Services.World
{
    // Stores mutable state shared by world navigation systems.
    public class WorldState
    {
        // Visibility can be used by region transition logic; start at 100 (fully visible)
        public int Visibility { get; set; } = 100;

        // Identifies the location currently occupied by the player.
        public string CurrentLocationId { get; set; } = "silvermere-tavern";
    }

}

