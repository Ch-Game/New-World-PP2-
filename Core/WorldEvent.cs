using System;
using NewWorldPP2.Core;
using NewWorldPP2.Entities;

namespace NewWorldPP2.World
{
    public class WorldState
    {
        // Visibility can be used by region transition logic; start at 100 (fully visible)
        public int Visibility { get; set; } = 100;
    }

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

    public class EventSystem
    {
        // Trigger an event that is influenced by a d20 roll. High rolls are better for player.
        public void TriggerEvent(string eventName, PlayerCharacter player)
        {
            var roll = Dice.Roll();
            Console.WriteLine($"Event '{eventName}' triggered. d20 roll: {roll}");
            if (roll == 20)
            {
                Console.WriteLine("Critical success! The event turns in the player's favor spectacularly.");
                // grant a small permanent boon as an example
                player.MaxHealth += 5;
            }
            else if (roll >= 12)
            {
                Console.WriteLine("Success: The player handles the event well.");
            }
            else if (roll > 1)
            {
                Console.WriteLine("Partial failure: consequences apply.");
                player.Morality -= 1;
            }
            else
            {
                Console.WriteLine("Critical failure: disastrous outcome.");
                player.Health = Math.Max(1, player.Health / 2);
                player.Morality -= 5;
            }
        }
    }
}
