using NewWorldPP2.Entities;
using NewWorldPP2.Services;

namespace NewWorldPP2.Services.World
{
    public static class EncounterSystem
    {
        public static Encounter CreateRandomEncounter(WorldLocation location)
        {
            return Dice.Roll(4) switch
            {
                1 => CreateCombatEncounter(location, true),
                2 => CreateNpcEncounter(location, true),
                3 => CreateTreasureEncounter(location),
                _ => CreateTrapEncounter(location)
            };
        }

        public static Encounter CreateLocationEncounter(WorldLocation location)
        {
            return location.Type switch
            {
                LocationType.Dungeon => CreateCombatEncounter(location, false),
                LocationType.Landmark => Dice.Roll(2) == 1
                    ? CreateTreasureEncounter(location)
                    : CreateTrapEncounter(location),
                LocationType.Tavern or LocationType.Guild => CreateNpcEncounter(location, false),
                LocationType.Market or LocationType.Shrine => CreateTreasureEncounter(location),
                _ => CreateNpcEncounter(location, false)
            };
        }

        private static Encounter CreateCombatEncounter(WorldLocation location, bool roaming)
        {
            var enemy = roaming
                ? new Enemy
                {
                    Name = "Roadside Marauder",
                    MaxHealth = 24,
                    Health = 24,
                    Attack = 6,
                    Defense = 2,
                    GoldReward = 18,
                    ItemReward = ItemFactory.CreatePotion(PotionType.Health, ItemRarity.Common)
                }
                : new Enemy
                {
                    Name = "Cinder Mine Stalker",
                    MaxHealth = 34,
                    Health = 34,
                    Attack = 8,
                    Defense = 4,
                    GoldReward = 30,
                    ItemReward = ItemFactory.CreateWeapon(WeaponCategory.Dagger, Material.Bronze, ItemRarity.Uncommon)
                };

            return new Encounter
            {
                Id = $"combat-{location.Id}-{Dice.Roll(100000)}",
                Name = enemy.Name,
                Description = roaming
                    ? "A hostile figure steps onto the road and reaches for a battered blade."
                    : $"Something dangerous moves in the depths of {location.Name}.",
                Type = EncounterType.Combat,
                Enemy = enemy
            };
        }

        public static Encounter CreateNpcEncounter(WorldLocation location, bool roaming = false)
        {
            var quest = new Quest
            {
                Id = $"quest-{location.Id}-{Dice.Roll(100000)}",
                Title = roaming ? "A Stranger's Request" : "A Local Problem",
                Description = roaming
                    ? "Deal with the danger on the roads and return with proof of your courage."
                    : $"Help the people of {location.Name} by surviving one dangerous encounter.",
                RequiredEncounters = 1,
                GoldReward = roaming ? 25 : 35,
                ItemReward = ItemFactory.CreatePotion(PotionType.Health, ItemRarity.Uncommon)
            };

            var enemy = new Enemy
            {
                Name = roaming ? "Roadside Scout" : "Local Duelist",
                MaxHealth = roaming ? 18 : 22,
                Health = roaming ? 18 : 22,
                Attack = roaming ? 5 : 6,
                Defense = roaming ? 2 : 3,
                GoldReward = roaming ? 12 : 16,
                ItemReward = ItemFactory.CreatePotion(PotionType.Health, ItemRarity.Common)
            };

            return new Encounter
            {
                Id = $"npc-{location.Id}-{Dice.Roll(100000)}",
                Name = roaming ? "Traveling Scout" : "Concerned Local",
                Description = roaming
                    ? "A traveling scout asks whether you can help keep the road safe."
                    : "A local approaches with a problem that needs an adventurer's attention.",
                Type = EncounterType.Npc,
                Enemy = enemy,
                Quest = quest
            };
        }

        private static Encounter CreateTreasureEncounter(WorldLocation location)
        {
            return new Encounter
            {
                Id = $"treasure-{location.Id}-{Dice.Roll(100000)}",
                Name = "Hidden Cache",
                Description = $"You discover a concealed cache near {location.Name}.",
                Type = EncounterType.Treasure,
                GoldReward = Dice.Roll(16) + 9,
                ItemReward = ItemFactory.CreatePotion(PotionType.Health, ItemRarity.Common)
            };
        }

        private static Encounter CreateTrapEncounter(WorldLocation location)
        {
            return new Encounter
            {
                Id = $"trap-{location.Id}-{Dice.Roll(100000)}",
                Name = "Hidden Trap",
                Description = $"A hidden mechanism snaps into action while you search {location.Name}.",
                Type = EncounterType.Trap
            };
        }
    }
}
