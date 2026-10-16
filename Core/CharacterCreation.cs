using System;
using NewWorldPP2.Entities;
using NewWorldPP2.Game.Systems;

namespace NewWorldPP2.Core
{
    public static class CharacterCreation
    {
        private const int PointsToDistribute = 20;

        public static PlayerCharacter CreateCharacter()
        {
            var pc = new PlayerCharacter();
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
            pc.Class = CharacterCustomization.ChooseClass();
            pc.StartingWeapon = CharacterCustomization.ChooseWeapon(pc.Class);
            pc.StartingArmor = CharacterCustomization.ChooseArmor(pc.Class);

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

        public static void CustomizeCharacter(PlayerCharacter pc)
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
                        pc.Class = CharacterCustomization.ChooseClass();
                        break;
                    case "6":
                        pc.StartingWeapon = CharacterCustomization.ChooseWeapon(pc.Class);
                        break;
                    case "7":
                        pc.StartingArmor = CharacterCustomization.ChooseArmor(pc.Class);
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

                if (remaining > 0)
                {
                    Console.WriteLine($"You still have {remaining} points remaining. Distribute all points before continuing.");
                    Console.WriteLine("Press Enter to continue and reassign remaining points...");
                    Console.ReadLine();
                    Console.Clear();
                    continue;
                }

                if (remaining < 0)
                {
                    Console.WriteLine("You allocated too many points. Start over. Press Enter...");
                    Console.ReadLine();
                    Console.Clear();
                    // reset
                    s.Strength = s.Dexterity = s.Constitution = s.Intelligence = s.Wisdom = s.Charisma = 1;
                    remaining = totalPoints;
                    continue;
                }

                break;
            }

            return s;
        }

        private static int PromptAddToStat(string statName, int statValue, out int newStatValue, int remaining)
        {
            while (true)
            {
                Console.Write($"Add points to {statName} (current {statValue}): ");
                string? input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    newStatValue = statValue;
                    return remaining;
                }
                if (!int.TryParse(input.Trim(), out var add) || add < 0)
                {
                    Console.WriteLine("Invalid number. Enter 0 or a positive integer.");
                    continue;
                }
                if (add > remaining)
                {
                    Console.WriteLine($"Not enough points. You have {remaining} remaining.");
                    continue;
                }
                newStatValue = statValue + add;
                return remaining - add;
            }
        }

        private static void ApplyRaceModifiers(PlayerCharacter pc)
        {
            // Start from the base allocated stats and apply modifiers to produce final Stats.
            var baseStats = pc.BaseStats ?? new Stats();
            var final = new Stats
            {
                Strength = baseStats.Strength,
                Dexterity = baseStats.Dexterity,
                Constitution = baseStats.Constitution,
                Intelligence = baseStats.Intelligence,
                Wisdom = baseStats.Wisdom,
                Charisma = baseStats.Charisma,
            };

            switch (pc.Race)
            {
                case Race.Human:
                    final.Strength += 1;
                    final.Dexterity += 1;
                    final.Constitution += 1;
                    final.Intelligence += 1;
                    final.Wisdom += 1;
                    final.Charisma += 1;
                    break;
                case Race.Elf:
                    final.Dexterity += 2;
                    final.Constitution = Math.Max(1, final.Constitution - 1);
                    break;
                case Race.Dwarf:
                    final.Constitution += 2;
                    final.Charisma = Math.Max(1, final.Charisma - 1);
                    break;
                case Race.Orc:
                    final.Strength += 2;
                    final.Intelligence = Math.Max(1, final.Intelligence - 1);
                    break;
            }

            pc.Stats = final;
            pc.RecalculateDerivedStats();
        }

        private static string ReadMultiline()
        {
            string? line;
            string result = string.Empty;
            while (true)
            {
                line = Console.ReadLine();
                if (string.IsNullOrEmpty(line)) break;
                if (result.Length > 0) result += "\n";
                result += line;
            }
            return result;
        }

        // Validate character names: non-empty, reasonable length, and allowed characters
        private static bool IsValidName(string name, out string error)
        {
            error = string.Empty;
            if (string.IsNullOrWhiteSpace(name))
            {
                error = "Name is required and cannot be blank.";
                return false;
            }
            if (name.Length < 2 || name.Length > 20)
            {
                error = "Name must be between 2 and 20 characters.";
                return false;
            }
            foreach (var c in name)
            {
                if (!(char.IsLetterOrDigit(c) || c == ' ' || c == '-' || c == '\''))
                {
                    error = "Name contains invalid characters. Use letters, numbers, spaces, hyphens or apostrophes.";
                    return false;
                }
            }
            return true;
        }
    }
}
