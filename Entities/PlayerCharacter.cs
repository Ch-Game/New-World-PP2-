

using System.Collections.Generic;

namespace NewWorldPP2.Entities
{
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
        public int Gold { get; set; } = 50;
        public List<Item> Inventory { get; set; } = new();

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
            return $"Name: {Name}\nClass: {cls}\nRace: {Race}\nStats: {Stats}\nHP: {Health}/{MaxHealth}\nWeapon: {weapon}\nArmor: {armor}\nGold: {Gold}\nInventory: {Inventory.Count} item(s)\nMorality: {Morality}\nBackstory:\n{Backstory}";
        }
    }
}
