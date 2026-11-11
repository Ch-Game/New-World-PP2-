using System;
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
                Console.WriteLine($"Skills: {(player.Skills.Count == 0 ? "None" : string.Join(", ", player.Skills))}");
                Console.WriteLine("Equipped moves: " + (player.EquippedMoves.Count == 0 ? "None" : string.Join(", ", player.EquippedMoves)));
                Console.WriteLine();
                for (var i = 0; i < player.KnownMoves.Count; i++)
                {
                    var move = player.KnownMoves[i];
                    var equipped = player.EquippedMoves.Exists(name => name.Equals(move.Name, StringComparison.OrdinalIgnoreCase));
                    Console.WriteLine($"{i + 1}) {(equipped ? "[Equipped] " : "")}{move.Name} - {move.SourceType} ({move.Source})");
                    Console.WriteLine($"   {move.Description}");
                }
                Console.WriteLine("0) Back");
                Console.Write("Choose a move to equip/unequip: ");
                if (!int.TryParse(Console.ReadLine(), out var choice)) continue;
                if (choice == 0) return;
                if (choice < 1 || choice > player.KnownMoves.Count) continue;

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
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
            }
        }
    }
}
