using System;
using System.Collections.Generic;
using NewWorldPP2.Entities;

namespace NewWorldPP2.Services
{
    // Creates weapons, armor, and potions from their gameplay categories.
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
                Durability = (int)Math.Round(30 * matMul * rarityMul),
                WeaponCategory = cat,
                RequiredStats = WeaponRequirements(cat, mat, rarity)
            };

            if (rarity == ItemRarity.Mythical)
            {
                itm.Abilities = MythicalAbilitiesForWeapon(cat);
            }

            return itm;
        }

        private static Stats WeaponRequirements(WeaponCategory category, Material material, ItemRarity rarity)
        {
            var materialTier = material switch
            {
                Material.Wood => 0,
                Material.Bronze => 1,
                Material.Iron => 2,
                Material.Steel => 3,
                Material.Obsidian => 4,
                Material.Mythril => 5,
                Material.Adamantine => 6,
                _ => 0
            };
            var rarityTier = rarity switch
            {
                ItemRarity.Common => 0,
                ItemRarity.Uncommon => 1,
                ItemRarity.Rare => 2,
                ItemRarity.Epic => 3,
                ItemRarity.Legendary => 4,
                ItemRarity.Mythical => 5,
                _ => 0
            };
            var primaryRequirement = 3 + materialTier * 2 + rarityTier * 2;

            var requirements = category switch
            {
                WeaponCategory.Bow => new Stats(dexterity: primaryRequirement, perception: Math.Max(0, primaryRequirement - 3)),
                WeaponCategory.Dagger => new Stats(speed: rarity == ItemRarity.Mythical ? 20 : primaryRequirement + 2, dexterity: primaryRequirement),
                WeaponCategory.Staff => new Stats(intelligence: primaryRequirement, wisdom: Math.Max(0, primaryRequirement - 2)),
                WeaponCategory.MagicGun => new Stats(intelligence: primaryRequirement, perception: primaryRequirement),
                WeaponCategory.Gauntlets => new Stats(strength: primaryRequirement, dexterity: Math.Max(0, primaryRequirement - 2)),
                WeaponCategory.Spear => new Stats(strength: primaryRequirement, dexterity: Math.Max(0, primaryRequirement - 3)),
                WeaponCategory.Mace => new Stats(strength: primaryRequirement, constitution: Math.Max(0, primaryRequirement - 2)),
                _ => new Stats(strength: primaryRequirement)
            };

            return requirements;
        }

        // Creates armor whose defense and durability scale with material and rarity.
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
                Durability = (int)Math.Round(40 * matMul * rarityMul),
                ArmorCategory = cat
            };

            if (rarity == ItemRarity.Mythical)
            {
                itm.Abilities = MythicalAbilitiesForArmor(cat);
            }

            return itm;
        }

        // Creates a consumable whose effect strength scales with rarity.
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

        // Converts an item rarity into the multiplier used by factory statistics.
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

        // Creates the special ability associated with a mythical weapon category.
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

        // Creates the special ability associated with a mythical armor category.
        private static List<Ability> MythicalAbilitiesForArmor(ArmorCategory cat)
        {
            return cat switch
            {
                ArmorCategory.Robes => new List<Ability> { new Ability { Name = "Spell Ward", Description = "Reduces incoming magic damage." } },
                ArmorCategory.Chestplate => new List<Ability> { new Ability { Name = "Bulwark", Description = "Chance to block heavy hits completely." } },
                _ => new List<Ability> { new Ability { Name = "Aegis", Description = "Small passive regen when out of combat." } }
            };
        }

        // Creates the special ability associated with a mythical potion type.
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
}

