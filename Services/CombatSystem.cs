using System;
using NewWorldPP2.Entities;

namespace NewWorldPP2.Services
{
    // Runs turn-based combat between the player and a single enemy.
    public static class CombatSystem
    {
        // Presents combat actions until the player wins, flees, or is defeated.
        public static bool Fight(PlayerCharacter player, Enemy enemy)
        {
            player.RefreshMoves();
            Console.Clear();
            Console.WriteLine($"A fight begins: {enemy.Name}!");
            Console.WriteLine(enemy.Name + " has " + enemy.MaxHealth + " health.");
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
                        actionSucceeded = Attack(player, enemy);
                        break;
                    case "2":
                        Console.Clear();
                        actionSucceeded = UseMove(player, enemy, out moveDefenseBonus);
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
                    EnemyAttack(player, enemy, moveDefenseBonus);
                    moveDefenseBonus = 0;
                    round++;
                }
            }

            if (enemy.Health <= 0)
            {
                Console.WriteLine($"You defeat {enemy.Name} and earn {enemy.GoldReward} gold.");
                player.Gold += enemy.GoldReward;
                var previousLevel = player.Level;
                player.AddExperience(enemy.ExperienceReward);
                Console.WriteLine($"You gain {enemy.ExperienceReward} XP.");
                if (player.Level > previousLevel)
                {
                    Console.WriteLine($"Level up! You are now level {player.Level} and gained 3 stat points.");
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
        private static bool Attack(PlayerCharacter player, Enemy enemy)
        {
            var weapon = ChooseWeapon(player);
            var weaponAttack = weapon?.Attack ?? 0;
            var accuracy = StatCalculator.AccuracyBonus(player.Stats);
            var armor = enemy.Defense;
            if (weapon?.Abilities?.Exists(a => a.Name == "Piercing Shot") == true)
            {
                armor = Math.Max(0, armor - Math.Max(1, player.Stats.Perception / 2));
            }

            if (Dice.Roll(20) + accuracy < 8 + armor)
            {
                Console.WriteLine("Your attack misses.");
                return true;
            }

            var damage = Math.Max(1, StatCalculator.AttackBonus(player.Stats) + weaponAttack + Dice.Roll(6) - armor);
            if (weapon?.Abilities?.Exists(a => a.Name == "Flameburst") == true && Dice.Roll(100) <= 25 + player.Stats.Intelligence * 3)
            {
                damage += Math.Max(1, StatCalculator.MagicBonus(player.Stats));
            }
            if (StatCalculator.RollCritical(player.Stats)) damage *= 2;
            enemy.Health = Math.Max(0, enemy.Health - damage);
            Console.WriteLine($"You attack with {weapon?.Name ?? "your fists"} for {damage} damage.");
            return true;
        }

        // Applies the enemy's attack after a successful player action.
        private static void EnemyAttack(PlayerCharacter player, Enemy enemy, int moveDefenseBonus = 0)
        {
            var defense = GetArmorDefense(player) + StatCalculator.DefenseBonus(player.Stats) + moveDefenseBonus;
            var damage = Math.Max(1, enemy.Attack + Dice.Roll(4) - defense);
            player.Health = Math.Max(0, player.Health - damage);
            Console.WriteLine($"{enemy.Name} hits you for {damage} damage.");
        }

        private static bool UseMove(PlayerCharacter player, Enemy enemy, out int defenseBonus)
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

            if (selected.Healing > 0)
            {
                var restored = Math.Min(player.MaxHealth - player.Health, selected.Healing + StatCalculator.HealingBonus(player.Stats));
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
                if (Dice.Roll(100) > selected.Accuracy + StatCalculator.AccuracyBonus(player.Stats))
                {
                    Console.WriteLine($"Your {selected.Name} misses.");
                }
                else
                {
                    var damage = Math.Max(1, selected.Power + (weapon?.Attack ?? 0) + StatCalculator.MagicBonus(player.Stats) / 2 - armor);
                    enemy.Health = Math.Max(0, enemy.Health - damage);
                    Console.Clear();
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
