using System;
using System.Collections.Generic;
using NewWorldPP2.Entities;
using NewWorldPP2.Services;
using NewWorldPP2.Services.World;

namespace NewWorldPP2.UI
{
    // Runs the main console loop for navigating and interacting with the world.
    public static class WorldExplorer
    {
        // Starts world exploration and returns when the player leaves or dies.
        public static void Enter(PlayerCharacter player, WorldState? savedState = null,
            List<Quest>? savedQuests = null, Action? onAutosave = null, Action? onDeath = null)
        {
            var map = WorldMap.CreateDefault();
            var state = savedState ?? new WorldState();
            var quests = savedQuests ?? new List<Quest>();
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
                Console.WriteLine("2) Interact with a person, object, or location service");
                Console.WriteLine("3) View character");
                Console.WriteLine("4) Enter dungeon or gauntlet");
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
                        InteractWithObject(player, location, quests);
                        break;
                    case "3":
                        Console.Clear();
                        Console.WriteLine(player.GetFullDescription());
                        Pause();
                        break;
                    case "4":
                        if (location.Type is LocationType.Dungeon or LocationType.Gauntlet)
                        {
                            DungeonSystem.Run(player, location);
                        }
                        else
                        {
                            Console.WriteLine("There is no dungeon or gauntlet at this location.");
                            Pause();
                        }
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

                onAutosave?.Invoke();

                if (player.Health <= 0)
                {
                    Console.Clear();
                    Console.WriteLine("Your character has died. This character can never be played again.");
                    onDeath?.Invoke();
                    Pause();
                    return;
                }
            }
        }

        // Displays the current location and the player's basic context.
        private static void ShowLocation(PlayerCharacter player, WorldLocation location)
        {
            Console.WriteLine("=== The Realms of Aetheria ===");
            Console.WriteLine($"Adventurer: {player.Name}");
            Console.WriteLine($"Level: {player.Level} | XP: {player.Experience}/{player.ExperienceToNextLevel} | Stat points: {player.UnspentStatPoints}");
            Console.WriteLine($"Location: {location.Name} ({location.Type})");
            if (!string.IsNullOrWhiteSpace(location.TownName))
            {
                Console.WriteLine($"Town: {location.TownName}");
            }

            Console.WriteLine();
            Console.WriteLine(location.Description);
        }

        // Moves to a connected location and may trigger a road encounter.
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
            if (Dice.Roll(100) <= StatCalculator.TravelEncounterChance(player.Stats))
            {
                Console.WriteLine("Something approaches on the road...");
                HandleEncounter(EncounterSystem.CreateRandomEncounter(destinationLocation), player, quests);
            }

            Pause();
        }

        // Resolves the selected encounter and applies its rewards or consequences.
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
                                encounter.Quest.RequiredEncounters = Math.Max(1, encounter.Quest.RequiredEncounters - StatCalculator.SocialBonus(player.Stats));
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
                    var gold = encounter.GoldReward + StatCalculator.RewardBonus(player.Stats);
                    player.Gold += gold;
                    Console.WriteLine($"You find {gold} gold.");
                    AddReward(player, encounter.ItemReward);
                    RecordQuestProgress(quests);
                    break;
                case EncounterType.Trap:
                    var damage = Math.Max(0, Dice.Roll(6) - StatCalculator.TrapReduction(player.Stats));
                    player.Health = Math.Max(0, player.Health - damage);
                    Console.WriteLine($"The trap injures you for {damage} damage.");
                    break;
            }

            CompleteQuests(quests, player);
        }

        // Advances all active quests after a successful encounter.
        private static void RecordQuestProgress(List<Quest> quests)
        {
            foreach (var quest in quests)
            {
                quest.RecordProgress();
            }
        }

        // Pays rewards for quests that became complete.
        private static void CompleteQuests(List<Quest> quests, PlayerCharacter player)
        {
            foreach (var quest in quests)
            {
                if (quest.Status != QuestStatus.Completed || quest.RewardsClaimed) continue;

                player.Gold += quest.GoldReward;
                var previousLevel = player.Level;
                player.AddExperience(quest.ExperienceReward);
                AddReward(player, quest.ItemReward);
                Console.WriteLine($"Quest complete: {quest.Title}. Reward: {quest.GoldReward} gold and {quest.ExperienceReward} XP.");
                if (player.Level > previousLevel)
                {
                    Console.WriteLine($"Level up! You are now level {player.Level} and gained 3 stat points.");
                }
                quest.GoldReward = 0;
                quest.ExperienceReward = 0;
                quest.RewardsClaimed = true;
            }
        }

        // Adds an optional item reward and reports it to the player.
        private static void AddReward(PlayerCharacter player, Item? item)
        {
            if (item == null) return;

            player.Inventory.Add(item);
            Console.WriteLine($"You acquire: {item.Name}.");
        }

        // Displays current and completed quest progress.
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

        // Announces arrival at a new location.
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

        // Displays and resolves the location's interactable objects.
        private static void InteractWithObject(PlayerCharacter player, WorldLocation location, List<Quest> quests)
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

            var serviceChoice = location.Interactions.Count + 1;
            Console.WriteLine($"{serviceChoice}) Use location services");
            Console.WriteLine("0) Back");
            Console.Write("Choose an interaction: ");
            var input = Console.ReadLine();
            if (!int.TryParse(input, out var choice) || choice < 0 || choice > serviceChoice)
            {
                Console.WriteLine("Invalid interaction.");
                Pause();
                return;
            }

            if (choice == 0)
            {
                return;
            }

            if (choice == serviceChoice)
            {
                UseLocationServices(player, location);
                return;
            }

            var interaction = location.Interactions[choice - 1];
            Console.Clear();
            Console.WriteLine(interaction.Name);
            Console.WriteLine(interaction.Description);
            Console.WriteLine();
            ResolveInteraction(player, location, interaction, quests);
            Pause();
        }

        // Applies the behavior associated with one selected interaction.
        private static void ResolveInteraction(PlayerCharacter player, WorldLocation location, WorldInteraction interaction, List<Quest> quests)
        {
            switch (interaction.Type)
            {
                case WorldInteractionType.Npc:
                    player.Morality++;
                    Console.WriteLine("You have a useful conversation and gain a better understanding of the area.");
                    break;
                case WorldInteractionType.Quest:
                    var trackedQuest = quests.Find(quest => quest.Id == interaction.Id);
                    if (trackedQuest != null)
                    {
                        Console.WriteLine($"This quest is already tracked: {trackedQuest.Title} [{trackedQuest.Status}].");
                        break;
                    }

                    quests.Add(new Quest
                    {
                        Id = interaction.Id,
                        Title = interaction.Name,
                        Description = interaction.Description,
                        RequiredEncounters = 1,
                        GoldReward = 15,
                        ExperienceReward = 40
                    });
                    player.Morality++;
                    Console.WriteLine("Quest accepted and added to your tracked quests.");
                    break;
                case WorldInteractionType.Service:
                    UseSpecificService(player, interaction);
                    break;
                case WorldInteractionType.Chest:
                    if (player.TryOpenChest(interaction.Id))
                    {
                        var gold = 10 + StatCalculator.RewardBonus(player.Stats) * 2;
                        player.Gold += gold;
                        Console.WriteLine($"You find {gold} gold inside.");
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
                    if (Dice.Roll(100) <= StatCalculator.DiscoveryChance(player.Stats))
                    {
                        player.Inventory.Add(new Item { Name = "Extra gathered materials", Rarity = ItemRarity.Common, Magic = 1, Durability = 1 });
                        Console.WriteLine("Your perception and luck help you find extra materials.");
                    }
                    break;
            }
        }

        // Handles a named service interaction such as resting or training.
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
                var cost = Math.Max(10, 25 * (100 - StatCalculator.ShopDiscountPercent(player.Stats)) / 100);
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

        // Opens the service menu appropriate to the current location.
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
            ShowLocationServiceOptions(location.Type);
            Console.Write("Choose a service: ");
            var choice = Console.ReadLine();
            if (choice == "0") return;

            switch (location.Type)
            {
                case LocationType.Tavern:
                    if (choice == "1") TavernRest(player);
                    else if (choice == "2") TavernRumors();
                    else InvalidServiceChoice();
                    break;
                case LocationType.Guild:
                    if (choice == "1") GuildContract(player);
                    else if (choice == "2") GuildTraining(player);
                    else InvalidServiceChoice();
                    break;
                case LocationType.Market:
                    if (choice == "1") MarketInteraction(player);
                    else InvalidServiceChoice();
                    break;
                case LocationType.Landmark:
                    if (choice == "1") LandmarkInteraction(player, location);
                    else InvalidServiceChoice();
                    break;
                case LocationType.Shrine:
                    if (choice == "1") ShrineInteraction(player);
                    else if (choice == "2") ShrineHistory();
                    else InvalidServiceChoice();
                    break;
                case LocationType.Dungeon:
                    if (choice == "1") DungeonInteraction(player, location);
                    else InvalidServiceChoice();
                    break;
                case LocationType.Town:
                    if (choice == "1") TownNews(player);
                    else InvalidServiceChoice();
                    break;
                default:
                    InvalidServiceChoice();
                    break;
            }

            Pause();
        }

        // Lists the services available at the current location without performing them automatically.
        private static void ShowLocationServiceOptions(LocationType locationType)
        {
            switch (locationType)
            {
                case LocationType.Tavern:
                    Console.WriteLine("1) Rest and recover");
                    Console.WriteLine("2) Hear rumors");
                    break;
                case LocationType.Guild:
                    Console.WriteLine("1) Complete a contract");
                    Console.WriteLine("2) Train with guild members");
                    break;
                case LocationType.Market:
                    Console.WriteLine("1) Browse the market");
                    break;
                case LocationType.Landmark:
                    Console.WriteLine("1) Explore the area");
                    break;
                case LocationType.Shrine:
                    Console.WriteLine("1) Pray for guidance and recovery");
                    Console.WriteLine("2) Learn local history");
                    break;
                case LocationType.Dungeon:
                    Console.WriteLine("1) Explore the dungeon entrance");
                    break;
                case LocationType.Town:
                    Console.WriteLine("1) Ask the townsfolk for news");
                    break;
            }

            Console.WriteLine("0) Back");
        }

        // Resolves the tavern's rest service.
        private static void TavernRest(PlayerCharacter player)
        {
            player.Health = player.MaxHealth;
            Console.WriteLine("You rest fully and recover all health.");
        }

        // Resolves the tavern's rumor service.
        private static void TavernRumors()
        {
            Console.WriteLine("You speak with the tavern keeper.");
            Console.WriteLine("Rumor: A sealed road near the Old Watchtower has recently reopened.");
        }

        // Resolves a guild contract.
        private static void GuildContract(PlayerCharacter player)
        {
            Console.WriteLine("The guild clerk offers you a simple contract.");
            player.Gold += 20;
            Console.WriteLine("You complete a training contract and earn 20 gold.");
        }

        // Resolves guild training.
        private static void GuildTraining(PlayerCharacter player)
        {
            player.MaxHealth++;
            player.Health = Math.Min(player.MaxHealth, player.Health + 1);
            Console.WriteLine("Guild training permanently increases your maximum health by 1.");
        }

        // Resolves the market's item and merchant options.
        private static void MarketInteraction(PlayerCharacter player)
        {
            ShopSystem.Open(player);
        }

        // Resolves exploration options for a landmark location.
        private static void LandmarkInteraction(PlayerCharacter player, WorldLocation location)
        {
            var roll = Dice.Roll() + StatCalculator.DiscoveryChance(player.Stats) / 10;
            Console.WriteLine($"You investigate {location.Name}. Exploration roll: {roll}");
            if (roll >= 12)
            {
                var gold = 15 + StatCalculator.RewardBonus(player.Stats) * 2;
                player.Gold += gold;
                Console.WriteLine($"You discover an old traveler's cache containing {gold} gold.");
            }
            else
            {
                player.Health = Math.Max(1, player.Health - 2);
                Console.WriteLine("A hidden hazard injures you for 2 health.");
            }
        }

        // Resolves the shrine's guidance and recovery options.
        private static void ShrineInteraction(PlayerCharacter player)
        {
            player.Health = player.MaxHealth;
            player.Morality++;
            Console.WriteLine("You pray at the shrine. Your wounds close and your resolve strengthens.");
            Console.WriteLine("Health restored and morality increased by 1.");
        }

        // Shares local history without consuming the shrine's recovery service.
        private static void ShrineHistory()
        {
            Console.WriteLine("The shrine keeper tells stories of the Ember Star and the first settlers.");
        }

        // Shares local news in a town.
        private static void TownNews(PlayerCharacter player)
        {
            Console.WriteLine("The townsfolk welcome you. Ask around for local news.");
            player.Morality++;
        }

        private static void InvalidServiceChoice()
        {
            Console.WriteLine("Invalid service choice.");
        }

        // Starts the dungeon or gauntlet associated with the location.
        private static void DungeonInteraction(PlayerCharacter player, WorldLocation location)
        {
            var roll = Dice.Roll() + StatCalculator.InitiativeBonus(player.Stats) + StatCalculator.AccuracyBonus(player.Stats) / 2;
            Console.WriteLine($"You enter {location.Name}. Expedition roll: {roll}");
            if (roll >= 10)
            {
                var gold = 30 + StatCalculator.RewardBonus(player.Stats) * 2;
                player.Gold += gold;
                player.Inventory.Add(ItemFactory.CreateWeapon(WeaponCategory.Dagger, Material.Bronze, ItemRarity.Uncommon));
                Console.WriteLine($"You defeat a lurking creature and recover {gold} gold and an Uncommon Bronze Dagger.");
            }
            else
            {
                player.Health = Math.Max(1, player.Health - 5);
                Console.WriteLine("The dungeon fights back. You escape with 5 health damage.");
            }
        }

        // Waits for acknowledgement before returning to the previous menu.
        private static void Pause()
        {
            Console.WriteLine();
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }
    }
}
