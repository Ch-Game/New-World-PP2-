using System;
using NewWorldPP2.Entities;
using NewWorldPP2.UI;
using NewWorldPP2.Services.World;

namespace NewWorldPP2.Services
{
    // Runs turn-based combat between the player and a single enemy.
    public static class CombatSystem
    {
        // Presents combat actions until the player wins, flees, or is defeated.
        public static bool Fight(PlayerCharacter player, Enemy enemy, WeatherCondition? weather = null)
        {
            weather ??= WeatherSystem.GetCondition("Clear");
            player.RefreshMoves();
            Console.Clear();
            Console.WriteLine($"A fight begins: {enemy.Name}!");
            Console.WriteLine(enemy.Name + " has " + enemy.MaxHealth + " health.");
            Console.WriteLine($"Weather: {weather.Name} - {weather.Description}");
            var round = 1;
            var moveDefenseBonus = 0;

            while (player.Health > 0 && enemy.Health > 0)
            {
                Console.WriteLine($"--- Round {round} ---");
                Console.WriteLine($"You: {player.Health}/{player.MaxHealth} HP | {enemy.Name}: {enemy.Health}/{enemy.MaxHealth} HP");
                Console.WriteLine("1) Attack");
                Console.WriteLine("2) Use move");
                Console.WriteLine("3) Use potion");
                Console.WriteLine("4) Flee");
                Console.Write("Choose an action: ");

                var choice = Console.ReadLine();
                var actionSucceeded = false;
                switch (choice)
                {
                    case "1":
                        Console.Clear();
                        actionSucceeded = Attack(player, enemy, weather);
                        break;
                    case "2":
                        Console.Clear();
                        actionSucceeded = UseMove(player, enemy, weather, out moveDefenseBonus);
                        break;
                    case "3":
                        Console.Clear();
                        actionSucceeded = UsePotion(player);
                        break;
                    case "4":
                        if (Dice.Roll(100) <= StatCalculator.FleeChance(player.Stats) - StatCalculator.InitiativeBonus(enemy.Stats))
                        {
                            Console.WriteLine("You escape from the fight.");
                            return false;
                        }
                        Console.Clear();
                        Console.WriteLine("You fail to escape.");
                        actionSucceeded = true;
                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine("Invalid combat action.");
                        continue;
                }

                if (actionSucceeded && enemy.Health > 0)
                {
                    EnemyAttack(player, enemy, weather, moveDefenseBonus);
                    moveDefenseBonus = 0;
                    round++;
                }
            }

            if (enemy.Health <= 0)
            {
                Console.WriteLine($"You defeat {enemy.Name} and earn {enemy.GoldReward} gold.");
                player.Gold += enemy.GoldReward;
                var previousPoints = player.SpecialMovePoints;
                var levelsGained = player.AddExperience(enemy.ExperienceReward);
                Console.WriteLine($"You gain {enemy.ExperienceReward} XP.");
                if (levelsGained > 0)
                {
                    Console.WriteLine($"Level up! You gained {levelsGained} level(s), 3 stat points per level, and {player.SpecialMovePoints - previousPoints} special move point(s).");
                    MoveMenu.Manage(player);
                }
                if (enemy.ItemReward != null)
                {
                    player.Inventory.Add(enemy.ItemReward);
                    Console.WriteLine($"You acquire: {enemy.ItemReward.Name}.");
                }

                return true;
            }

            Console.WriteLine("You are defeated.");
            return false;
        }

        // Calculates player damage using the strongest available weapon.
        private static bool Attack(PlayerCharacter player, Enemy enemy, WeatherCondition weather)
        {
            var weapon = ChooseWeapon(player);
            var weaponAttack = weapon?.Attack ?? 0;
            var accuracy = StatCalculator.AccuracyBonus(player.Stats);
            var armor = enemy.Defense;
            if (weapon?.Abilities?.Exists(a => a.Name == "Piercing Shot") == true)
            {
                armor = Math.Max(0, armor - Math.Max(1, player.Stats.Perception / 2));
            }

            var targetDefense = 8 + armor;
            var attackRoll = Dice.RollD20(accuracy + weather.AccuracyModifier);
            if (attackRoll.IsCriticalFailure || (!attackRoll.IsCriticalSuccess && attackRoll.Total < targetDefense))
            {
                Console.WriteLine($"You roll d20: {attackRoll.Roll} + {attackRoll.Modifier} = {attackRoll.Total} vs DC {targetDefense}. Your attack misses.");
                return true;
            }

            var damage = Math.Max(1, StatCalculator.AttackBonus(player.Stats) + weaponAttack + Dice.Roll(6) - armor);
            if (weapon?.Abilities?.Exists(a => a.Name == "Flameburst") == true && Dice.Roll(100) <= 25 + player.Stats.Intelligence * 3)
            {
                damage += Math.Max(1, StatCalculator.MagicBonus(player.Stats));
            }
            var critical = attackRoll.IsCriticalSuccess || StatCalculator.RollCritical(player.Stats);
            if (critical) damage *= 2;
            enemy.Health = Math.Max(0, enemy.Health - damage);
            Console.WriteLine($"You roll d20: {attackRoll.Roll} + {attackRoll.Modifier} = {attackRoll.Total} vs DC {targetDefense}.");
            Console.WriteLine(critical
                ? $"Critical hit! You attack with {weapon?.Name ?? "your fists"} for {damage} damage."
                : $"You attack with {weapon?.Name ?? "your fists"} for {damage} damage.");
            return true;
        }

        // Applies the enemy's attack after a successful player action.
        private static void EnemyAttack(PlayerCharacter player, Enemy enemy, WeatherCondition weather, int moveDefenseBonus = 0)
        {
            var defense = GetArmorDefense(player) + StatCalculator.DefenseBonus(player.Stats) + weather.PlayerDefenseModifier + moveDefenseBonus;
            var targetDefense = 8 + defense;
            var attackRoll = Dice.RollD20(enemy.Attack + weather.EnemyAttackModifier);
            if (attackRoll.IsCriticalFailure || (!attackRoll.IsCriticalSuccess && attackRoll.Total < targetDefense))
            {
                Console.WriteLine($"{enemy.Name} rolls d20: {attackRoll.Roll} + {attackRoll.Modifier} = {attackRoll.Total} vs DC {targetDefense} and misses.");
                return;
            }

            var damage = Math.Max(1, enemy.Attack + weather.EnemyAttackModifier + Dice.Roll(4) - defense);
            if (attackRoll.IsCriticalSuccess) damage *= 2;
            player.Health = Math.Max(0, player.Health - damage);
            Console.WriteLine($"{enemy.Name} rolls d20: {attackRoll.Roll} + {attackRoll.Modifier} = {attackRoll.Total} vs DC {targetDefense}.");
            Console.WriteLine(attackRoll.IsCriticalSuccess
                ? $"Critical hit! {enemy.Name} hits you for {damage} damage."
                : $"{enemy.Name} hits you for {damage} damage.");
        }

        private static bool UseMove(PlayerCharacter player, Enemy enemy, WeatherCondition weather, out int defenseBonus)
        {
            defenseBonus = 0;
            if (player.EquippedMoves.Count == 0)
            {
                Console.WriteLine("You have no equipped moves.");
                return false;
            }

            Console.WriteLine("Choose a move:");
            for (var i = 0; i < player.EquippedMoves.Count; i++)
            {
                var move = player.KnownMoves.Find(m => m.Name.Equals(player.EquippedMoves[i], StringComparison.OrdinalIgnoreCase));
                if (move != null) Console.WriteLine($"{i + 1}) {move.Name} - {move.Description}");
            }
            Console.Write("Move: ");
            if (!int.TryParse(Console.ReadLine(), out var choice) || choice < 1 || choice > player.EquippedMoves.Count)
            {
                Console.WriteLine("Invalid move.");
                return false;
            }

            var selected = player.KnownMoves.Find(m => m.Name.Equals(player.EquippedMoves[choice - 1], StringComparison.OrdinalIgnoreCase));
            if (selected == null) return false;

            if (!player.CanUseMove(selected))
            {
                Console.WriteLine($"You need a {selected.RequiredWeaponCategory} to use {selected.Name}.");
                return false;
            }

            if (selected.Healing > 0)
            {
                var restored = Math.Min(player.MaxHealth - player.Health, Math.Max(0, selected.Healing + weather.HealingModifier + StatCalculator.HealingBonus(player.Stats)));
                player.Health += restored;
                Console.WriteLine($"You use {selected.Name} and restore {restored} health.");
            }

            if (selected.DefenseBoost > 0)
            {
                defenseBonus = selected.DefenseBoost;
                Console.WriteLine($"You use {selected.Name} and gain {selected.DefenseBoost} defense for the next hit.");
            }

            if (selected.Power > 0)
            {
                var weapon = ChooseWeapon(player);
                var armor = selected.IgnoresDefense ? 0 : enemy.Defense;
                var moveAccuracy = Math.Clamp(selected.Accuracy + StatCalculator.AccuracyBonus(player.Stats) + weather.AccuracyModifier, 5, 100);
                var moveRoll = Dice.Roll(100);
                if (moveRoll > moveAccuracy)
                {
                    Console.WriteLine($"You roll d100: {moveRoll} vs {moveAccuracy}% accuracy. Your {selected.Name} misses.");
                }
                else
                {
                    var damage = Math.Max(1, selected.Power + (weapon?.Attack ?? 0) + StatCalculator.MagicBonus(player.Stats) / 2 - armor);
                    enemy.Health = Math.Max(0, enemy.Health - damage);
                    Console.Clear();
                    Console.WriteLine($"You roll d100: {moveRoll} vs {moveAccuracy}% accuracy.");
                    Console.WriteLine($"You use {selected.Name} for {damage} damage.");
                }
            }

            return true;
        }

        // Consumes the first available potion and restores its magic value as health.
        private static bool UsePotion(PlayerCharacter player)
        {
            for (var i = 0; i < player.Inventory.Count; i++)
            {
                var item = player.Inventory[i];
                if (!item.Name.Contains("Potion", StringComparison.OrdinalIgnoreCase)) continue;

                player.Inventory.RemoveAt(i);
                var restored = Math.Max(1, item.Magic + StatCalculator.HealingBonus(player.Stats));
                player.Health = Math.Min(player.MaxHealth, player.Health + restored);
                Console.WriteLine($"You use {item.Name} and restore {restored} health.");
                return true;
            }

            Console.WriteLine("You have no potion to use.");
            return false;
        }

        // Selects the inventory item with the highest attack value.
        private static Item? ChooseWeapon(PlayerCharacter player)
        {
            Item? best = null;
            foreach (var item in player.Inventory)
            {
                if (item.WeaponCategory == null || !player.MeetsWeaponRequirements(item)) continue;
                if (item.Attack > (best?.Attack ?? 0)) best = item;
            }

            return best;
        }

        // Returns the highest defense value provided by the inventory.
        private static int GetArmorDefense(PlayerCharacter player)
        {
            var defense = 0;
            foreach (var item in player.Inventory)
            {
                if (item.Defense > defense) defense = item.Defense;
            }

            return defense;
        }
    }
}
