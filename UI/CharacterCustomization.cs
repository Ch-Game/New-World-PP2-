using System;
using NewWorldPP2.Entities;

namespace NewWorldPP2.UI
{
    // Provides the console menus for selecting a character's starting options.
    public static class CharacterCustomization
    {
        // Prompts until the player selects a valid character class.
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

        // Prompts until the player selects a valid starting weapon.
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

        // Prompts until the player selects a valid starting armor.
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


