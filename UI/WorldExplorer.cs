using System;
using System.Collections.Generic;
using NewWorldPP2.Entities;
using NewWorldPP2.Services;
using NewWorldPP2.Services.World;

namespace NewWorldPP2.UI
{
    public static class WorldExplorer
    {
        public static void Enter(PlayerCharacter player)
        {
            var map = WorldMap.CreateDefault();
            var state = new WorldState();
            var quests = new List<Quest>();
            string? lastLocationId = null;

            while (true)
            {
                if (!map.Locations.TryGetValue(state.CurrentLocationId, out var location))
                {
                    state.CurrentLocationId = "silvermere-tavern";
                    location = map.Locations[state.CurrentLocationId];
                }

                if (!string.Equals(lastLocationId, location.Id, StringComparison.Ordinal))
                {
                    ShowArrival(location);
                    lastLocationId = location.Id;
                }

                Console.Clear();
                ShowLocation(player, location);
                Console.WriteLine();
                Console.WriteLine("1) Travel");
                Console.WriteLine("2) Interact with a person or object");
                Console.WriteLine("3) Use location services");
                Console.WriteLine("4) View character");
                Console.WriteLine("5) Search for an encounter");
                Console.WriteLine("6) Challenge a local NPC");
                Console.WriteLine("7) View quests");
                Console.WriteLine("8) Leave the fantasy world");
                Console.Write("Choose an action: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        Travel(map, state, location, player, quests);
                        break;
                    case "2":
                        InteractWithObject(player, location);
                        break;
                    case "3":
                        UseLocationServices(player, location);
                        break;
                    case "4":
                        Console.Clear();
                        Console.WriteLine(player.GetFullDescription());
                        Pause();
                        break;
                    case "5":
                        HandleEncounter(EncounterSystem.CreateLocationEncounter(location), player, quests);
                        Pause();
                        break;
                    case "6":
                        HandleEncounter(EncounterSystem.CreateNpcEncounter(location), player, quests);
                        Pause();
                        break;
                    case "7":
                        ShowQuests(quests);
                        Pause();
                        break;
                    case "8":
                        return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        Pause();
                        break;
                }
            }
        }

        private static void ShowLocation(PlayerCharacter player, WorldLocation location)
        {
            Console.WriteLine("=== The Realms of Aetheria ===");
            Console.WriteLine($"Adventurer: {player.Name}");
            Console.WriteLine($"Location: {location.Name} ({location.Type})");
            if (!string.IsNullOrWhiteSpace(location.TownName))
            {
                Console.WriteLine($"Town: {location.TownName}");
            }

            Console.WriteLine();
            Console.WriteLine(location.Description);
        }

        private static void Travel(WorldMap map, WorldState state, WorldLocation currentLocation, PlayerCharacter player, List<Quest> quests)
        {
            Console.Clear();
            Console.WriteLine($"Travel from {currentLocation.Name}");
            Console.WriteLine();

            for (var i = 0; i < currentLocation.Connections.Count; i++)
            {
                var destination = map.Locations[currentLocation.Connections[i]];
                Console.WriteLine($"{i + 1}) {destination.Name} ({destination.Type})");
                Console.WriteLine($"   {destination.Description}");
            }

            Console.WriteLine("0) Stay here");
            Console.Write("Choose a destination: ");
            var input = Console.ReadLine();
            if (!int.TryParse(input, out var choice) || choice < 0 || choice > currentLocation.Connections.Count)
            {
                Console.WriteLine("Invalid destination.");
                Pause();
                return;
            }

            if (choice == 0)
            {
                return;
            }

            state.CurrentLocationId = currentLocation.Connections[choice - 1];
            var destinationLocation = map.Locations[state.CurrentLocationId];
            Console.WriteLine($"You travel to {destinationLocation.Name}.");
            if (Dice.Roll(2) == 1)
            {
                Console.WriteLine("Something approaches on the road...");
                HandleEncounter(EncounterSystem.CreateRandomEncounter(destinationLocation), player, quests);
            }

            Pause();
        }

        private static void HandleEncounter(Encounter encounter, PlayerCharacter player, List<Quest> quests)
        {
            Console.Clear();
            Console.WriteLine($"=== Encounter: {encounter.Name} ===");
            Console.WriteLine(encounter.Description);
            Console.WriteLine();

            switch (encounter.Type)
            {
                case EncounterType.Combat:
                    if (encounter.Enemy != null && CombatSystem.Fight(player, encounter.Enemy))
                    {
                        RecordQuestProgress(quests);
                    }
                    break;
                case EncounterType.Npc:
                    Console.WriteLine("1) Accept the NPC's request");
                    Console.WriteLine("2) Challenge the NPC to a fight");
                    Console.WriteLine("0) Walk away");
                    Console.Write("Choose an option: ");

                    switch (Console.ReadLine())
                    {
                        case "1":
                            if (encounter.Quest != null)
                            {
                                quests.Add(encounter.Quest);
                                Console.WriteLine($"Quest accepted: {encounter.Quest.Title}");
                                Console.WriteLine(encounter.Quest.Description);
                            }
                            break;
                        case "2":
                            if (encounter.Enemy != null && CombatSystem.Fight(player, encounter.Enemy))
                            {
                                RecordQuestProgress(quests);
                            }
                            break;
                        case "0":
                            Console.WriteLine("You leave the NPC alone.");
                            break;
                        default:
                            Console.WriteLine("Invalid encounter choice.");
                            break;
                    }
                    break;
                case EncounterType.Treasure:
                    player.Gold += encounter.GoldReward;
                    Console.WriteLine($"You find {encounter.GoldReward} gold.");
                    AddReward(player, encounter.ItemReward);
                    RecordQuestProgress(quests);
                    break;
                case EncounterType.Trap:
                    var damage = Dice.Roll(6);
                    player.Health = Math.Max(1, player.Health - damage);
                    Console.WriteLine($"The trap injures you for {damage} damage.");
                    break;
            }

            CompleteQuests(quests, player);
        }

        private static void RecordQuestProgress(List<Quest> quests)
        {
            foreach (var quest in quests)
            {
                quest.RecordProgress();
            }
        }

        private static void CompleteQuests(List<Quest> quests, PlayerCharacter player)
        {
            foreach (var quest in quests)
            {
                if (quest.Status != QuestStatus.Completed || quest.GoldReward == 0) continue;

                player.Gold += quest.GoldReward;
                AddReward(player, quest.ItemReward);
                Console.WriteLine($"Quest complete: {quest.Title}. Reward: {quest.GoldReward} gold.");
                quest.GoldReward = 0;
            }
        }

        private static void AddReward(PlayerCharacter player, Item? item)
        {
            if (item == null) return;

            player.Inventory.Add(item);
            Console.WriteLine($"You acquire: {item.Name}.");
        }

        private static void ShowQuests(List<Quest> quests)
        {
            Console.Clear();
            Console.WriteLine("=== Quest Log ===");
            if (quests.Count == 0)
            {
                Console.WriteLine("You have no active or completed quests.");
                return;
            }

            foreach (var quest in quests)
            {
                Console.WriteLine($"- {quest.Title} [{quest.Status}] ({quest.CompletedEncounters}/{quest.RequiredEncounters})");
                Console.WriteLine($"  {quest.Description}");
            }
        }

        private static void ShowArrival(WorldLocation location)
        {
            Console.Clear();
            Console.WriteLine($"You arrive at {location.Name}.");
            Console.WriteLine(location.Type switch
            {
                LocationType.Tavern => "The smell of warm food and woodsmoke welcomes you.",
                LocationType.Guild => "Guild members pause to assess the new arrival.",
                LocationType.Market => "Merchants call out from their colorful stalls.",
                LocationType.Landmark => "The atmosphere changes as the ancient place reveals itself.",
                LocationType.Shrine => "A quiet flame stirs as you enter the sacred grounds.",
                LocationType.Dungeon => "Cold air rises from the depths, carrying a distant sound.",
                _ => "The area is busy with travelers and local residents."
            });
            Pause();
        }

        private static void InteractWithObject(PlayerCharacter player, WorldLocation location)
        {
            Console.Clear();
            Console.WriteLine($"People and objects at {location.Name}");
            Console.WriteLine();
            for (var i = 0; i < location.Interactions.Count; i++)
            {
                var listedInteraction = location.Interactions[i];
                Console.WriteLine($"{i + 1}) {listedInteraction.Name} [{listedInteraction.Type}]");
                Console.WriteLine($"   {listedInteraction.Description}");
            }

            Console.WriteLine("0) Back");
            Console.Write("Choose an interaction: ");
            var input = Console.ReadLine();
            if (!int.TryParse(input, out var choice) || choice < 0 || choice > location.Interactions.Count)
            {
                Console.WriteLine("Invalid interaction.");
                Pause();
                return;
            }

            if (choice == 0)
            {
                return;
            }

            var interaction = location.Interactions[choice - 1];
            Console.Clear();
            Console.WriteLine(interaction.Name);
            Console.WriteLine(interaction.Description);
            Console.WriteLine();
            ResolveInteraction(player, location, interaction);
            Pause();
        }

        private static void ResolveInteraction(PlayerCharacter player, WorldLocation location, WorldInteraction interaction)
        {
            switch (interaction.Type)
            {
                case WorldInteractionType.Npc:
                    player.Morality++;
                    Console.WriteLine("You have a useful conversation and gain a better understanding of the area.");
                    break;
                case WorldInteractionType.Quest:
                    player.Gold += 15;
                    player.Morality++;
                    Console.WriteLine("You accept the task and receive an advance of 15 gold.");
                    break;
                case WorldInteractionType.Service:
                    UseSpecificService(player, interaction);
                    break;
                case WorldInteractionType.Chest:
                    if (player.TryOpenChest(interaction.Id))
                    {
                        player.Gold += 10;
                        Console.WriteLine("You find 10 gold inside.");
                    }
                    else
                    {
                        Console.WriteLine("The chest is already open.");
                    }
                    break;
                case WorldInteractionType.Door:
                    Console.WriteLine("The way opens. You prepare yourself for what lies beyond.");
                    if (location.Type == LocationType.Dungeon)
                    {
                        DungeonInteraction(player, location);
                    }
                    break;
                case WorldInteractionType.Sign:
                    Console.WriteLine("The writing confirms that this is a known place on the realm's map.");
                    break;
                case WorldInteractionType.Resource:
                    var resource = interaction.Id.Contains("ore", StringComparison.Ordinal)
                        ? new Item { Name = "Glowing Cinder Ore", Rarity = ItemRarity.Uncommon, Magic = 2, Durability = 10 }
                        : new Item { Name = "Moonleaf Herb", Rarity = ItemRarity.Common, Magic = 1, Durability = 3 };
                    player.Inventory.Add(resource);
                    Console.WriteLine($"You gather {resource.Name} and add it to your inventory.");
                    break;
            }
        }

        private static void UseSpecificService(PlayerCharacter player, WorldInteraction interaction)
        {
            if (interaction.Id.Contains("room", StringComparison.Ordinal) || interaction.Id.Contains("blessing", StringComparison.Ordinal))
            {
                player.Health = player.MaxHealth;
                player.Morality++;
                Console.WriteLine("The service restores your health and lifts your spirits.");
            }
            else if (interaction.Id.Contains("smith", StringComparison.Ordinal))
            {
                const int cost = 25;
                if (player.Gold < cost)
                {
                    Console.WriteLine("You do not have enough gold for the weapon.");
                    return;
                }

                player.Gold -= cost;
                player.Inventory.Add(ItemFactory.CreateWeapon(WeaponCategory.Sword, Material.Iron, ItemRarity.Common));
                Console.WriteLine("You purchase an Iron Sword.");
            }
            else
            {
                player.MaxHealth++;
                player.Health = Math.Min(player.MaxHealth, player.Health + 1);
                Console.WriteLine("The service improves your readiness. Maximum health increases by 1.");
            }
        }

        private static void UseLocationServices(PlayerCharacter player, WorldLocation location)
        {
            Console.Clear();
            Console.WriteLine($"=== {location.Name} ===");
            Console.WriteLine();
            Console.WriteLine(location.Description);
            Console.WriteLine();
            Console.WriteLine("Available here:");
            foreach (var feature in location.Features)
            {
                Console.WriteLine($"- {feature}");
            }

            Console.WriteLine();
            switch (location.Type)
            {
                case LocationType.Tavern:
                    TavernInteraction(player);
                    break;
                case LocationType.Guild:
                    GuildInteraction(player);
                    break;
                case LocationType.Market:
                    MarketInteraction(player);
                    break;
                case LocationType.Landmark:
                    LandmarkInteraction(player, location);
                    break;
                case LocationType.Shrine:
                    ShrineInteraction(player);
                    break;
                case LocationType.Dungeon:
                    DungeonInteraction(player, location);
                    break;
                case LocationType.Town:
                    Console.WriteLine("The townsfolk welcome you. Ask around for local news.");
                    player.Morality++;
                    break;
            }

            Pause();
        }

        private static void TavernInteraction(PlayerCharacter player)
        {
            Console.WriteLine("You settle beside the hearth and speak with the tavern keeper.");
            player.Health = player.MaxHealth;
            Console.WriteLine("You rest fully and recover all health.");
            Console.WriteLine("Rumor: A sealed road near the Old Watchtower has recently reopened.");
        }

        private static void GuildInteraction(PlayerCharacter player)
        {
            Console.WriteLine("The guild clerk offers you a simple contract.");
            player.Gold += 20;
            player.MaxHealth++;
            player.Health = Math.Min(player.MaxHealth, player.Health + 1);
            Console.WriteLine("You complete a training contract and earn 20 gold.");
            Console.WriteLine("Guild training permanently increases your maximum health by 1.");
        }

        private static void MarketInteraction(PlayerCharacter player)
        {
            const int itemCost = 25;
            Console.WriteLine($"A blacksmith offers you an Iron Sword for {itemCost} gold.");
            if (player.Gold < itemCost)
            {
                Console.WriteLine("You do not have enough gold.");
                return;
            }

            player.Gold -= itemCost;
            player.Inventory.Add(ItemFactory.CreateWeapon(WeaponCategory.Sword, Material.Iron, ItemRarity.Common));
            Console.WriteLine("You purchase an Iron Sword and add it to your inventory.");
        }

        private static void LandmarkInteraction(PlayerCharacter player, WorldLocation location)
        {
            var roll = Dice.Roll();
            Console.WriteLine($"You investigate {location.Name}. Exploration roll: {roll}");
            if (roll >= 12)
            {
                player.Gold += 15;
                Console.WriteLine("You discover an old traveler's cache containing 15 gold.");
            }
            else
            {
                player.Health = Math.Max(1, player.Health - 2);
                Console.WriteLine("A hidden hazard injures you for 2 health.");
            }
        }

        private static void ShrineInteraction(PlayerCharacter player)
        {
            player.Health = player.MaxHealth;
            player.Morality++;
            Console.WriteLine("You pray at the shrine. Your wounds close and your resolve strengthens.");
            Console.WriteLine("Health restored and morality increased by 1.");
        }

        private static void DungeonInteraction(PlayerCharacter player, WorldLocation location)
        {
            var roll = Dice.Roll();
            Console.WriteLine($"You enter {location.Name}. Expedition roll: {roll}");
            if (roll >= 10)
            {
                player.Gold += 30;
                player.Inventory.Add(ItemFactory.CreateWeapon(WeaponCategory.Dagger, Material.Bronze, ItemRarity.Uncommon));
                Console.WriteLine("You defeat a lurking creature and recover 30 gold and an Uncommon Bronze Dagger.");
            }
            else
            {
                player.Health = Math.Max(1, player.Health - 5);
                Console.WriteLine("The dungeon fights back. You escape with 5 health damage.");
            }
        }

        private static void Pause()
        {
            Console.WriteLine();
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }
    }
}
