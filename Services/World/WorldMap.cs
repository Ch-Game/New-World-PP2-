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
                Interactions = new()
                {
                    new() { Id = "moonlit-innkeeper", Name = "Speak with Innkeeper Mira", Type = WorldInteractionType.Npc, Description = "Mira knows every traveler who has passed through Silvermere." },
                    new() { Id = "moonlit-room", Name = "Rent a room", Type = WorldInteractionType.Service, Description = "Rest safely and recover your health." },
                    new() { Id = "moonlit-notice", Name = "Read the quest board", Type = WorldInteractionType.Quest, Description = "Look for a small job posted by a worried ferryman." }
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
                    new() { Id = "silver-guild-master", Name = "Speak with Guildmaster Rowan", Type = WorldInteractionType.Npc, Description = "The guildmaster evaluates new adventurers." },
                    new() { Id = "silver-guild-contract", Name = "Accept a bounty contract", Type = WorldInteractionType.Quest, Description = "A local bounty offers a reliable reward." },
                    new() { Id = "silver-guild-training", Name = "Use the training yard", Type = WorldInteractionType.Service, Description = "Practice your combat stance with guild veterans." }
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
                    new() { Id = "silver-market-smith", Name = "Visit the blacksmith", Type = WorldInteractionType.Service, Description = "Purchase a dependable iron weapon." },
                    new() { Id = "silver-market-chest", Name = "Inspect the merchant chest", Type = WorldInteractionType.Chest, Description = "A merchant offers a small sample from a locked stock chest." },
                    new() { Id = "silver-market-sign", Name = "Read the town sign", Type = WorldInteractionType.Sign, Description = "The sign lists the safest roads out of Silvermere." }
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
                    new() { Id = "woods-herbalist", Name = "Help the forest herbalist", Type = WorldInteractionType.Npc, Description = "An herbalist asks you to listen for danger in the trees." },
                    new() { Id = "woods-herbs", Name = "Gather moonleaf herbs", Type = WorldInteractionType.Resource, Description = "Collect useful herbs growing beside the trail." },
                    new() { Id = "woods-stone-door", Name = "Examine the root-covered door", Type = WorldInteractionType.Door, Description = "An ancient sealed door is hidden beneath the roots." }
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
                    new() { Id = "watchtower-scout", Name = "Speak with the road scout", Type = WorldInteractionType.Npc, Description = "A scout watches the road for bandits and monsters." },
                    new() { Id = "watchtower-cache", Name = "Open the old supply chest", Type = WorldInteractionType.Chest, Description = "A weathered chest may still contain useful supplies." },
                    new() { Id = "watchtower-sign", Name = "Read the route marker", Type = WorldInteractionType.Sign, Description = "The marker points toward Silvermere and Emberfall." }
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
                    new() { Id = "stag-barkeep", Name = "Speak with Barkeep Harlan", Type = WorldInteractionType.Npc, Description = "Harlan hears rumors from every caravan entering Emberfall." },
                    new() { Id = "stag-room", Name = "Rent a room", Type = WorldInteractionType.Service, Description = "Rest beside the warm frontier hearth." },
                    new() { Id = "stag-caravan", Name = "Join a caravan", Type = WorldInteractionType.Quest, Description = "A caravan needs an extra guard for the road." }
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
                    new() { Id = "iron-quartermaster", Name = "Speak with the quartermaster", Type = WorldInteractionType.Npc, Description = "The quartermaster records threats along the frontier." },
                    new() { Id = "iron-contract", Name = "Accept a mine contract", Type = WorldInteractionType.Quest, Description = "The guild needs someone to investigate missing miners." },
                    new() { Id = "iron-armory", Name = "Use the guild armory", Type = WorldInteractionType.Service, Description = "Train and prepare for a dangerous expedition." }
                },
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
                Interactions = new()
                {
                    new() { Id = "ember-priest", Name = "Speak with Priestess Veya", Type = WorldInteractionType.Npc, Description = "The priestess explains the shrine's ancient ember." },
                    new() { Id = "ember-blessing", Name = "Receive an ember blessing", Type = WorldInteractionType.Service, Description = "Ask the shrine to mend your wounds and strengthen your resolve." },
                    new() { Id = "ember-inscription", Name = "Read the founder's inscription", Type = WorldInteractionType.Sign, Description = "The inscription tells how Emberfall survived its first eruption." }
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
                    new() { Id = "cinder-entrance", Name = "Open the mine gate", Type = WorldInteractionType.Door, Description = "Unlock the old gate and begin an expedition." },
                    new() { Id = "cinder-ore", Name = "Mine a glowing ore vein", Type = WorldInteractionType.Resource, Description = "Gather valuable ore while the tunnel is quiet." },
                    new() { Id = "cinder-captive", Name = "Answer a miner's call", Type = WorldInteractionType.Npc, Description = "Someone deeper in the mine is calling for help." }
                },
                Connections = new() { "emberfall-tavern", "emberfall-shrine", "old-watchtower" }
            });

            return map;
        }

        private void AddTown(Town town) => Towns.Add(town);

        private void AddLocation(WorldLocation location) => Locations[location.Id] = location;
    }
}
