using System;
using System.Text;
using NewWorldPP2.Entities;

namespace NewWorldPP2.UI
{
    // Guides the player through creating and editing a character.
    public static class CharacterCreation
    {
        private const int PointsToDistribute = 35;

        // Collects the required identity, choices, statistics, and backstory.
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
            pc.Class = NewWorldPP2.UI.CharacterCustomization.ChooseClass();
            pc.StartingWeapon = NewWorldPP2.UI.CharacterCustomization.ChooseWeapon(pc.Class);
            pc.StartingArmor = NewWorldPP2.UI.CharacterCustomization.ChooseArmor(pc.Class);

            Console.WriteLine($"You have {PointsToDistribute} points to distribute among nine stats.");
            pc.BaseStats = DistributePoints(PointsToDistribute);

            ApplyRaceModifiers(pc);
            pc.Skills.Add("Combat Training");
            pc.RefreshMoves();
            Console.Clear();
            Console.WriteLine("Write a short backstory (press Enter twice to finish):");
            pc.Backstory = ReadMultiline();

            Console.Clear();
            Console.WriteLine("Character created:");
            Console.WriteLine(pc.GetFullDescription());
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
            Console.Clear();

            return pc;
        }

        // Provides a menu for changing an existing character's details.
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
                        pc.Class = NewWorldPP2.UI.CharacterCustomization.ChooseClass();
                        break;
                    case "6":
                        pc.StartingWeapon = NewWorldPP2.UI.CharacterCustomization.ChooseWeapon(pc.Class);
                        break;
                    case "7":
                        pc.StartingArmor = NewWorldPP2.UI.CharacterCustomization.ChooseArmor(pc.Class);
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

        // Prompts until the player selects a valid race.
        private static Race ChooseRace()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Choose a race:");
                Console.WriteLine("1) Human (+1 all stats)");
                Console.WriteLine("2) Elf (+2 Dexterity, -1 Constitution, +1 Intelligence, -1 Wisdom)");
                Console.WriteLine("3) Dwarf (+2 Constitution, -1 Charisma, +1 Strength, -1 Dexterity)");
                Console.WriteLine("4) Orc (+2 Strength, -1 Intelligence, +1 Constitution, -1 Wisdom)");
                Console.WriteLine("5) Undead (+2 Constitution, -1 Wisdom, +1 Intelligence, -1 Charisma)");
                Console.WriteLine("6) Goblin (+2 Dexterity, -1 Strength, +1 Luck, +1 Perception)");
                Console.WriteLine("--- Special Races ---");
                Console.WriteLine("7) Angel (+2 Wisdom, +1 Charisma, -1 Constitution, -100 Strength, +75 Luck)");
                Console.WriteLine("8) Demon (+100 Strength, -10 Charisma, +50 Constitution, -25 Wisdom, -50 Luck)");
                Console.WriteLine("9) White Walker (-100 Charisma, -50 Constitution, +25 Wisdom, -50 Luck, +25 Intelligence, +75 Perception)");
                Console.WriteLine("10) Dragon (+50 Strength, +25 Constitution, +25 Intelligence, +10 Wisdom, +10 Charisma, -10 Luck)");
                Console.WriteLine("Enter the number of your choice (1-10):");
                Console.Write("Selection: ");
                string? input = Console.ReadLine();
                if (int.TryParse(input, out var val) && val >= 1 && val <= 10)
                {
                    return (Race)val;
                }
                Console.WriteLine("Invalid selection, try again.");
            }
        }

        // Allocates a fixed pool of points across all character stats.
        private static Stats DistributePoints(int totalPoints)
        {
            Stats s = new Stats(1, 1, 1, 1, 1, 1, 1, 1, 1);
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
                remaining = PromptAddToStat("Luck", s.Luck, out newVal, remaining);
                s.Luck = newVal;
                remaining = PromptAddToStat("Perception", s.Perception, out newVal, remaining);
                s.Perception = newVal;
                remaining = PromptAddToStat("Speed", s.Speed, out newVal, remaining);
                s.Speed = newVal;

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

        // Validates one stat allocation and returns the points spent.
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

        // Rebuilds the final stats by applying the selected race's modifiers.
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
                Charisma = pc.BaseStats.Charisma,
                Luck = pc.BaseStats.Luck,
                Perception = pc.BaseStats.Perception,
                Speed = pc.BaseStats.Speed
            };

            switch (pc.Race)
            {
                case Race.Human:
                    pc.Stats.Strength += 1; pc.Stats.Dexterity += 1; pc.Stats.Constitution += 1;
                    pc.Stats.Intelligence += 1; pc.Stats.Wisdom += 1; pc.Stats.Charisma += 1;
                    break;
                case Race.Elf:
                    pc.Stats.Dexterity += 2; pc.Stats.Constitution -= 1; pc.Stats.Intelligence += 1; pc.Stats.Wisdom += 1;
                    break;
                case Race.Dwarf:
                    pc.Stats.Constitution += 2; pc.Stats.Charisma -= 1; pc.Stats.Strength += 1; pc.Stats.Wisdom += 1;
                    break;
                case Race.Orc:
                    pc.Stats.Strength += 2; pc.Stats.Intelligence -= 1; pc.Stats.Constitution += 1; pc.Stats.Dexterity += 1;
                    break;
                case Race.Undead:
                    pc.Stats.Constitution += 2; pc.Stats.Wisdom -= 1; pc.Stats.Intelligence += 1; pc.Stats.Charisma -= 1;
                    break;
                case Race.Goblin:
                    pc.Stats.Dexterity += 2; pc.Stats.Strength -= 1; pc.Stats.Luck += 1; pc.Stats.Perception += 1;
                    break;
                case Race.Angel:
                    pc.Stats.Wisdom += 2; pc.Stats.Charisma += 1; pc.Stats.Constitution -= 1; pc.Stats.Strength -= 100; pc.Stats.Luck += 75;
                    break;
                case Race.Demon:
                    pc.Stats.Strength += 100; pc.Stats.Charisma -= 10; pc.Stats.Constitution += 50; pc.Stats.Wisdom -= 25; pc.Stats.Luck -= 50;
                    break;
                case Race.WhiteWalker:
                    pc.Stats.Charisma -= 100; pc.Stats.Constitution -= 50; pc.Stats.Wisdom += 25; pc.Stats.Luck -= 50; pc.Stats.Intelligence += 25; pc.Stats.Perception += 75;
                    break;
                case Race.Dragon:
                    pc.Stats.Strength += 50; pc.Stats.Constitution += 25; pc.Stats.Intelligence += 25; pc.Stats.Wisdom += 10; pc.Stats.Charisma += 10; pc.Stats.Luck -= 10;
                    break;
            }

            pc.RecalculateDerivedStats();
        }

        // Reads backstory lines until the player submits an empty line.
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

        // Checks the name rules used by character creation and customization.
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


