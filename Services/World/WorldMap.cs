using System.Collections.Generic;

namespace NewWorldPP2.Services.World
{
    public class WorldMap
    {
        public List<Town> Towns { get; } = new();
        public Dictionary<string, WorldLocation> Locations { get; } = new();

        public static WorldMap CreateDefault()
        {
            var map = new WorldMap();

            map.AddTown(new Town
            {
                Name = "Silvermere",
                Description = "A bright lakeside town where merchants, sailors, and adventurers meet.",
                LocationIds = new() { "silvermere-tavern", "silvermere-guild", "silvermere-market" }
            });
            map.AddTown(new Town
            {
                Name = "Emberfall",
                Description = "A frontier settlement built beneath red cliffs and the smoke of old volcanoes.",
                LocationIds = new() { "emberfall-tavern", "emberfall-guild", "emberfall-shrine", "cinder-mines" }
            });

            map.AddLocation(new WorldLocation
            {
                Id = "silvermere-tavern",
                Name = "The Moonlit Tankard",
                TownName = "Silvermere",
                Type = LocationType.Tavern,
                Description = "A warm tavern overlooking the lake. Its common room is full of rumors, songs, and quest notices.",
                Features = new() { "Rest and recover", "Listen for rumors", "Accept local quests" },
                Connections = new() { "silvermere-guild", "silvermere-market", "whispering-woods" }
            });
            map.AddLocation(new WorldLocation
            {
                Id = "silvermere-guild",
                Name = "Silvermere Adventurers Guild",
                TownName = "Silvermere",
                Type = LocationType.Guild,
                Description = "The regional guild hall, where new heroes register and veteran adventurers post contracts.",
                Features = new() { "Register as an adventurer", "Review bounty contracts", "Train with guild members" },
                Connections = new() { "silvermere-tavern", "silvermere-market", "old-watchtower" }
            });
            map.AddLocation(new WorldLocation
            {
                Id = "silvermere-market",
                Name = "Silvermere Market Square",
                TownName = "Silvermere",
                Type = LocationType.Market,
                Description = "Colorful stalls sell provisions, maps, weapons, and strange treasures from distant lands.",
                Features = new() { "Browse supplies", "Visit the blacksmith", "Speak with traveling merchants" },
                Connections = new() { "silvermere-tavern", "silvermere-guild", "old-watchtower" }
            });
            map.AddLocation(new WorldLocation
            {
                Id = "whispering-woods",
                Name = "The Whispering Woods",
                Type = LocationType.Landmark,
                Description = "Ancient trees surround a mossy trail. The leaves whisper names that no living traveler remembers.",
                Features = new() { "Explore the forest", "Search for herbs", "Follow mysterious sounds" },
                Connections = new() { "silvermere-tavern", "old-watchtower" }
            });
            map.AddLocation(new WorldLocation
            {
                Id = "old-watchtower",
                Name = "The Old Watchtower",
                Type = LocationType.Landmark,
                Description = "A ruined watchtower overlooking the road between Silvermere and Emberfall.",
                Features = new() { "Survey the roads", "Search the ruins", "Watch for travelers" },
                Connections = new() { "silvermere-guild", "silvermere-market", "whispering-woods", "emberfall-guild", "cinder-mines" }
            });
            map.AddLocation(new WorldLocation
            {
                Id = "emberfall-tavern",
                Name = "The Golden Stag",
                TownName = "Emberfall",
                Type = LocationType.Tavern,
                Description = "A lively frontier tavern where miners, caravan guards, and monster hunters share a crowded hearth.",
                Features = new() { "Rest and recover", "Hear frontier rumors", "Join a caravan" },
                Connections = new() { "emberfall-guild", "emberfall-shrine", "cinder-mines" }
            });
            map.AddLocation(new WorldLocation
            {
                Id = "emberfall-guild",
                Name = "The Iron Vanguard Guild",
                TownName = "Emberfall",
                Type = LocationType.Guild,
                Description = "A fortified guild building that protects the frontier and coordinates expeditions into the mines.",
                Features = new() { "Take frontier contracts", "Practice combat", "Form an expedition" },
                Connections = new() { "emberfall-tavern", "emberfall-shrine", "old-watchtower" }
            });
            map.AddLocation(new WorldLocation
            {
                Id = "emberfall-shrine",
                Name = "Shrine of the Ember Star",
                TownName = "Emberfall",
                Type = LocationType.Shrine,
                Description = "A quiet shrine glowing with a coal that has burned since the first settlers arrived.",
                Features = new() { "Pray for guidance", "Learn local history", "Light a protective candle" },
                Connections = new() { "emberfall-tavern", "emberfall-guild", "cinder-mines" }
            });
            map.AddLocation(new WorldLocation
            {
                Id = "cinder-mines",
                Name = "The Cinder Mines",
                TownName = "Emberfall",
                Type = LocationType.Dungeon,
                Description = "Abandoned mines descend beneath the cliffs. Strange red light flickers deep below.",
                Features = new() { "Enter the dungeon", "Search for ore", "Investigate the red glow" },
                Connections = new() { "emberfall-tavern", "emberfall-shrine", "old-watchtower" }
            });

            return map;
        }

        private void AddTown(Town town) => Towns.Add(town);

        private void AddLocation(WorldLocation location) => Locations[location.Id] = location;
    }
}
