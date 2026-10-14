using System;
using NewWorldPP2.Core;
using NewWorldPP2.Entities;
using NewWorldPP2.Systems;
using NewWorldPP2.World;

// Minimal console demonstration of systems and methods provided in the workspace.
var player = CharacterCustomization.CreateCharacter("Ari", "Human", "Ranger");
var civ = new Civilization("Verdant Coalition");

Console.WriteLine($"Created player: {player.Name} ({player.Race} {player.Class})");

// Weather affects travel and visibility
var weather = new WeatherSystem();
var world = new WorldState();
weather.Update(world);

// Morality adjusts according to an example action
var morality = new MoralitySystem();
morality.AdjustMorality(player, -5, "Raided a caravan");

// Example interaction between creature and civilization
var wolf = new Creature("Dire Wolf", 30);
wolf.InteractWith(civ);

// Example magic cast
var magic = new MagicSystem();
magic.CastSpell(player, "Guiding Light", 1);

// Example event triggered with dice roll
var evt = new EventSystem();
evt.TriggerEvent("Bandit Ambush", player);

Console.WriteLine("Demo complete. Explore other classes and methods in the Game folder.");

// Keep console open when run interactively
if (Environment.UserInteractive)
{
    Console.WriteLine("Press Enter to exit...");
    Console.ReadLine();
}
