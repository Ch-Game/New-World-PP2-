using System;
using NewWorldPP2.Entities;
using NewWorldPP2.Services.World;

namespace NewWorldPP2.Services
{
    // Runs staged combat activities and awards a final dungeon reward.
    public static class DungeonSystem
    {
        // Runs the selected dungeon or gauntlet until completion or retreat.
        public static void Run(PlayerCharacter player, WorldLocation location)
        {
            var isGauntlet = location.Type == LocationType.Gauntlet;
            var stageCount = isGauntlet ? 5 : 3;
            var activityName = isGauntlet ? "the Emberfall Gauntlet" : "the Cinder Mines";

            Console.Clear();
            Console.WriteLine($"=== {activityName} ===");
            Console.WriteLine(isGauntlet
                ? "Five champions stand between you and the champion's cache."
                : "Three dangerous depths await beneath the mines.");
            Console.WriteLine("Every victory earns loot. Fleeing ends the run.");
            Console.WriteLine("Press Enter to begin...");
            Console.ReadLine();

            for (var stage = 1; stage <= stageCount; stage++)
            {
                var enemy = CreateEnemy(stage, isGauntlet, player);
                Console.Clear();
                Console.WriteLine($"=== Stage {stage}/{stageCount} ===");
                Console.WriteLine($"A guardian blocks your path: {enemy.Name}.");
                Console.WriteLine("Press Enter to fight...");
                Console.ReadLine();

                if (!CombatSystem.Fight(player, enemy))
                {
                    if (player.Health > 0)
                    {
                        Console.WriteLine("You leave the run before reaching the final reward.");
                        Pause();
                    }

                    return;
                }

                Console.WriteLine($"Stage {stage} cleared. You collect the guardian's loot.");
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
            }

            var finalReward = CreateFinalReward(isGauntlet);
            player.Inventory.Add(finalReward);
            var goldReward = (isGauntlet ? 150 : 75) + StatCalculator.RewardBonus(player.Stats) * 5;
            player.Gold += goldReward;

            Console.Clear();
            Console.WriteLine(isGauntlet
                ? "You conquer the Emberfall Gauntlet!"
                : "You conquer the depths of the Cinder Mines!");
            Console.WriteLine($"Champion's cache: {goldReward} gold and {finalReward.Name}.");
            Console.WriteLine("Press Enter to return to the world...");
            Console.ReadLine();
        }

        // Builds a stage-appropriate guardian with scaling statistics and loot.
        private static Enemy CreateEnemy(int stage, bool isGauntlet, PlayerCharacter player)
        {
            var names = isGauntlet
                ? new[] { "Ashen Challenger", "Iron Arena Beast", "Flamebound Knight", "Obsidian Ravager", "Emberfall Flamebeast" }
                : new[] { "Cinder Rat Swarm", "Molten Mine Stalker", "Magma Brute" };
            var index = Math.Min(stage - 1, names.Length - 1);
            var health = (isGauntlet ? 28 : 24) + stage * (isGauntlet ? 8 : 6) + Math.Max(0, player.Stats.Perception - 5);
            var attack = (isGauntlet ? 6 : 5) + stage + Math.Max(0, player.Stats.Strength / 4);
            var defense = (isGauntlet ? 2 : 1) + stage + Math.Max(0, player.Stats.Wisdom / 4);
            var rarity = stage >= (isGauntlet ? 4 : 3) ? ItemRarity.Rare : ItemRarity.Uncommon;

            return new Enemy
            {
                Name = names[index],
                MaxHealth = health,
                Health = health,
                Attack = attack,
                Defense = defense,
                GoldReward = (isGauntlet ? 20 : 12) * stage + StatCalculator.RewardBonus(player.Stats),
                ExperienceReward = (isGauntlet ? 35 : 25) + stage * 10,
                ItemReward = CreateStageLoot(stage, rarity)
            };
        }

        // Randomly selects equipment or a potion for a cleared stage.
        private static Item CreateStageLoot(int stage, ItemRarity rarity)
        {
            return Dice.Roll(6) switch
            {
                1 => ItemFactory.CreatePotion(PotionType.Health, rarity),
                2 => ItemFactory.CreatePotion(PotionType.Mana, rarity),
                3 => ItemFactory.CreatePotion(PotionType.Strength, rarity),
                4 => ItemFactory.CreatePotion(PotionType.Stamina, rarity),
                5 => ItemFactory.CreateWeapon(
                    stage % 2 == 0 ? WeaponCategory.Spear : WeaponCategory.Dagger,
                    stage >= 4 ? Material.Obsidian : Material.Iron,
                    rarity),
                _ => ItemFactory.CreateArmor(
                    stage % 2 == 0 ? ArmorCategory.Chainmail : ArmorCategory.Leather,
                    stage >= 4 ? Material.Obsidian : Material.Steel,
                    rarity)
            };
        }

        // Creates the guaranteed reward for completing the activity.
        private static Item CreateFinalReward(bool isGauntlet)
        {
            return isGauntlet
                ? ItemFactory.CreateWeapon(WeaponCategory.Axe, Material.Obsidian, ItemRarity.Legendary)
                : ItemFactory.CreateArmor(ArmorCategory.Chestplate, Material.Steel, ItemRarity.Rare);
        }

        // Pauses the console before returning to the world.
        private static void Pause()
        {
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }
    }
}
