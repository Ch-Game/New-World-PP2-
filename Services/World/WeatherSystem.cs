using System;

namespace NewWorldPP2.Services.World
{
    public enum WeatherType
    {
        Clear,
        Rain,
        Fog,
        Heatwave,
        Storm
    }

    // Describes the current weather and its gameplay effects.
    public sealed class WeatherCondition
    {
        public WeatherType Type { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public int AccuracyModifier { get; init; }
        public int EnemyAttackModifier { get; init; }
        public int PlayerDefenseModifier { get; init; }
        public int HealingModifier { get; init; }
        public int TravelEncounterModifier { get; init; }
    }

    // Generates weather changes and resolves saved weather names into effects.
    public static class WeatherSystem
    {
        public static WeatherCondition GetCurrent(WorldState state) => GetCondition(state.CurrentWeather);

        public static void ChangeForLocation(WorldState state, LocationType locationType)
        {
            var roll = Dice.Roll(100);
            var type = locationType switch
            {
                LocationType.Dungeon or LocationType.Gauntlet when roll <= 25 => WeatherType.Storm,
                LocationType.Dungeon or LocationType.Gauntlet when roll <= 50 => WeatherType.Fog,
                _ when roll <= 20 => WeatherType.Rain,
                _ when roll <= 40 => WeatherType.Fog,
                _ when roll <= 55 => WeatherType.Heatwave,
                _ when roll <= 70 => WeatherType.Storm,
                _ => WeatherType.Clear
            };

            state.CurrentWeather = type.ToString();
        }

        public static WeatherCondition GetCondition(string? weatherName)
        {
            if (!Enum.TryParse<WeatherType>(weatherName, true, out var type))
            {
                type = WeatherType.Clear;
            }

            return type switch
            {
                WeatherType.Rain => new WeatherCondition
                {
                    Type = type,
                    Name = "Rain",
                    Description = "Steady rain makes footing slippery and ranged attacks less reliable.",
                    AccuracyModifier = -8,
                    EnemyAttackModifier = -1,
                    TravelEncounterModifier = -5
                },
                WeatherType.Fog => new WeatherCondition
                {
                    Type = type,
                    Name = "Fog",
                    Description = "Dense fog obscures movement and reduces combat accuracy.",
                    AccuracyModifier = -12,
                    EnemyAttackModifier = -1,
                    TravelEncounterModifier = 8
                },
                WeatherType.Heatwave => new WeatherCondition
                {
                    Type = type,
                    Name = "Heatwave",
                    Description = "The oppressive heat makes recovery more difficult.",
                    HealingModifier = -3,
                    PlayerDefenseModifier = -1,
                    TravelEncounterModifier = 5
                },
                WeatherType.Storm => new WeatherCondition
                {
                    Type = type,
                    Name = "Storm",
                    Description = "Thunder and violent winds make every battle dangerous.",
                    AccuracyModifier = -10,
                    EnemyAttackModifier = 1,
                    PlayerDefenseModifier = -1,
                    TravelEncounterModifier = -8
                },
                _ => new WeatherCondition
                {
                    Type = WeatherType.Clear,
                    Name = "Clear",
                    Description = "Clear skies provide ideal conditions for travel and combat."
                }
            };
        }
    }
}
