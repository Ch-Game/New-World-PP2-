using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

// Consolidated single-file version of the project's code.

namespace NewWorldPP2.Entities
{
    public enum Race
    {
        Human = 1,
        Elf,
        Dwarf,
        Orc
    }

    public enum CharacterClass
    {
        Undefined = 0,
        Brawler = 1,
        Magian,
        Swordsman,
        Monk,
        Archer,
        Rogue
    }

    public enum WeaponType
    {
        None = 0,
        Bow,
        Sword,
        Staff,
        Dagger,
        MagicGun,
        Gauntlets
    }

    public enum ArmorType
    {
        None = 0,
        Robes,
        Chestplate,
        Tunic
    }

    public class Stats
    {
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Constitution { get; set; }
        public int Intelligence { get; set; }
        public int Wisdom { get; set; }
        public int Charisma { get; set; }

        public int Total() => Strength + Dexterity + Constitution + Intelligence + Wisdom + Charisma;

        public override string ToString()
        {
            return $"STR: {Strength}  DEX: {Dexterity}  CON: {Constitution}  INT: {Intelligence}  WIS: {Wisdom}  CHA: {Charisma}";
        }
    }

    public class PlayerCharacter
    {
        public string Name { get; set; } = string.Empty;
        public Race Race { get; set; }
        public CharacterClass Class { get; set; } = CharacterClass.Undefined;
        public WeaponType StartingWeapon { get; set; } = WeaponType.None;
        public ArmorType StartingArmor { get; set; } = ArmorType.None;
        // BaseStats represent the allocated points before racial modifiers.
        public Stats BaseStats { get; set; } = new Stats();
        // Stats reflect BaseStats plus race modifiers.
        public Stats Stats { get; set; } = new Stats();
        public string Backstory { get; set; } = string.Empty;
        public int MaxHealth { get; set; } = 10;
        public int Health { get; set; } = 10;
        public int Morality { get; set; } = 0;

        public void RecalculateDerivedStats()
        {
            // Simple derived stat: MaxHealth scales with Constitution
            MaxHealth = 10 + (BaseStats?.Constitution ?? Stats.Constitution) * 2;
            if (Health > MaxHealth) Health = MaxHealth;
        }

        public string GetShortDescription()
        {
            var cls = Class == CharacterClass.Undefined ? "No Class" : Class.ToString();
            var weapon = StartingWeapon == WeaponType.None ? "No Weapon" : StartingWeapon.ToString();
            var armor = StartingArmor == ArmorType.None ? "No Armor" : StartingArmor.ToString();
            return $"{Name} the {Race} ({cls}) - {Stats} (HP {Health}/{MaxHealth})\nWeapon: {weapon}  Armor: {armor}";
        }

        public string GetFullDescription()
        {
            var cls = Class == CharacterClass.Undefined ? "None" : Class.ToString();
            var weapon = StartingWeapon == WeaponType.None ? "None" : StartingWeapon.ToString();
            var armor = StartingArmor == ArmorType.None ? "None" : StartingArmor.ToString();
            return $"Name: {Name}\nClass: {cls}\nRace: {Race}\nStats: {Stats}\nHP: {Health}/{MaxHealth}\nWeapon: {weapon}\nArmor: {armor}\nMorality: {Morality}\nBackstory:\n{Backstory}";
        }
    }

    public enum ItemRarity { Common = 1, Uncommon, Rare, Epic, Legendary, Mythical }

    public enum WeaponCategory { Sword, Bow, Staff, Dagger, Axe, Spear, Mace, MagicGun, Gauntlets }
    public enum Material { Wood, Bronze, Iron, Steel, Obsidian, Mythril, Adamantine }
    public enum ArmorCategory { Robes, Tunic, Leather, Chainmail, Chestplate, Plate }
    public enum PotionType { Health, Mana, Stamina, Strength, Invisibility, Revival }

    public class Ability
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class Item
    {
        public string Name { get; set; } = string.Empty;
        public ItemRarity Rarity { get; set; } = ItemRarity.Common;
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int Magic { get; set; }
        public int Speed { get; set; }
        public int Durability { get; set; }
        // Only mythical items will have abilities populated by factory
        public List<Ability>? Abilities { get; set; }

        public override string ToString()
        {
            var ab = Abilities == null ? "" : $" Abilities: {Abilities.Count}";
            return $"{Name} ({Rarity}) A:{Attack} D:{Defense} M:{Magic} S:{Speed} Dur:{Durability}{ab}";
        }
    }

    public static class ItemFactory
    {
        // Lists used by game to spawn or reward items
        public static readonly WeaponCategory[] Weapons = (WeaponCategory[])Enum.GetValues(typeof(WeaponCategory));
        public static readonly Material[] Materials = (Material[])Enum.GetValues(typeof(Material));
        public static readonly ArmorCategory[] Armors = (ArmorCategory[])Enum.GetValues(typeof(ArmorCategory));
        public static readonly PotionType[] Potions = (PotionType[])Enum.GetValues(typeof(PotionType));

        // Create a weapon instance with stats derived from category, material and rarity
        public static Item CreateWeapon(WeaponCategory cat, Material mat, ItemRarity rarity)
        {
            var baseAtk = cat switch
            {
                WeaponCategory.Sword => 6,
                WeaponCategory.Bow => 5,
                WeaponCategory.Staff => 3,
                WeaponCategory.Dagger => 4,
                WeaponCategory.Axe => 7,
                WeaponCategory.Spear => 6,
                WeaponCategory.Mace => 7,
                WeaponCategory.MagicGun => 5,
                WeaponCategory.Gauntlets => 4,
                _ => 3
            };

            var matMul = mat switch
            {
                Material.Wood => 0.8,
                Material.Bronze => 0.95,
                Material.Iron => 1.0,
                Material.Steel => 1.15,
                Material.Obsidian => 1.25,
                Material.Mythril => 1.5,
                Material.Adamantine => 1.75,
                _ => 1.0
            };

            var rarityMul = RarityMultiplier(rarity);

            var itm = new Item
            {
                Name = $"{rarity} {mat} {cat}",
                Rarity = rarity,
                Attack = (int)Math.Round(baseAtk * matMul * rarityMul),
                Defense = 0,
                Magic = cat == WeaponCategory.Staff || cat == WeaponCategory.MagicGun ? (int)Math.Round(3 * rarityMul) : 0,
                Speed = cat == WeaponCategory.Dagger || cat == WeaponCategory.Bow ? 2 : 0,
                Durability = (int)Math.Round(30 * matMul * rarityMul)
            };

            if (rarity == ItemRarity.Mythical)
            {
                itm.Abilities = MythicalAbilitiesForWeapon(cat);
            }

            return itm;
        }

        public static Item CreateArmor(ArmorCategory cat, Material mat, ItemRarity rarity)
        {
            var baseDef = cat switch
            {
                ArmorCategory.Robes => 1,
                ArmorCategory.Tunic => 3,
                ArmorCategory.Leather => 4,
                ArmorCategory.Chainmail => 6,
                ArmorCategory.Chestplate => 8,
                ArmorCategory.Plate => 10,
                _ => 2
            };

            var matMul = mat switch
            {
                Material.Wood => 0.7,
                Material.Bronze => 0.9,
                Material.Iron => 1.0,
                Material.Steel => 1.2,
                Material.Obsidian => 1.3,
                Material.Mythril => 1.6,
                Material.Adamantine => 1.9,
                _ => 1.0
            };

            var rarityMul = RarityMultiplier(rarity);

            var itm = new Item
            {
                Name = $"{rarity} {mat} {cat}",
                Rarity = rarity,
                Attack = 0,
                Defense = (int)Math.Round(baseDef * matMul * rarityMul),
                Magic = cat == ArmorCategory.Robes ? (int)Math.Round(2 * rarityMul) : 0,
                Speed = cat == ArmorCategory.Robes || cat == ArmorCategory.Tunic ? 1 : 0,
                Durability = (int)Math.Round(40 * matMul * rarityMul)
            };

            if (rarity == ItemRarity.Mythical)
            {
                itm.Abilities = MythicalAbilitiesForArmor(cat);
            }

            return itm;
        }

        public static Item CreatePotion(PotionType type, ItemRarity rarity)
        {
            var potency = type switch
            {
                PotionType.Health => 20,
                PotionType.Mana => 15,
                PotionType.Stamina => 12,
                PotionType.Strength => 10,
                PotionType.Invisibility => 0,
                PotionType.Revival => 0,
                _ => 10
            };

            var rarityMul = RarityMultiplier(rarity);

            var itm = new Item
            {
                Name = $"{rarity} {type} Potion",
                Rarity = rarity,
                Attack = 0,
                Defense = 0,
                Magic = (int)Math.Round(potency * rarityMul),
                Speed = 0,
                Durability = 1
            };

            if (rarity == ItemRarity.Mythical)
            {
                itm.Abilities = MythicalAbilitiesForPotion(type);
            }

            return itm;
        }

        private static double RarityMultiplier(ItemRarity r) => r switch
        {
            ItemRarity.Common => 1.0,
            ItemRarity.Uncommon => 1.1,
            ItemRarity.Rare => 1.25,
            ItemRarity.Epic => 1.5,
            ItemRarity.Legendary => 1.8,
            ItemRarity.Mythical => 2.5,
            _ => 1.0
        };

        //weapon abilities for mythical items
        private static List<Ability> MythicalAbilitiesForWeapon(WeaponCategory cat)
        {
            return cat switch
            {
                WeaponCategory.Sword => new List<Ability> { new Ability { Name = "Flameburst", Description = "Occasionally deals extra fire damage." } },
                WeaponCategory.Bow => new List<Ability> { new Ability { Name = "Piercing Shot", Description = "Arrows ignore a portion of enemy armor." } },
                WeaponCategory.Staff => new List<Ability> { new Ability { Name = "Arcane Surge", Description = "Boosts spell potency when equipped." } },
                WeaponCategory.Dagger => new List<Ability> { new Ability { Name = "Shadowstep", Description = "Chance to teleport behind the enemy on hit." } },
                WeaponCategory.MagicGun => new List<Ability> { new Ability { Name = "Mana Bullet", Description = "Shots consume mana for extra effect." } },
                _ => new List<Ability> { new Ability { Name = "Eternal Edge", Description = "Slight chance to instantly slay weaker foes." } }
            };
        }

        private static List<Ability> MythicalAbilitiesForArmor(ArmorCategory cat)
        {
            return cat switch
            {
                ArmorCategory.Robes => new List<Ability> { new Ability { Name = "Spell Ward", Description = "Reduces incoming magic damage." } },
                ArmorCategory.Chestplate => new List<Ability> { new Ability { Name = "Bulwark", Description = "Chance to block heavy hits completely." } },
                _ => new List<Ability> { new Ability { Name = "Aegis", Description = "Small passive regen when out of combat." } }
            };
        }

        private static List<Ability> MythicalAbilitiesForPotion(PotionType type)
        {
            return type switch
            {
                PotionType.Health => new List<Ability> { new Ability { Name = "Rejuvenation", Description = "Fully restores health and grants temporary regen." } },
                PotionType.Invisibility => new List<Ability> { new Ability { Name = "True Invisibility", Description = "Hides the player from detection for longer." } },
                PotionType.Revival => new List<Ability> { new Ability { Name = "Phoenix Rise", Description = "Revives the player with a portion of max health." } },
                _ => new List<Ability> { new Ability { Name = "Empowered", Description = "Potion effects are greatly amplified." } }
            };
        }
    }
}

namespace NewWorldPP2.Core
{
    using NewWorldPP2.Entities;
    public static class Dice
    {
        private static readonly Random _rng = new();

        // Roll an n-sided dice (default d20)
        public static int Roll(int sides = 20)
        {
            if (sides < 2) return 1;
            return _rng.Next(1, sides + 1);
        }
    }

    // Saves and related helpers (keeps file-per-user JSON with SHA-256 hashing)
    public static class SaveSystem
    {
        private const string SaveFolder = "Saves";

        private sealed class SaveData
        {
            public string Username { get; set; } = string.Empty;
            public string PasswordHash { get; set; } = string.Empty;
            public NewWorldPP2.Entities.PlayerCharacter? Character { get; set; }
        }

        public static bool SaveCharacter(NewWorldPP2.Entities.PlayerCharacter pc, string username, string password)
        {
            if (pc == null) return false;
            if (string.IsNullOrWhiteSpace(pc.Name) || string.IsNullOrWhiteSpace(username)) return false;

            try
            {
                Directory.CreateDirectory(SaveFolder);
                var file = Path.Combine(SaveFolder, Sanitize(username) + ".json");
                if (File.Exists(file)) return false; // do not overwrite

                var data = new SaveData
                {
                    Username = username,
                    PasswordHash = ComputeHash(username, password),
                    Character = pc
                };

                var opts = new JsonSerializerOptions { WriteIndented = true };
                opts.Converters.Add(new JsonStringEnumConverter());
                File.WriteAllText(file, JsonSerializer.Serialize(data, opts), Encoding.UTF8);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool SaveExists(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return false;
            var file = Path.Combine(SaveFolder, Sanitize(username) + ".json");
            return File.Exists(file);
        }

        public static NewWorldPP2.Entities.PlayerCharacter? LoadCharacter(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username)) return null;

            try
            {
                var file = Path.Combine(SaveFolder, Sanitize(username) + ".json");
                if (!File.Exists(file)) return null;

                var opts = new JsonSerializerOptions();
                opts.Converters.Add(new JsonStringEnumConverter());
                var json = File.ReadAllText(file, Encoding.UTF8);
                var data = JsonSerializer.Deserialize<SaveData>(json, opts);
                if (data == null) return null;

                var expected = ComputeHash(data.Username ?? string.Empty, password);
                if (!string.Equals(expected, data.PasswordHash ?? string.Empty, StringComparison.OrdinalIgnoreCase)) return null;

                var character = data.Character;
                if (character == null) return null;

                character.RecalculateDerivedStats();
                return character;
            }
            catch
            {
                return null;
            }
        }

        private static string Sanitize(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return string.Empty;
            var invalid = Path.GetInvalidFileNameChars();
            var sb = new StringBuilder(username.Length);
            foreach (var c in username)
            {
                sb.Append(Array.IndexOf(invalid, c) >= 0 ? '_' : c);
            }
            return sb.ToString().ToLowerInvariant();
        }

        private static string ComputeHash(string username, string password)
        {
            using var sha = SHA256.Create();
            var combined = (username ?? string.Empty) + ":" + (password ?? string.Empty);
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(combined));
            var sb = new StringBuilder(hash.Length * 2);
            foreach (var b in hash) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }

    public static class CharacterCreation
    {
        private const int PointsToDistribute = 20;

        public static NewWorldPP2.Entities.PlayerCharacter CreateCharacter()
        {
            var pc = new NewWorldPP2.Entities.PlayerCharacter();
            Console.Clear();
            Console.WriteLine("--- Create New Character ---");
            // Require a valid name (non-empty, length and character checks)
            while (true)
            {
                Console.Write("Enter character name: ");
                var input = Console.ReadLine()?.Trim() ?? string.Empty;
                if (IsValidName(input, out var err))
                {
                    pc.Name = input;
                    break;
                }
                Console.WriteLine(err + " Press Enter to try again...");
                Console.ReadLine();
                Console.Clear();
            }

            pc.Race = ChooseRace();

            // Allow the player to choose a class and starting gear via the CharacterCustomization system
            pc.Class = NewWorldPP2.Game.Systems.CharacterCustomization.ChooseClass();
            pc.StartingWeapon = NewWorldPP2.Game.Systems.CharacterCustomization.ChooseWeapon(pc.Class);
            pc.StartingArmor = NewWorldPP2.Game.Systems.CharacterCustomization.ChooseArmor(pc.Class);

            Console.WriteLine($"You have {PointsToDistribute} points to distribute among six stats.");
            pc.BaseStats = DistributePoints(PointsToDistribute);

            ApplyRaceModifiers(pc);

            Console.WriteLine("Write a short backstory (press Enter twice to finish):");
            pc.Backstory = ReadMultiline();

            Console.WriteLine();
            Console.WriteLine("Character created:");
            Console.WriteLine(pc.GetFullDescription());
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
            Console.Clear();

            return pc;
        }

        public static void CustomizeCharacter(NewWorldPP2.Entities.PlayerCharacter pc)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("--- Customize Character ---");
                Console.WriteLine("1) Change Name");
                Console.WriteLine("2) Change Race");
                Console.WriteLine("3) Reassign Stats");
                Console.WriteLine("4) Edit Backstory");
                Console.WriteLine("5) Change Class");
                Console.WriteLine("6) Change Starting Weapon");
                Console.WriteLine("7) Change Starting Armor");
                Console.WriteLine("8) Back");
                Console.WriteLine();
                Console.WriteLine(pc.GetShortDescription());
                Console.Write("Choose option: ");
                string? key = Console.ReadLine();

                switch (key)
                {
                    case "1":
                        // Re-prompt until a valid name is entered
                        while (true)
                        {
                            Console.Write($"New name (current: {pc.Name}): ");
                            var newName = Console.ReadLine()?.Trim() ?? string.Empty;
                            if (IsValidName(newName, out var nameErr))
                            {
                                pc.Name = newName;
                                break;
                            }
                            Console.WriteLine(nameErr + " Press Enter to try again...");
                            Console.ReadLine();
                            Console.Clear();
                        }
                        break;
                    case "2":
                        pc.Race = ChooseRace();
                        ApplyRaceModifiers(pc);
                        break;
                    case "3":
                        pc.BaseStats = DistributePoints(PointsToDistribute);
                        ApplyRaceModifiers(pc);
                        break;
                    case "4":
                        Console.WriteLine("Enter backstory (press Enter twice to finish):");
                        pc.Backstory = ReadMultiline();
                        break;
                    case "5":
                        pc.Class = NewWorldPP2.Game.Systems.CharacterCustomization.ChooseClass();
                        break;
                    case "6":
                        pc.StartingWeapon = NewWorldPP2.Game.Systems.CharacterCustomization.ChooseWeapon(pc.Class);
                        break;
                    case "7":
                        pc.StartingArmor = NewWorldPP2.Game.Systems.CharacterCustomization.ChooseArmor(pc.Class);
                        break;
                    case "8":
                        return;
                        default:
                            Console.WriteLine("Invalid option. Press Enter to continue...");
                            Console.ReadLine();
                            Console.Clear();
                            break;
                }
            }
        }

        private static Race ChooseRace()
        {
            while (true)
            {
                Console.WriteLine("Choose a race:");
                Console.WriteLine("1) Human (+1 all stats)");
                Console.WriteLine("2) Elf (+2 Dexterity, -1 Constitution)");
                Console.WriteLine("3) Dwarf (+2 Constitution, -1 Charisma)");
                Console.WriteLine("4) Orc (+2 Strength, -1 Intelligence)");
                Console.Write("Selection: ");
                string? input = Console.ReadLine();
                if (int.TryParse(input, out var val) && val >= 1 && val <= 4)
                {
                    return (Race)val;
                }
                Console.WriteLine("Invalid selection, try again.");
            }
        }

        private static Stats DistributePoints(int totalPoints)
        {
            var s = new Stats();
            // Start with 1 in each stat so none are zero
            s.Strength = 1; s.Dexterity = 1; s.Constitution = 1; s.Intelligence = 1; s.Wisdom = 1; s.Charisma = 1;
            int remaining = totalPoints;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Distribute your points:");
                Console.WriteLine($"Remaining points: {remaining}");
                Console.WriteLine(s.ToString());
                Console.WriteLine();
                Console.WriteLine("Enter values to add to each stat. Leave blank to skip or enter 0.");

                int newVal;
                remaining = PromptAddToStat("Strength", s.Strength, out newVal, remaining);
                s.Strength = newVal;
                remaining = PromptAddToStat("Dexterity", s.Dexterity, out newVal, remaining);
                s.Dexterity = newVal;
                remaining = PromptAddToStat("Constitution", s.Constitution, out newVal, remaining);
                s.Constitution = newVal;

                remaining = PromptAddToStat("Intelligence", s.Intelligence, out newVal, remaining);
                s.Intelligence = newVal;
                remaining = PromptAddToStat("Wisdom", s.Wisdom, out newVal, remaining);
                s.Wisdom = newVal;
                remaining = PromptAddToStat("Charisma", s.Charisma, out newVal, remaining);
                s.Charisma = newVal;

                if (remaining < 0)
                {
                    Console.WriteLine("You allocated too many points. Press Enter to try again...");
                    Console.ReadLine();
                    continue;
                }

                Console.WriteLine($"Allocation complete. Remaining: {remaining}. Accept? (y/n)");
                var ans = Console.ReadLine()?.Trim().ToLowerInvariant() ?? "";
                if (ans == "y" || ans == "yes") break;
            }

            return s;
        }

        private static int PromptAddToStat(string name, int current, out int newVal, int remaining)
        {
            Console.Write($"{name} (current {current}) add: ");
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                newVal = current;
                return remaining;
            }

            if (!int.TryParse(input.Trim(), out var add))
            {
                newVal = current;
                return remaining;
            }

            newVal = current + add;
            return remaining - add;
        }

        private static void ApplyRaceModifiers(NewWorldPP2.Entities.PlayerCharacter pc)
        {
            // Apply simple modifiers to BaseStats to compute Stats
            pc.Stats = new Stats
            {
                Strength = pc.BaseStats.Strength,
                Dexterity = pc.BaseStats.Dexterity,
                Constitution = pc.BaseStats.Constitution,
                Intelligence = pc.BaseStats.Intelligence,
                Wisdom = pc.BaseStats.Wisdom,
                Charisma = pc.BaseStats.Charisma
            };

            switch (pc.Race)
            {
                case Race.Human:
                    pc.Stats.Strength += 1; pc.Stats.Dexterity += 1; pc.Stats.Constitution += 1;
                    pc.Stats.Intelligence += 1; pc.Stats.Wisdom += 1; pc.Stats.Charisma += 1;
                    break;
                case Race.Elf:
                    pc.Stats.Dexterity += 2; pc.Stats.Constitution -= 1;
                    break;
                case Race.Dwarf:
                    pc.Stats.Constitution += 2; pc.Stats.Charisma -= 1;
                    break;
                case Race.Orc:
                    pc.Stats.Strength += 2; pc.Stats.Intelligence -= 1;
                    break;
            }

            pc.RecalculateDerivedStats();
        }

        private static string ReadMultiline()
        {
            var sb = new StringBuilder();
            string? line;
            int emptyCount = 0;
            while ((line = Console.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    emptyCount++;
                    if (emptyCount >= 2) break;
                }
                else
                {
                    emptyCount = 0;
                    sb.AppendLine(line);
                }
            }
            return sb.ToString().TrimEnd();
        }

        private static bool IsValidName(string input, out string error)
        {
            error = string.Empty;
            if (string.IsNullOrWhiteSpace(input)) { error = "Name cannot be empty."; return false; }
            if (input.Length < 2 || input.Length > 24) { error = "Name must be between 2 and 24 characters."; return false; }
            foreach (var c in input)
            {
                if (!char.IsLetter(c) && c != ' ' && c != '-') { error = "Name contains invalid characters."; return false; }
            }
            return true;
        }
    }
}

namespace NewWorldPP2.World
{
    using NewWorldPP2.Core;
    using NewWorldPP2.Entities;

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
            var roll = NewWorldPP2.Core.Dice.Roll();
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

namespace NewWorldPP2.Game.Systems
{
    using NewWorldPP2.Entities;

    public static class CharacterCustomization
    {
        public static CharacterClass ChooseClass()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Choose a class:");
                Console.WriteLine("1) Brawler - melee brawler, uses Gauntlets or Dagger");
                Console.WriteLine("2) Magian - magic user, prefers Staff or MagicGun");
                Console.WriteLine("3) Swordsman - skilled with swords");
                Console.WriteLine("4) Monk - agile unarmed specialist");
                Console.WriteLine("5) Archer - ranged with bow");
                Console.WriteLine("6) Rogue - stealthy, favors daggers");
                Console.Write("Selection: ");
                string? input = Console.ReadLine();
                if (int.TryParse(input, out var val) && val >= 1 && val <= 6)
                {
                    return (CharacterClass)val;
                }
                Console.WriteLine("Invalid selection, try again. Press Enter...");
                Console.ReadLine();
            }
        }

        public static WeaponType ChooseWeapon(CharacterClass cls)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Choose a starting weapon (class: {cls}):");
                Console.WriteLine("1) Bow");
                Console.WriteLine("2) Sword");
                Console.WriteLine("3) Staff");
                Console.WriteLine("4) Dagger");
                Console.WriteLine("5) Magic Gun");
                Console.WriteLine("6) Gauntlets");
                Console.Write("Selection: ");
                string? input = Console.ReadLine();
                if (int.TryParse(input, out var val) && val >= 1 && val <= 6)
                {
                    return (WeaponType)val;
                }
                Console.WriteLine("Invalid selection, try again. Press Enter...");
                Console.ReadLine();
            }
        }

        public static ArmorType ChooseArmor(CharacterClass cls)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Choose a starting armor (class: {cls}):");
                Console.WriteLine("1) Robes");
                Console.WriteLine("2) Chestplate");
                Console.WriteLine("3) Tunic");
                Console.Write("Selection: ");
                string? input = Console.ReadLine();
                if (int.TryParse(input, out var val) && val >= 1 && val <= 3)
                {
                    return (ArmorType)val;
                }
                Console.WriteLine("Invalid selection, try again. Press Enter...");
                Console.ReadLine();
            }
        }
    }
}

namespace NewWorldPP2
{
    using NewWorldPP2.Core;
    using NewWorldPP2.Entities;

    class Program
    {
        static void Main(string[] args)
        {
            PlayerCharacter? player = null;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Character Menu ===");
                Console.WriteLine("1) Create New Character");
                Console.WriteLine("2) Customize Character");
                Console.WriteLine("3) View Character");
                Console.WriteLine("4) Save Character");
                Console.WriteLine("5) Load Character");
                Console.WriteLine("6) Exit");
                Console.Write("Choose an option: ");
                var key = Console.ReadLine() ?? string.Empty;

                switch (key)
                {
                    case "1":
                        player = CharacterCreation.CreateCharacter();
                        break;
                    case "2":
                        if (player == null)
                        {
                            Console.WriteLine("No character yet. Press Enter to continue...");
                            Console.ReadLine();
                            Console.Clear();
                        }
                        else
                        {
                            CharacterCreation.CustomizeCharacter(player);
                        }
                        break;
                    case "3":
                        if (player == null)
                        {
                            Console.WriteLine("No character yet. Press Enter to continue...");
                            Console.ReadLine();
                            Console.Clear();
                        }
                        else
                        {
                            Console.WriteLine(player.GetFullDescription());
                            Console.WriteLine("Press Enter to continue...");
                            Console.ReadLine();
                            Console.Clear();
                        }
                        break;
                    case "4":
                        if (player == null)
                        {
                            Console.WriteLine("No character to save. Press Enter to continue...");
                            Console.ReadLine();
                            Console.Clear();
                        }
                        else
                        {
                            Console.Write("Enter save username: ");
                            var user = Console.ReadLine()?.Trim() ?? string.Empty;
                            Console.Write("Enter password: ");
                            var pass = ReadPassword();
                            if (SaveSystem.SaveExists(user))
                            {
                                Console.WriteLine("A save with that username already exists. Choose a different username.");
                            }
                            else
                            {
                                var ok = SaveSystem.SaveCharacter(player, user, pass);
                                Console.WriteLine(ok ? "Saved successfully." : "Save failed.");
                            }
                            Console.WriteLine("Press Enter to continue...");
                            Console.ReadLine();
                            Console.Clear();
                        }
                        break;
                    case "5":
                        Console.Write("Enter save username: ");
                        var loadUser = Console.ReadLine()?.Trim() ?? string.Empty;
                        Console.Write("Enter password: ");
                        var loadPass = ReadPassword();
                        var loaded = SaveSystem.LoadCharacter(loadUser, loadPass);
                        if (loaded == null)
                        {
                            Console.WriteLine("Failed to load save (incorrect user/password or missing save). Press Enter...");
                            Console.ReadLine();
                            Console.Clear();
                        }
                        else
                        {
                            player = loaded;
                            Console.WriteLine("Save loaded. Press Enter to continue...");
                            Console.ReadLine();
                            Console.Clear();
                        }
                        break;
                    case "6":
                        return;
                        default:
                            Console.WriteLine("Invalid option. Press Enter to continue...");
                            Console.ReadLine();
                            Console.Clear();
                            break;
                }
            }
        }

        // Minimal password masking for console input
        private static string ReadPassword()
        {
            var pass = string.Empty;
            ConsoleKeyInfo key;
            while ((key = Console.ReadKey(true)).Key != ConsoleKey.Enter)
            {
                if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
                {
                    pass = pass[..^1];
                    Console.Write("\b \b");
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    pass += key.KeyChar;
                    Console.Write("*");
                }
            }
            Console.WriteLine();
            return pass;
        }
    }
}
