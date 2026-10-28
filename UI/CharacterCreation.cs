using System;
using System.Text;
using NewWorldPP2.Entities;

namespace NewWorldPP2.UI
{
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
            pc.Class = NewWorldPP2.UI.CharacterCustomization.ChooseClass();
            pc.StartingWeapon = NewWorldPP2.UI.CharacterCustomization.ChooseWeapon(pc.Class);
            pc.StartingArmor = NewWorldPP2.UI.CharacterCustomization.ChooseArmor(pc.Class);

            Console.WriteLine($"You have {PointsToDistribute} points to distribute among six stats.");
            pc.BaseStats = DistributePoints(PointsToDistribute);

            ApplyRaceModifiers(pc);
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

        private static Race ChooseRace()
        {
            while (true)
            {
                Console.Clear();
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


