

namespace NewWorldPP2.Services.World
{
    public class WorldState
    {
        // Visibility can be used by region transition logic; start at 100 (fully visible)
        public int Visibility { get; set; } = 100;

        public string CurrentLocationId { get; set; } = "silvermere-tavern";
    }

}

