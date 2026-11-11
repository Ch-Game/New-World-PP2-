using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using NewWorldPP2.Entities;
using NewWorldPP2.Services.World;

namespace NewWorldPP2.Persistence
{
    // Provides file-based character saves with password verification.
    public static class SaveSystem
    {
        private const string SaveFolder = "Saves";

        public sealed class LoadedGame
        {
            public PlayerCharacter Character { get; init; } = new();
            public WorldState WorldState { get; init; } = new();
            public List<Quest> Quests { get; init; } = new();
        }

        private sealed class SaveData
        {
            public string Username { get; set; } = string.Empty;
            public string PasswordHash { get; set; } = string.Empty;
            public PlayerCharacter? Character { get; set; }
            public WorldState? WorldState { get; set; }
            public List<Quest>? Quests { get; set; }
        }

        // Serializes a new character save without overwriting an existing account.
        public static bool SaveCharacter(NewWorldPP2.Entities.PlayerCharacter pc, string username, string password)
        {
            if (pc == null) return false;
            if (string.IsNullOrWhiteSpace(pc.Name) || string.IsNullOrWhiteSpace(username)) return false;

            try
            {
                Directory.CreateDirectory(SaveFolder);
                var file = Path.Combine(SaveFolder, Sanitize(username) + ".json");
                if (File.Exists(file)) return false; // do not overwrite

                var data = new SaveData
                {
                    Username = username,
                    PasswordHash = ComputeHash(username, password),
                    Character = pc
                };

                WriteSave(file, data, overwrite: false);
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Creates a new save containing all currently playable game state.
        public static bool SaveGame(PlayerCharacter character, string username, string password,
            WorldState worldState, List<Quest> quests)
        {
            return WriteGame(character, username, password, worldState, quests, overwrite: false);
        }

        // Updates an existing save after authenticating the account credentials.
        public static bool UpdateGame(PlayerCharacter character, string username, string password,
            WorldState worldState, List<Quest> quests)
        {
            return WriteGame(character, username, password, worldState, quests, overwrite: true);
        }

        // Checks whether a save file exists for the supplied username.
        public static bool SaveExists(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return false;
            var file = Path.Combine(SaveFolder, Sanitize(username) + ".json");
            return File.Exists(file);
        }

        // Loads and authenticates a saved character before restoring derived statistics.
        public static NewWorldPP2.Entities.PlayerCharacter? LoadCharacter(string username, string password)
        {
            return LoadGame(username, password)?.Character;
        }

        // Loads and authenticates the complete game state. Older character-only saves are supported.
        public static LoadedGame? LoadGame(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username)) return null;

            try
            {
                var file = Path.Combine(SaveFolder, Sanitize(username) + ".json");
                if (!File.Exists(file)) return null;

                var opts = new JsonSerializerOptions();
                opts.Converters.Add(new JsonStringEnumConverter());
                var json = File.ReadAllText(file, Encoding.UTF8);
                var data = JsonSerializer.Deserialize<SaveData>(json, opts);
                if (data == null) return null;

                var expected = ComputeHash(data.Username ?? string.Empty, password);
                if (!string.Equals(expected, data.PasswordHash ?? string.Empty, StringComparison.OrdinalIgnoreCase)) return null;

                var character = data.Character;
                if (character == null) return null;

                character.NormalizeProgression();
                character.RecalculateDerivedStats();
                return new LoadedGame
                {
                    Character = character,
                    WorldState = data.WorldState ?? new WorldState(),
                    Quests = data.Quests ?? new List<Quest>()
                };
            }
            catch
            {
                return null;
            }
        }

        private static bool WriteGame(PlayerCharacter character, string username, string password,
            WorldState worldState, List<Quest> quests, bool overwrite)
        {
            if (character == null || string.IsNullOrWhiteSpace(character.Name) || string.IsNullOrWhiteSpace(username)) return false;
            if (worldState == null || quests == null) return false;

            try
            {
                Directory.CreateDirectory(SaveFolder);
                var file = Path.Combine(SaveFolder, Sanitize(username) + ".json");
                if (overwrite)
                {
                    var existing = ReadSave(file);
                    if (existing == null || !PasswordsMatch(existing, username, password)) return false;
                }
                else if (File.Exists(file))
                {
                    return false;
                }

                WriteSave(file, new SaveData
                {
                    Username = username,
                    PasswordHash = ComputeHash(username, password),
                    Character = character,
                    WorldState = worldState,
                    Quests = quests
                }, overwrite);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static SaveData? ReadSave(string file)
        {
            if (!File.Exists(file)) return null;
            return JsonSerializer.Deserialize<SaveData>(File.ReadAllText(file, Encoding.UTF8), CreateOptions());
        }

        private static bool PasswordsMatch(SaveData data, string username, string password)
        {
            return string.Equals(data.Username, username, StringComparison.OrdinalIgnoreCase)
                && string.Equals(ComputeHash(data.Username ?? string.Empty, password), data.PasswordHash ?? string.Empty, StringComparison.OrdinalIgnoreCase);
        }

        private static void WriteSave(string file, SaveData data, bool overwrite)
        {
            if (!overwrite && File.Exists(file)) throw new IOException("Save already exists.");

            var tempFile = file + ".tmp";
            var json = JsonSerializer.Serialize(data, CreateOptions());
            File.WriteAllText(tempFile, json, Encoding.UTF8);
            if (overwrite)
            {
                File.Move(tempFile, file, true);
            }
            else
            {
                File.Move(tempFile, file);
            }
        }

        private static JsonSerializerOptions CreateOptions()
        {
            var opts = new JsonSerializerOptions { WriteIndented = true };
            opts.Converters.Add(new JsonStringEnumConverter());
            return opts;
        }

        // Deletes a save only after verifying the associated password.
        public static bool DeleteSave(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username)) return false;

            try
            {
                var file = Path.Combine(SaveFolder, Sanitize(username) + ".json");
                if (!File.Exists(file)) return false;

                var opts = new JsonSerializerOptions();
                opts.Converters.Add(new JsonStringEnumConverter());
                var json = File.ReadAllText(file, Encoding.UTF8);
                var data = JsonSerializer.Deserialize<SaveData>(json, opts);
                if (data == null) return false;

                var expected = ComputeHash(data.Username ?? string.Empty, password);
                if (!string.Equals(expected, data.PasswordHash ?? string.Empty, StringComparison.OrdinalIgnoreCase)) return false;

                File.Delete(file);
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Converts a username into a safe, stable file-name component.
        private static string Sanitize(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return string.Empty;
            var invalid = Path.GetInvalidFileNameChars();
            var sb = new StringBuilder(username.Length);
            foreach (var c in username)
            {
                sb.Append(Array.IndexOf(invalid, c) >= 0 ? '_' : c);
            }
            return sb.ToString().ToLowerInvariant();
        }

        // Creates the stored password digest from the username and password.
        private static string ComputeHash(string username, string password)
        {
            using var sha = SHA256.Create();
            var combined = (username ?? string.Empty) + ":" + (password ?? string.Empty);
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(combined));
            var sb = new StringBuilder(hash.Length * 2);
            foreach (var b in hash) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}
