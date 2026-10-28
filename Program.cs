using System;
using NewWorldPP2.Entities;
using NewWorldPP2.Persistence;
using NewWorldPP2.Services;
using NewWorldPP2.UI;

namespace NewWorldPP2
{
    class Program
    {
        static void Main(string[] args)
        {
            PlayerCharacter? player = null;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Character Menu ===");
                Console.WriteLine("1) Create New Character");
                Console.WriteLine("2) Customize Character");
                Console.WriteLine("3) View Character");
                Console.WriteLine("4) Save Character");
                Console.WriteLine("5) Load Character");
                Console.WriteLine("6) Delete Save");
                Console.WriteLine("7) Start Fantasy World");
                Console.WriteLine("8) Exit");
                Console.Write("Choose an option: ");
                var key = Console.ReadLine() ?? string.Empty;

                switch (key)
                {
                    case "1":
                        player = CharacterCreation.CreateCharacter();
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
                            if (SaveSystem.SaveExists(user))
                            {
                                Console.WriteLine("A save with that username already exists. Choose a different username.");
                            }
                            else
                            {
                                var ok = SaveSystem.SaveCharacter(player, user, pass);
                                Console.WriteLine(ok ? "Saved successfully." : "Save failed.");
                            }
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
                        var loaded = SaveSystem.LoadCharacter(loadUser, loadPass);
                        if (loaded == null)
                        {
                            Console.WriteLine("Failed to load save (incorrect user/password or missing save). Press Enter...");
                            Console.ReadLine();
                            Console.Clear();
                        }
                        else
                        {
                            player = loaded;
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
                            WorldExplorer.Enter(player);
                        }
                        break;
                    case "8":
                        return;
                        default:
                            Console.WriteLine("Invalid option. Press Enter to continue...");
                            Console.ReadLine();
                            Console.Clear();
                            break;
                }
            }
        }

        // Minimal password masking for console input
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

