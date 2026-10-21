using System;
using NewWorldPP2.Entities;

namespace NewWorldPP2.Services
{
    public static class CombatSystem
    {
        public static bool Fight(PlayerCharacter player, Enemy enemy)
        {
            Console.Clear();
            Console.WriteLine($"A fight begins: {enemy.Name}!");
            Console.WriteLine(enemy.Name + " has " + enemy.MaxHealth + " health.");

            while (player.Health > 0 && enemy.Health > 0)
            {
                Console.WriteLine();
                Console.WriteLine($"You: {player.Health}/{player.MaxHealth} HP | {enemy.Name}: {enemy.Health}/{enemy.MaxHealth} HP");
                Console.WriteLine("1) Attack");
                Console.WriteLine("2) Use potion");
                Console.WriteLine("3) Flee");
                Console.Write("Choose an action: ");

                var choice = Console.ReadLine();
                var actionSucceeded = false;
                switch (choice)
                {
                    case "1":
                        actionSucceeded = Attack(player, enemy);
                        break;
                    case "2":
                        actionSucceeded = UsePotion(player);
                        break;
                    case "3":
                        if (Dice.Roll(2) == 1)
                        {
                            Console.WriteLine("You escape from the fight.");
                            return false;
                        }

                        Console.WriteLine("You fail to escape.");
                        actionSucceeded = true;
                        break;
                    default:
                        Console.WriteLine("Invalid combat action.");
                        continue;
                }

                if (actionSucceeded && enemy.Health > 0)
                {
                    EnemyAttack(player, enemy);
                }
            }

            if (enemy.Health <= 0)
            {
                Console.WriteLine($"You defeat {enemy.Name} and earn {enemy.GoldReward} gold.");
                player.Gold += enemy.GoldReward;
                if (enemy.ItemReward != null)
                {
                    player.Inventory.Add(enemy.ItemReward);
                    Console.WriteLine($"You acquire: {enemy.ItemReward.Name}.");
                }

                return true;
            }

            Console.WriteLine("You are defeated and barely escape with 1 health.");
            player.Health = 1;
            return false;
        }

        private static bool Attack(PlayerCharacter player, Enemy enemy)
        {
            var weapon = ChooseWeapon(player);
            var weaponAttack = weapon?.Attack ?? 0;
            var damage = Math.Max(1, player.Stats.Strength + weaponAttack + Dice.Roll(6) - enemy.Defense);
            enemy.Health = Math.Max(0, enemy.Health - damage);
            Console.WriteLine($"You attack with {weapon?.Name ?? "your fists"} for {damage} damage.");
            return true;
        }

        private static void EnemyAttack(PlayerCharacter player, Enemy enemy)
        {
            var defense = GetArmorDefense(player);
            var damage = Math.Max(1, enemy.Attack + Dice.Roll(4) - defense);
            player.Health = Math.Max(0, player.Health - damage);
            Console.WriteLine($"{enemy.Name} hits you for {damage} damage.");
        }

        private static bool UsePotion(PlayerCharacter player)
        {
            for (var i = 0; i < player.Inventory.Count; i++)
            {
                var item = player.Inventory[i];
                if (!item.Name.Contains("Potion", StringComparison.OrdinalIgnoreCase)) continue;

                player.Inventory.RemoveAt(i);
                var restored = Math.Max(1, item.Magic);
                player.Health = Math.Min(player.MaxHealth, player.Health + restored);
                Console.WriteLine($"You use {item.Name} and restore {restored} health.");
                return true;
            }

            Console.WriteLine("You have no potion to use.");
            return false;
        }

        private static Item? ChooseWeapon(PlayerCharacter player)
        {
            Item? best = null;
            foreach (var item in player.Inventory)
            {
                if (item.Attack > (best?.Attack ?? 0)) best = item;
            }

            return best;
        }

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
