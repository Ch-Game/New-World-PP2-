using System;
using System.Collections.Generic;
using System.Linq;
using NewWorldPP2.Entities;

namespace NewWorldPP2.Services
{
    // Defines moves granted by progression, skills, and equipment.
    public static class MoveCatalog
    {
        public static void Refresh(PlayerCharacter player)
        {
            player.KnownMoves ??= new List<Move>();
            player.EquippedMoves ??= new List<string>();
            player.Skills ??= new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var move in LevelMoves.Concat(SkillMoves(player)).Concat(EquipmentMoves(player)))
            {
                if (move.RequiredLevel > player.Level || player.KnownMoves.Any(m => m.Name.Equals(move.Name, StringComparison.OrdinalIgnoreCase))) continue;
                player.KnownMoves.Add(move);
                Console.WriteLine($"You learned the move {move.Name} ({move.SourceType}).");
            }

            player.EquippedMoves.RemoveAll(name => !player.KnownMoves.Any(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase)));
            foreach (var move in player.KnownMoves.Where(m => !player.EquippedMoves.Any(n => n.Equals(m.Name, StringComparison.OrdinalIgnoreCase))))
            {
                if (player.EquippedMoves.Count == 4) break;
                player.EquippedMoves.Add(move.Name);
            }
        }

        public static IReadOnlyList<Move> GetSkillMoves(PlayerCharacter player) => SkillMoves(player).ToList();

        public static IReadOnlyList<Move> GetSpecialMoves(PlayerCharacter player)
        {
            return SpecialMoves
                .Where(move => move.RequiredLevel <= player.Level)
                .Select(move => move.Clone())
                .ToList();
        }

        private static IEnumerable<Move> LevelMoves => new[]
        {
            new Move { Name = "Quick Strike", Description = "A reliable fast attack.", SourceType = MoveSourceType.Level, Source = "Level 1", Power = 3, Accuracy = 95 },
            new Move { Name = "Guard Break", Description = "A heavy attack that ignores some defense.", SourceType = MoveSourceType.Level, Source = "Level 2", RequiredLevel = 2, RequiredWeaponCategory = WeaponCategory.Sword, Power = 5, Accuracy = 75, IgnoresDefense = true },
            new Move { Name = "Second Wind", Description = "Recover health during combat.", SourceType = MoveSourceType.Level, Source = "Level 3", RequiredLevel = 3, Healing = 8, Accuracy = 100 },
            new Move { Name = "Spear Thrust", Description = "A precise attack with extended reach.", SourceType = MoveSourceType.Level, Source = "Level 3", RequiredLevel = 3, RequiredWeaponCategory = WeaponCategory.Spear, Power = 7, Accuracy = 90 },
            new Move { Name = "Shield Bash", Description = "Stagger the enemy while bracing for impact.", SourceType = MoveSourceType.Level, Source = "Level 4", RequiredLevel = 4, RequiredWeaponCategory = WeaponCategory.Mace, Power = 5, Accuracy = 85, DefenseBoost = 3 },
            new Move { Name = "Aimed Shot", Description = "Take careful aim to strike through armor.", SourceType = MoveSourceType.Level, Source = "Level 4", RequiredLevel = 4, RequiredWeaponCategory = WeaponCategory.Bow, Power = 8, Accuracy = 90, IgnoresDefense = true },
            new Move { Name = "Heroic Impact", Description = "A powerful high-level attack.", SourceType = MoveSourceType.Level, Source = "Level 5", RequiredLevel = 5, RequiredWeaponCategory = WeaponCategory.Sword, Power = 10, Accuracy = 70 },
            new Move { Name = "Twin Fang", Description = "A rapid dagger strike aimed at a vulnerable point.", SourceType = MoveSourceType.Level, Source = "Level 6", RequiredLevel = 6, RequiredWeaponCategory = WeaponCategory.Dagger, Power = 11, Accuracy = 80 },
            new Move { Name = "Arcane Bolt", Description = "Launch a concentrated bolt of magical energy.", SourceType = MoveSourceType.Level, Source = "Level 7", RequiredLevel = 7, RequiredWeaponCategory = WeaponCategory.Staff, Power = 13, Accuracy = 85 },
            new Move { Name = "Iron Fists", Description = "Deliver a crushing gauntlet attack.", SourceType = MoveSourceType.Level, Source = "Level 8", RequiredLevel = 8, RequiredWeaponCategory = WeaponCategory.Gauntlets, Power = 15, Accuracy = 80 }
        };

        private static IEnumerable<Move> SpecialMoves => new[]
        {
            new Move
            {
                Name = "Whirlwind",
                Description = "A sweeping attack that strikes with exceptional force.",
                SourceType = MoveSourceType.Special,
                Source = "Special move catalog",
                RequiredLevel = 2,
                SpecialPointCost = 2,
                RequiredWeaponCategory = WeaponCategory.Axe,
                Power = 8,
                Accuracy = 85
            },
            new Move
            {
                Name = "Vital Surge",
                Description = "A focused burst of energy that restores health.",
                SourceType = MoveSourceType.Special,
                Source = "Special move catalog",
                RequiredLevel = 3,
                SpecialPointCost = 3,
                RequiredWeaponCategory = WeaponCategory.Staff,
                Healing = 18,
                Accuracy = 100
            },
            new Move
            {
                Name = "Iron Fortress",
                Description = "Raise an iron defense against the next attack.",
                SourceType = MoveSourceType.Special,
                Source = "Special move catalog",
                RequiredLevel = 4,
                SpecialPointCost = 3,
                RequiredWeaponCategory = WeaponCategory.Mace,
                DefenseBoost = 8,
                Accuracy = 100
            },
            new Move
            {
                Name = "Executioner's Blow",
                Description = "A devastating strike that cuts through defense.",
                SourceType = MoveSourceType.Special,
                Source = "Special move catalog",
                RequiredLevel = 6,
                SpecialPointCost = 5,
                RequiredWeaponCategory = WeaponCategory.Sword,
                Power = 15,
                Accuracy = 75,
                IgnoresDefense = true
            },
            new Move
            {
                Name = "Phoenix Renewal",
                Description = "A legendary recovery technique that restores substantial health.",
                SourceType = MoveSourceType.Special,
                Source = "Special move catalog",
                RequiredLevel = 8,
                SpecialPointCost = 6,
                RequiredWeaponCategory = WeaponCategory.Staff,
                Healing = 30,
                Accuracy = 100
            },
            new Move
            {
                Name = "Tempest Lance",
                Description = "A focused spear strike that tears through defenses.",
                SourceType = MoveSourceType.Special,
                Source = "Special move catalog",
                RequiredLevel = 5,
                SpecialPointCost = 4,
                RequiredWeaponCategory = WeaponCategory.Spear,
                Power = 18,
                Accuracy = 80,
                IgnoresDefense = true
            },
            new Move
            {
                Name = "Meteor Round",
                Description = "Fire a devastating magical shot from a gun.",
                SourceType = MoveSourceType.Special,
                Source = "Special move catalog",
                RequiredLevel = 6,
                SpecialPointCost = 5,
                RequiredWeaponCategory = WeaponCategory.MagicGun,
                Power = 20,
                Accuracy = 75
            },
            new Move
            {
                Name = "Stonebreaker",
                Description = "A crushing blow that leaves no defense intact.",
                SourceType = MoveSourceType.Special,
                Source = "Special move catalog",
                RequiredLevel = 7,
                SpecialPointCost = 5,
                RequiredWeaponCategory = WeaponCategory.Gauntlets,
                Power = 22,
                Accuracy = 70,
                IgnoresDefense = true
            }
        };

        private static IEnumerable<Move> SkillMoves(PlayerCharacter player)
        {
            if (player.Skills.Contains("Combat Training"))
            {
                yield return new Move { Name = "Power Slash", Description = "A trained sword technique.", SourceType = MoveSourceType.Skill, Source = "Combat Training", RequiredWeaponCategory = WeaponCategory.Sword, Power = 6, Accuracy = 85 };
                yield return new Move { Name = "Weapon Mastery", Description = "A disciplined strike that adapts to any weapon.", SourceType = MoveSourceType.Skill, Source = "Combat Training", Power = 8, Accuracy = 90 };
            }
            if (player.Skills.Contains("Field Medicine"))
            {
                yield return new Move { Name = "Battle Heal", Description = "Restore health with field medicine.", SourceType = MoveSourceType.Skill, Source = "Field Medicine", Healing = 12, Accuracy = 100 };
                yield return new Move { Name = "Tactical Recovery", Description = "Recover health while preparing a defensive stance.", SourceType = MoveSourceType.Skill, Source = "Field Medicine", Healing = 7, DefenseBoost = 4, Accuracy = 100 };
            }
        }

        private static IEnumerable<Move> EquipmentMoves(PlayerCharacter player)
        {
            foreach (var item in player.Inventory)
            {
                if (item.WeaponCategory != null && item.Abilities != null)
                {
                    foreach (var ability in item.Abilities)
                    {
                        var move = MoveFromAbility(ability, MoveSourceType.Weapon, item.Name, item.WeaponCategory);
                        if (move != null) yield return move;
                    }
                }
                if (item.ArmorCategory != null && item.Abilities != null)
                {
                    foreach (var ability in item.Abilities)
                    {
                        var move = MoveFromAbility(ability, MoveSourceType.Armor, item.Name, null);
                        if (move != null) yield return move;
                    }
                }
            }
        }

        private static Move? MoveFromAbility(Ability ability, MoveSourceType sourceType, string source, WeaponCategory? requiredWeaponCategory)
        {
            var move = ability.Name switch
            {
                "Flameburst" => new Move { Power = 8, Accuracy = 80, Description = "A burst of flame that deals heavy damage." },
                "Piercing Shot" => new Move { Power = 6, Accuracy = 90, IgnoresDefense = true, Description = "A shot that ignores enemy defense." },
                "Arcane Surge" => new Move { Power = 9, Accuracy = 85, Description = "A surge of arcane power." },
                "Shadowstep" => new Move { Power = 7, Accuracy = 90, Description = "Strike from the enemy's blind spot." },
                "Mana Bullet" => new Move { Power = 7, Accuracy = 95, Description = "Fire a concentrated magical bullet." },
                "Eternal Edge" => new Move { Power = 9, Accuracy = 80, Description = "A devastating legendary cut." },
                "Bulwark" => new Move { DefenseBoost = 5, Accuracy = 100, Description = "Brace behind an unbreakable defense." },
                "Spell Ward" => new Move { DefenseBoost = 4, Accuracy = 100, Description = "Raise a ward that reduces the next hit." },
                "Aegis" => new Move { Healing = 6, Accuracy = 100, Description = "Recover behind an aegis." },
                _ => null
            };
            if (move == null) return null;
            move.Name = ability.Name;
            move.SourceType = sourceType;
            move.Source = source;
            move.RequiredWeaponCategory = requiredWeaponCategory;
            return move;
        }
    }
}
