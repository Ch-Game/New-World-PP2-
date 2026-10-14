using System;
using NewWorldPP2.World;

namespace NewWorldPP2.Systems
{
    public class WeatherSystem
    {
        public WeatherType Current { get; private set; } = WeatherType.Clear;

        // Simple weather update that can affect visibility, travel speed, etc.
        public void Update(WorldState world)
        {
            // Example deterministic-ish change using dice
            var roll = Core.Dice.Roll();
            if (roll <= 5) Current = WeatherType.Storm;
            else if (roll <= 10) Current = WeatherType.Rain;
            else if (roll <= 15) Current = WeatherType.Cloudy;
            else Current = WeatherType.Clear;

            Console.WriteLine($"Weather changed to {Current} (roll {roll}).");
            ApplyEffects(world);
        }

        private void ApplyEffects(WorldState world)
        {
            switch (Current)
            {
                case WeatherType.Storm:
                    world.Visibility -= 30;
                    world.TravelSpeedModifier -= 0.3f;
                    break;
                case WeatherType.Rain:
                    world.Visibility -= 10;
                    world.TravelSpeedModifier -= 0.15f;
                    break;
                case WeatherType.Cloudy:
                    world.Visibility -= 5;
                    break;
                case WeatherType.Clear:
                default:
                    // small recovery towards defaults
                    world.Visibility = Math.Min(world.Visibility + 5, 100);
                    world.TravelSpeedModifier = Math.Min(world.TravelSpeedModifier + 0.05f, 1f);
                    break;
            }
            Console.WriteLine($"World visibility now {world.Visibility}, travel modifier {world.TravelSpeedModifier:0.00}.");
        }
    }

    public enum WeatherType
    {
        Clear,
        Cloudy,
        Rain,
        Storm
    }
}
