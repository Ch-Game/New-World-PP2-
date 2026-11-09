using System;
using System.Collections.Generic;
using NewWorldPP2.Entities;

namespace NewWorldPP2.Services
{
    // Provides purchasable equipment and consumables for market locations.
    public static class ShopSystem
    {
        private sealed record ShopOffer(Item Item, int Price);

        // Opens the market catalog and handles purchases until the player leaves.
        public static void Open(PlayerCharacter player)
        {
            var offers = CreateCatalog(player);

            while (offers.Count > 0)
            {
                Console.Clear();
                Console.WriteLine("=== Market Shop ===");
                Console.WriteLine($"Gold: {player.Gold}");
                Console.WriteLine();

                for (var i = 0; i < offers.Count; i++)
                {
                    var offer = offers[i];
                    Console.WriteLine($"{i + 1}) {offer.Item.Name} - {offer.Price} gold");
                    Console.WriteLine($"   {offer.Item}");
                }

                Console.WriteLine("0) Leave shop");
                Console.Write("Choose an item to buy: ");
                var input = Console.ReadLine();

                if (!int.TryParse(input, out var choice) || choice < 0 || choice > offers.Count)
                {
                    Console.WriteLine("Invalid shop choice. Press Enter to continue...");
                    Console.ReadLine();
                    continue;
                }

                if (choice == 0)
                {
                    return;
                }

                var selectedIndex = choice - 1;
                var selectedOffer = offers[selectedIndex];
                if (player.Gold < selectedOffer.Price)
                {
                    Console.WriteLine("You do not have enough gold for that item. Press Enter to continue...");
                    Console.ReadLine();
                    continue;
                }

                player.Gold -= selectedOffer.Price;
                player.Inventory.Add(selectedOffer.Item);
                offers.RemoveAt(selectedIndex);
                Console.WriteLine($"You purchased {selectedOffer.Item.Name} for {selectedOffer.Price} gold.");
                Console.WriteLine("The item was added to your inventory. Press Enter to continue...");
                Console.ReadLine();
            }

            Console.WriteLine("The shop has sold out. Press Enter to leave...");
            Console.ReadLine();
        }

        // Creates the market's weapons, armor, and potion selection.
        private static List<ShopOffer> CreateCatalog(PlayerCharacter player)
        {
            var items = new[]
            {
                ItemFactory.CreateWeapon(WeaponCategory.Sword, Material.Iron, ItemRarity.Common),
                ItemFactory.CreateWeapon(WeaponCategory.Bow, Material.Bronze, ItemRarity.Common),
                ItemFactory.CreateWeapon(WeaponCategory.Dagger, Material.Steel, ItemRarity.Uncommon),
                ItemFactory.CreateArmor(ArmorCategory.Leather, Material.Iron, ItemRarity.Common),
                ItemFactory.CreateArmor(ArmorCategory.Chainmail, Material.Steel, ItemRarity.Uncommon),
                ItemFactory.CreatePotion(PotionType.Health, ItemRarity.Common),
                ItemFactory.CreatePotion(PotionType.Mana, ItemRarity.Common),
                ItemFactory.CreatePotion(PotionType.Strength, ItemRarity.Uncommon)
            };

            var offers = new List<ShopOffer>(items.Length);
            foreach (var item in items)
            {
                offers.Add(new ShopOffer(item, CalculatePrice(item, player)));
            }

            return offers;
        }

        // Calculates a price from the item's rarity and useful combat statistics.
        private static int CalculatePrice(Item item, PlayerCharacter player)
        {
            var rarityMultiplier = item.Rarity switch
            {
                ItemRarity.Common => 1.0,
                ItemRarity.Uncommon => 1.5,
                ItemRarity.Rare => 2.25,
                ItemRarity.Epic => 3.25,
                ItemRarity.Legendary => 4.5,
                ItemRarity.Mythical => 7.0,
                _ => 1.0
            };

            var value = 15 + item.Attack * 6 + item.Defense * 5 + item.Magic * 2 + item.Speed * 3;
            var discounted = value * rarityMultiplier * (100 - StatCalculator.ShopDiscountPercent(player.Stats)) / 100.0;
            return Math.Max(10, (int)Math.Round(discounted));
        }
    }
}
