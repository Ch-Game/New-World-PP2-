using System;
using System.Collections.Generic;
using System.Linq;
using NewWorldPP2.Services;

namespace NewWorldPP2.Entities
{
    // Holds the player's identity, progression, statistics, and inventory.
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
        public int Level { get; set; } = 1;
        public int Experience { get; set; }
        public int UnspentStatPoints { get; set; }
        public List<Item> Inventory { get; set; } = new();
        public HashSet<string> Skills { get; set; } = new(StringComparer.OrdinalIgnoreCase);
        public List<Move> KnownMoves { get; set; } = new();
        public List<string> EquippedMoves { get; set; } = new();
        public HashSet<string> OpenedChestIds { get; set; } = new();

        // Records a chest as opened and prevents the same chest from being opened twice.
        public bool TryOpenChest(string chestId)
        {
            if (string.IsNullOrWhiteSpace(chestId)) return false;

            OpenedChestIds ??= new HashSet<string>();
            return OpenedChestIds.Add(chestId);
        }

        public int ExperienceToNextLevel => 100 + (Math.Max(1, Level) - 1) * 50;

        public bool MeetsWeaponRequirements(Item weapon)
        {
            if (weapon == null || weapon.WeaponCategory == null) return false;

            var required = weapon.RequiredStats ?? new Stats();
            return Stats.Strength >= required.Strength
                && Stats.Dexterity >= required.Dexterity
                && Stats.Constitution >= required.Constitution
                && Stats.Intelligence >= required.Intelligence
                && Stats.Wisdom >= required.Wisdom
                && Stats.Charisma >= required.Charisma
                && Stats.Luck >= required.Luck
                && Stats.Perception >= required.Perception
                && Stats.Speed >= required.Speed;
        }

        public int AddExperience(int amount)
        {
            if (amount <= 0) return 0;

            var levelsGained = 0;
            Experience += amount;
            while (Experience >= ExperienceToNextLevel)
            {
                Experience -= ExperienceToNextLevel;
                Level = Math.Max(1, Level) + 1;
                UnspentStatPoints += 3;
                Skills.Add(Level switch
                {
                    2 => "Combat Training",
                    4 => "Field Medicine",
                    _ => string.Empty
                });
                Skills.Remove(string.Empty);
                levelsGained++;
            }

            return levelsGained;
        }

        public void NormalizeProgression()
        {
            Level = Math.Max(1, Level);
            Experience = Math.Max(0, Experience);
            UnspentStatPoints = Math.Max(0, UnspentStatPoints);
            Inventory ??= new List<Item>();
            Skills ??= new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            KnownMoves ??= new List<Move>();
            EquippedMoves ??= new List<string>();
            while (Experience >= ExperienceToNextLevel)
            {
                Experience -= ExperienceToNextLevel;
                Level++;
                UnspentStatPoints += 3;
            }

            if (Level >= 2) Skills.Add("Combat Training");
            if (Level >= 4) Skills.Add("Field Medicine");
            MoveCatalog.Refresh(this);
        }

        public void RefreshMoves() => MoveCatalog.Refresh(this);

        public bool EquipMove(string moveName)
        {
            var move = KnownMoves.Find(m => m.Name.Equals(moveName, StringComparison.OrdinalIgnoreCase));
            if (move == null || EquippedMoves.Any(n => n.Equals(move.Name, StringComparison.OrdinalIgnoreCase))) return false;
            if (EquippedMoves.Count >= 4) return false;
            EquippedMoves.Add(move.Name);
            return true;
        }

        public bool UnequipMove(string moveName) => EquippedMoves.RemoveAll(n => n.Equals(moveName, StringComparison.OrdinalIgnoreCase)) > 0;

        // Rebuilds values that depend on the character's base statistics.
        public void RecalculateDerivedStats()
        {
            // Simple derived stat: MaxHealth scales with Constitution
            MaxHealth = 10 + Stats.Constitution * 2 + Stats.Wisdom;
            if (Health > MaxHealth) Health = MaxHealth;
        }

        // Creates a compact summary suitable for menus and status screens.
        public string GetShortDescription()
        {
            var cls = Class == CharacterClass.Undefined ? "No Class" : Class.ToString();
            var weapon = StartingWeapon == WeaponType.None ? "No Weapon" : StartingWeapon.ToString();
            var armor = StartingArmor == ArmorType.None ? "No Armor" : StartingArmor.ToString();
            return $"{Name} the {Race} ({cls}) - {Stats} (HP {Health}/{MaxHealth})\nWeapon: {weapon}  Armor: {armor}";
        }

        // Creates a detailed summary containing the character's current progress.
        public string GetFullDescription()
        {
            var cls = Class == CharacterClass.Undefined ? "None" : Class.ToString();
            var weapon = StartingWeapon == WeaponType.None ? "None" : StartingWeapon.ToString();
            var armor = StartingArmor == ArmorType.None ? "None" : StartingArmor.ToString();
            return $"Name: {Name}\nClass: {cls}\nRace: {Race}\nLevel: {Level} ({Experience}/{ExperienceToNextLevel} XP)\nUnspent stat points: {UnspentStatPoints}\nStats: {Stats}\nHP: {Health}/{MaxHealth}\nWeapon: {weapon}\nArmor: {armor}\nGold: {Gold}\nInventory: {Inventory.Count} item(s)\nMorality: {Morality}\nBackstory:\n{Backstory}";
        }
    }
}
