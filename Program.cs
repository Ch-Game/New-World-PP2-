using System;
using NewWorldPP2.Entities;
using NewWorldPP2.Persistence;
using NewWorldPP2.Services;
using NewWorldPP2.Services.World;
using NewWorldPP2.UI;

namespace NewWorldPP2
{
    // Coordinates the application's top-level character and world menus.
    class Program
    {
        // Runs the main menu until the player chooses to exit.
        static void Main(string[] args)
        {
            PlayerCharacter? player = null;
            string? saveUsername = null;
            string? savePassword = null;
            var worldState = new WorldState();
            var quests = new List<Quest>();

            while (true)
            {
                Console.Clear();
                ShowTitleScreen();
                Console.WriteLine("=== Character Menu ===");
                Console.WriteLine("1) Create New Character");
                Console.WriteLine("2) Customize Character");
                Console.WriteLine("3) View Character");
                Console.WriteLine("4) Save Character");
                Console.WriteLine("5) Load Character");
                Console.WriteLine("6) Delete Save");
                Console.WriteLine("7) Start Fantasy World");
                Console.WriteLine("8) Multiplayer");
                Console.WriteLine("9) Manage Moves");
                Console.WriteLine("10) Exit");
                Console.Write("Choose an option: ");
                var key = Console.ReadLine() ?? string.Empty;

                switch (key)
                {
                    case "1":
                        player = CharacterCreation.CreateCharacter();
                        saveUsername = null;
                        savePassword = null;
                        worldState = new WorldState();
                        quests = new List<Quest>();
                        break;
                    case "2":
                        if (player == null)
                        {
                            Console.WriteLine("No character yet. Press Enter to continue...");
                            Console.ReadLine();
                            Console.Clear();
                        }
                        else
                        {
                            CharacterCreation.CustomizeCharacter(player);
                        }
                        break;
                    case "3":
                        if (player == null)
                        {
                            Console.WriteLine("No character yet. Press Enter to continue...");
                            Console.ReadLine();
                            Console.Clear();
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine(player.GetFullDescription());
                            Console.WriteLine("Press Enter to continue...");
                            Console.ReadLine();
                            Console.Clear();
                        }
                        break;
                    case "4":
                        if (player == null)
                        {
                            Console.WriteLine("No character to save. Press Enter to continue...");
                            Console.ReadLine();
                            Console.Clear();
                        }
                        else
                        {
                            Console.Write("Enter save username: ");
                            var user = Console.ReadLine()?.Trim() ?? string.Empty;
                            Console.Write("Enter password: ");
                            var pass = ReadPassword();
                            var ok = SaveSystem.SaveExists(user)
                                ? SaveSystem.UpdateGame(player, user, pass, worldState, quests)
                                : SaveSystem.SaveGame(player, user, pass, worldState, quests);
                            if (ok)
                            {
                                saveUsername = user;
                                savePassword = pass;
                            }
                            Console.WriteLine(ok ? "Saved successfully." : "Save failed (incorrect password or invalid save data).");
                            Console.WriteLine("Press Enter to continue...");
                            Console.ReadLine();
                            Console.Clear();
                        }
                        break;
                    case "5":
                        Console.Write("Enter save username: ");
                        var loadUser = Console.ReadLine()?.Trim() ?? string.Empty;
                        Console.Write("Enter password: ");
                        var loadPass = ReadPassword();
                        var loaded = SaveSystem.LoadGame(loadUser, loadPass);
                        if (loaded == null)
                        {
                            Console.WriteLine("Failed to load save (incorrect user/password or missing save). Press Enter...");
                            Console.ReadLine();
                            Console.Clear();
                        }
                        else
                        {
                            player = loaded.Character;
                            worldState = loaded.WorldState;
                            quests = loaded.Quests;
                            saveUsername = loadUser;
                            savePassword = loadPass;
                            Console.WriteLine("Save loaded. Press Enter to continue...");
                            Console.ReadLine();
                            Console.Clear();
                        }
                        break;
                    case "6":
                        Console.Write("Enter save username to delete: ");
                        var deleteUser = Console.ReadLine()?.Trim() ?? string.Empty;
                        Console.Write("Enter password: ");
                        var deletePass = ReadPassword();
                        Console.Write("Type DELETE to confirm: ");
                        var confirmation = Console.ReadLine();
                        if (!string.Equals(confirmation, "DELETE", StringComparison.Ordinal))
                        {
                            Console.WriteLine("Save deletion cancelled.");
                        }
                        else
                        {
                            var deleted = SaveSystem.DeleteSave(deleteUser, deletePass);
                            Console.WriteLine(deleted
                                ? "Save deleted successfully."
                                : "Save deletion failed (incorrect username/password or missing save).");
                        }
                        Console.WriteLine("Press Enter to continue...");
                        Console.ReadLine();
                        Console.Clear();
                        break;
                    case "7":
                        if (player == null)
                        {
                            Console.WriteLine("Create or load a character before entering the fantasy world.");
                            Console.WriteLine("Press Enter to continue...");
                            Console.ReadLine();
                            Console.Clear();
                        }
                        else
                        {
                            WorldExplorer.Enter(player, worldState, quests, () =>
                            {
                                if (saveUsername != null && savePassword != null)
                                {
                                    SaveSystem.UpdateGame(player, saveUsername, savePassword, worldState, quests);
                                }
                            }, () =>
                            {
                                if (saveUsername != null && savePassword != null)
                                {
                                    var deleted = SaveSystem.DeleteSave(saveUsername, savePassword);
                                    Console.WriteLine(deleted
                                        ? "Your character's save has been permanently deleted."
                                        : "The character died, but the save could not be deleted.");
                                }
                                else
                                {
                                    Console.WriteLine("This character had no save file to delete.");
                                }

                                player = null;
                                saveUsername = null;
                                savePassword = null;
                            });
                        }
                        break;
                    case "8":
                        if (player == null)
                        {
                            Console.WriteLine("Create or load a character before entering multiplayer.");
                            Console.WriteLine("Press Enter to continue...");
                            Console.ReadLine();
                            Console.Clear();
                        }
                        else
                        {
                            MultiplayerMenu.Enter(player);
                        }
                        break;
                    case "9":
                        if (player == null)
                        {
                            Console.WriteLine("Create or load a character before managing moves.");
                            Console.WriteLine("Press Enter to continue...");
                            Console.ReadLine();
                            Console.Clear();
                        }
                        else
                        {
                            MoveMenu.Manage(player);
                        }
                        break;
                    case "10":
                        if (player != null && saveUsername != null && savePassword != null)
                        {
                            SaveSystem.UpdateGame(player, saveUsername, savePassword, worldState, quests);
                        }
                        return;
                        default:
                            Console.WriteLine("Invalid option. Press Enter to continue...");
                            Console.ReadLine();
                            Console.Clear();
                            break;
                }
            }
        }

        // Displays the game title and introductory screen.
        private static void ShowTitleScreen()
        {
            Console.WriteLine(@"              *        .        *
         .           _          .
                    / \ 
          /\      __|_|__      /\
         /  \    /   _   \    /  \
        /^^^^\  /   | |   \  /^^^^\
       /      \/   /___\   \/      \
      /________\__|_|_|_|__/________\
             ~~~ NEW WORLD ~~~
                  (PP2)
");
        }

        // Minimal password masking for console input
        // Reads a password without echoing its characters to the console.
        private static string ReadPassword()
        {
            var pass = string.Empty;
            ConsoleKeyInfo key;
            while ((key = Console.ReadKey(true)).Key != ConsoleKey.Enter)
            {
                if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
                {
                    pass = pass[..^1];
                    Console.Write("\b \b");
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    pass += key.KeyChar;
                    Console.Write("*");
                }
            }
            Console.WriteLine();
            return pass;
        }
    }
}

