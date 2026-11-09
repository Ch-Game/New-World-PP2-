using System.Collections.Generic;

namespace NewWorldPP2.Services.World
{
    // Contains towns and locations that make up the explorable world.
    public class WorldMap
    {
        public List<Town> Towns { get; } = new();
        public Dictionary<string, WorldLocation> Locations { get; } = new();

        // Builds the game's starting map and its connected locations.
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
                LocationIds = new() { "emberfall-tavern", "emberfall-guild", "emberfall-shrine", "cinder-mines", "emberfall-gauntlet" }
            });

            map.AddLocation(new WorldLocation
            {
                Id = "silvermere-tavern",
                Name = "The Moonlit Tankard",
                TownName = "Silvermere",
                Type = LocationType.Tavern,
                Description = "A warm tavern overlooking the lake. Its common room is full of rumors, songs, and quest notices.",
                Features = new() { "Rest and recover", "Listen for rumors", "Accept local quests" },
                Interactions = new()
                {
                    new WorldInteraction { Id = "silvermere-keeper", Name = "Speak with the tavern keeper", Description = "The keeper knows which roads are safe and which travelers need help.", Type = WorldInteractionType.Npc },
                    new WorldInteraction { Id = "silvermere-noticeboard", Name = "Read the quest noticeboard", Description = "Several handwritten requests are pinned beneath the tavern's old map.", Type = WorldInteractionType.Quest }
                },
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
                Interactions = new()
                {
                    new WorldInteraction { Id = "silvermere-guild-clerk", Name = "Speak with the guild clerk", Description = "The clerk sorts contracts and records the deeds of local adventurers.", Type = WorldInteractionType.Npc },
                    new WorldInteraction { Id = "silvermere-guild-board", Name = "Inspect the bounty board", Description = "Fresh bounty notices describe dangers along the frontier roads.", Type = WorldInteractionType.Quest },
                    new WorldInteraction { Id = "silvermere-guild-banner", Name = "Read the guild banner", Description = "The banner lists the guild's rules for protecting travelers.", Type = WorldInteractionType.Sign }
                },
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
                Interactions = new()
                {
                    new WorldInteraction { Id = "silvermere-merchant", Name = "Speak with a traveling merchant", Description = "A merchant offers rumors about rare goods arriving from distant regions.", Type = WorldInteractionType.Npc },
                    new WorldInteraction { Id = "silvermere-blacksmith", Name = "Visit the blacksmith", Description = "The blacksmith is ready to sell a sturdy iron sword.", Type = WorldInteractionType.Service }
                },
                Connections = new() { "silvermere-tavern", "silvermere-guild", "old-watchtower" }
            });
            map.AddLocation(new WorldLocation
            {
                Id = "whispering-woods",
                Name = "The Whispering Woods",
                Type = LocationType.Landmark,
                Description = "Ancient trees surround a mossy trail. The leaves whisper names that no living traveler remembers.",
                Features = new() { "Explore the forest", "Search for herbs", "Follow mysterious sounds" },
                Interactions = new()
                {
                    new WorldInteraction { Id = "whispering-woods-herbs", Name = "Gather moonleaf herbs", Description = "Pale herbs grow beneath the whispering trees.", Type = WorldInteractionType.Resource },
                    new WorldInteraction { Id = "whispering-woods-sounds", Name = "Follow the mysterious sounds", Description = "Something is moving beyond the moss-covered trail.", Type = WorldInteractionType.Sign }
                },
                Connections = new() { "silvermere-tavern", "old-watchtower" }
            });
            map.AddLocation(new WorldLocation
            {
                Id = "old-watchtower",
                Name = "The Old Watchtower",
                Type = LocationType.Landmark,
                Description = "A ruined watchtower overlooking the road between Silvermere and Emberfall.",
                Features = new() { "Survey the roads", "Search the ruins", "Watch for travelers" },
                Interactions = new()
                {
                    new WorldInteraction { Id = "watchtower-scout", Name = "Speak with the road scout", Description = "A tired scout watches the roads from the tower's broken stairs.", Type = WorldInteractionType.Npc },
                    new WorldInteraction { Id = "watchtower-chest", Name = "Open the old supply chest", Description = "An abandoned supply chest rests beneath a layer of dust.", Type = WorldInteractionType.Chest },
                    new WorldInteraction { Id = "watchtower-sign", Name = "Read the old road marker", Description = "The marker points toward Silvermere and Emberfall.", Type = WorldInteractionType.Sign }
                },
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
                Interactions = new()
                {
                    new WorldInteraction { Id = "emberfall-keeper", Name = "Speak with the frontier innkeeper", Description = "The innkeeper has heard reports from miners returning from the Cinder Mines.", Type = WorldInteractionType.Npc },
                    new WorldInteraction { Id = "emberfall-caravan", Name = "Review the caravan request", Description = "A caravan master is looking for help along the volcanic road.", Type = WorldInteractionType.Quest }
                },
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
                Interactions = new()
                {
                    new WorldInteraction { Id = "emberfall-commander", Name = "Speak with the guild commander", Description = "The commander coordinates patrols against creatures from the mines.", Type = WorldInteractionType.Npc },
                    new WorldInteraction { Id = "emberfall-contracts", Name = "Take a frontier contract", Description = "A contract asks a capable adventurer to deal with a nearby threat.", Type = WorldInteractionType.Quest },
                    new WorldInteraction { Id = "emberfall-training-post", Name = "Inspect the training post", Description = "The battered training post bears marks from countless practice fights.", Type = WorldInteractionType.Sign }
                },
                Connections = new() { "emberfall-tavern", "emberfall-shrine", "old-watchtower", "emberfall-gauntlet" }
            });
            map.AddLocation(new WorldLocation
            {
                Id = "emberfall-shrine",
                Name = "Shrine of the Ember Star",
                TownName = "Emberfall",
                Type = LocationType.Shrine,
                Description = "A quiet shrine glowing with a coal that has burned since the first settlers arrived.",
                Features = new() { "Pray for guidance", "Learn local history", "Light a protective candle" },
                Interactions = new()
                {
                    new WorldInteraction { Id = "emberfall-priest", Name = "Speak with the shrine keeper", Description = "The shrine keeper studies the eternal coal and welcomes travelers.", Type = WorldInteractionType.Npc },
                    new WorldInteraction { Id = "emberfall-candle", Name = "Light a protective candle", Description = "A small candle waits beside the shrine's eternal coal.", Type = WorldInteractionType.Sign },
                    new WorldInteraction { Id = "emberfall-shrine-chest", Name = "Open the offering chest", Description = "Travelers leave small offerings in a sturdy wooden chest.", Type = WorldInteractionType.Chest }
                },
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
                Interactions = new()
                {
                    new WorldInteraction { Id = "cinder-mines-ore", Name = "Search for glowing ore", Description = "Veins of glowing cinder ore can be seen in the mine wall.", Type = WorldInteractionType.Resource },
                    new WorldInteraction { Id = "cinder-mines-door", Name = "Open the mine gate", Description = "A reinforced gate blocks the safest route into the depths.", Type = WorldInteractionType.Door }
                },
                Connections = new() { "emberfall-tavern", "emberfall-shrine", "old-watchtower", "emberfall-gauntlet" }
            });
            map.AddLocation(new WorldLocation
            {
                Id = "emberfall-gauntlet",
                Name = "The Emberfall Gauntlet",
                TownName = "Emberfall",
                Type = LocationType.Gauntlet,
                Description = "A fortified arena carved into the volcanic cliffs. Champions enter for glory, treasure, and the chance to face the final flamebeast.",
                Features = new() { "Enter the five-stage gauntlet", "Claim champion loot", "Test your combat skills" },
                Interactions = new()
                {
                    new WorldInteraction { Id = "emberfall-gauntlet-champion", Name = "Speak with the arena champion", Description = "The champion explains the risks of entering the Emberfall Gauntlet.", Type = WorldInteractionType.Npc },
                    new WorldInteraction { Id = "emberfall-gauntlet-gate", Name = "Approach the gauntlet gate", Description = "The massive gate opens toward the five-stage challenge.", Type = WorldInteractionType.Door }
                },
                Connections = new() { "emberfall-tavern", "emberfall-guild", "cinder-mines" }
            });

            return map;
        }

        // Adds a town to the map's settlement collection.
        private void AddTown(Town town) => Towns.Add(town);

        // Registers a location by its stable identifier for quick lookup.
        private void AddLocation(WorldLocation location) => Locations[location.Id] = location;
    }
}
