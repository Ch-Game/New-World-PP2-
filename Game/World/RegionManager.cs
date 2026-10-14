using System;
using NewWorldPP2.World;

namespace NewWorldPP2.World
{
    public class RegionManager
    {
        public string CurrentRegion { get; private set; } = "Starting Province";

        // Simulate a seamless region transition by reporting gradual state changes.
        public void TransitionTo(string region, WorldState world)
        {
            Console.WriteLine($"Beginning seamless transition from {CurrentRegion} to {region}.");
            // small simulated steps
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine($"Transition step {i + 1}/3...");
                world.Visibility = Math.Max(30, world.Visibility - 5);
            }
            CurrentRegion = region;
            Console.WriteLine($"Arrived at {CurrentRegion}. Visibility now {world.Visibility}.");
        }
    }
}
