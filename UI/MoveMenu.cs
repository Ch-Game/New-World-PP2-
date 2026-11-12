using System;
using System.Linq;
using NewWorldPP2.Entities;

namespace NewWorldPP2.UI
{
    // Lets the player manage the four moves used in combat.
    public static class MoveMenu
    {
        public static void Manage(PlayerCharacter player)
        {
            player.RefreshMoves();
            while (true)
            {
                Console.Clear();
                Console.WriteLine("--- Move Management ---");
                Console.WriteLine($"Special move points: {player.SpecialMovePoints}");
                Console.WriteLine($"Equipped ({player.EquippedMoves.Count}/4): {(player.EquippedMoves.Count == 0 ? "None" : string.Join(", ", player.EquippedMoves))}");
                Console.WriteLine();
                Console.WriteLine("1) Learn special move");
                Console.WriteLine("2) Equip or unequip a known move");
                Console.WriteLine("3) Forget a special move");
                Console.WriteLine("4) View known moves");
                Console.WriteLine("0) Back");
                Console.Write("Choose an option: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        LearnMove(player);
                        break;
                    case "2":
                        EquipOrUnequipMove(player);
                        break;
                    case "3":
                        ForgetMove(player);
                        break;
                    case "4":
                        ShowKnownMoves(player);
                        break;
                    case "0":
                        return;
                }
            }
        }

        private static void LearnMove(PlayerCharacter player)
        {
            var available = player.GetAvailableSpecialMoves();
            Console.Clear();
            Console.WriteLine("--- Learn Special Move ---");
            Console.WriteLine($"Special move points: {player.SpecialMovePoints}");
            if (available.Count == 0)
            {
                Console.WriteLine("There are no new special moves available at your level.");
                Pause();
                return;
            }

            for (var i = 0; i < available.Count; i++)
            {
                var move = available[i];
                Console.WriteLine($"{i + 1}) {move.Name} - {move.SpecialPointCost} point(s), requires level {move.RequiredLevel}{WeaponRequirement(move)}");
                Console.WriteLine($"   {move.Description}");
            }
            Console.WriteLine("0) Cancel");
            Console.Write("Choose a move to learn: ");
            if (!int.TryParse(Console.ReadLine(), out var choice) || choice == 0) return;
            if (choice < 1 || choice > available.Count) return;

            var selected = available[choice - 1];
            if (player.LearnSpecialMove(selected.Name))
            {
                Console.WriteLine($"You learned {selected.Name}.");
            }
            else
            {
                Console.WriteLine("You do not have enough special move points.");
            }
            Pause();
        }

        private static void EquipOrUnequipMove(PlayerCharacter player)
        {
            Console.Clear();
            Console.WriteLine("--- Equip or Unequip Move ---");
            if (player.KnownMoves.Count == 0)
            {
                Console.WriteLine("You do not know any moves.");
                Pause();
                return;
            }

            for (var i = 0; i < player.KnownMoves.Count; i++)
            {
                var move = player.KnownMoves[i];
                var equipped = player.EquippedMoves.Exists(name => name.Equals(move.Name, StringComparison.OrdinalIgnoreCase));
                Console.WriteLine($"{i + 1}) {(equipped ? "[Equipped] " : "")}{move.Name}{WeaponRequirement(move)} - {move.Description}");
            }
            Console.WriteLine("0) Cancel");
            Console.Write("Choose a move: ");
            if (!int.TryParse(Console.ReadLine(), out var choice) || choice == 0) return;
            if (choice < 1 || choice > player.KnownMoves.Count) return;

            var selected = player.KnownMoves[choice - 1];
            if (player.EquippedMoves.Exists(name => name.Equals(selected.Name, StringComparison.OrdinalIgnoreCase)))
            {
                player.UnequipMove(selected.Name);
                Console.WriteLine($"Unequipped {selected.Name}.");
            }
            else if (player.EquipMove(selected.Name))
            {
                Console.WriteLine($"Equipped {selected.Name}.");
            }
            else
            {
                Console.WriteLine("You already have four moves equipped. Unequip one first.");
            }
            Pause();
        }

        private static void ForgetMove(PlayerCharacter player)
        {
            var specialMoves = player.KnownMoves.Where(move => move.SourceType == MoveSourceType.Special).ToList();
            Console.Clear();
            Console.WriteLine("--- Forget Special Move ---");
            if (specialMoves.Count == 0)
            {
                Console.WriteLine("You have no learned special moves to forget.");
                Pause();
                return;
            }

            for (var i = 0; i < specialMoves.Count; i++)
            {
                var move = specialMoves[i];
                var equipped = player.EquippedMoves.Any(name => name.Equals(move.Name, StringComparison.OrdinalIgnoreCase));
                Console.WriteLine($"{i + 1}) {move.Name} (refunds {move.SpecialPointCost} point(s)){(equipped ? " [Equipped]" : "")}");
            }
            Console.WriteLine("0) Cancel");
            Console.Write("Choose a move to forget: ");
            if (!int.TryParse(Console.ReadLine(), out var choice) || choice == 0) return;
            if (choice < 1 || choice > specialMoves.Count) return;

            var selected = specialMoves[choice - 1];
            if (player.ForgetSpecialMove(selected.Name))
            {
                Console.WriteLine($"You forgot {selected.Name} and recovered {selected.SpecialPointCost} point(s).");
            }
            else
            {
                Console.WriteLine("Unequip that move before forgetting it.");
            }
            Pause();
        }

        private static void ShowKnownMoves(PlayerCharacter player)
        {
            Console.Clear();
            Console.WriteLine("--- Known Moves ---");
            foreach (var move in player.KnownMoves)
            {
                var equipped = player.EquippedMoves.Any(name => name.Equals(move.Name, StringComparison.OrdinalIgnoreCase));
                Console.WriteLine($"{(equipped ? "[Equipped] " : "")}{move.Name}{WeaponRequirement(move)} - {move.SourceType} ({move.Source})");
                Console.WriteLine($"  {move.Description}");
            }
            Pause();
        }

        private static void Pause()
        {
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }

        private static string WeaponRequirement(Move move)
        {
            return move.RequiredWeaponCategory == null
                ? string.Empty
                : $" [Requires {move.RequiredWeaponCategory}]";
        }
    }
}
