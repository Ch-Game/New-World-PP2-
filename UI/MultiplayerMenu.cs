using System.Net.Sockets;
using NewWorldPP2.Entities;
using NewWorldPP2.Services.Multiplayer;

namespace NewWorldPP2.UI;

public static class MultiplayerMenu
{
    public static void Enter(PlayerCharacter character)
    {
        Console.Clear();
        Console.WriteLine("=== Multiplayer ===");
        Console.WriteLine("1) Host a game");
        Console.WriteLine("2) Join a game");
        Console.WriteLine("3) Back");
        Console.Write("Choose an option: ");

        switch (Console.ReadLine())
        {
            case "1":
                RunHost(character).GetAwaiter().GetResult();
                break;
            case "2":
                RunClient(character).GetAwaiter().GetResult();
                break;
        }
    }

    private static async Task RunHost(PlayerCharacter character)
    {
        var mode = ChooseMode();
        var port = ReadPort();
        try
        {
            Console.Clear();
            await using var session = await MultiplayerSession.StartHostAsync(mode, port);
            await session.SetPlayerStateAsync(character, "silvermere-tavern");
            Console.WriteLine($"Hosting {mode} multiplayer on port {port}.");
            Console.WriteLine("Share your LAN address for LAN play, or your public address for online play.");
            await RunLobby(session, character);
        }
        catch (SocketException ex)
        {
            Console.Clear();
            Console.WriteLine($"Could not start the host: {ex.Message}");
            Pause();
        }
    }

    private static async Task RunClient(PlayerCharacter character)
    {
        var mode = ChooseMode();
        var address = mode == MultiplayerMode.Local ? "127.0.0.1" : ReadAddress();
        var port = ReadPort();
        try
        {
            Console.Clear();
            Console.WriteLine("Connecting...");
            await using var session = await MultiplayerSession.ConnectAsync(mode, address, port, character);
            await session.SetPlayerStateAsync(character, "silvermere-tavern");
            Console.WriteLine($"Connected to {address}:{port}.");
            await RunLobby(session, character);
        }
        catch (Exception ex) when (ex is SocketException or IOException or TimeoutException)
        {
            Console.Clear();
            Console.WriteLine($"Could not connect: {ex.Message}");
            Pause();
        }
    }

    private static async Task RunLobby(MultiplayerSession session, PlayerCharacter character)
    {
        session.MessageReceived += message =>
        {
            Console.Clear();
            if (message.Type == MultiplayerMessageType.PlayerJoined && message.Player != null)
                Console.WriteLine($"\n{message.Player.Name} joined the lobby.");
            else if (message.Type == MultiplayerMessageType.PlayerLeft)
                Console.WriteLine($"\nA player left the lobby.");
            else if (message.Type == MultiplayerMessageType.Chat)
                Console.WriteLine($"\nChat: {message.Text}");
        };

        while (true)
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("1) Show players");
            Console.WriteLine("2) Send chat");
            Console.WriteLine("3) Update location");
            Console.WriteLine("4) Leave session");
            Console.Write("Choose an option: ");
            switch (Console.ReadLine())
            {
                case "1":
                    ShowPlayers(session);
                    break;
                case "2":
                    Console.Write("Message: ");
                    await session.SendChatAsync(Console.ReadLine() ?? string.Empty);
                    break;
                case "3":
                    Console.Write("Location ID: ");
                    var location = Console.ReadLine()?.Trim();
                    if (!string.IsNullOrWhiteSpace(location))
                    {
                        await session.SetPlayerStateAsync(character, location);
                        Console.WriteLine("Location synchronized.");
                    }
                    break;
                case "4":
                    return;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
    }

    private static void ShowPlayers(MultiplayerSession session)
    {
        Console.WriteLine("--- Players ---");
        foreach (var player in session.Players)
            Console.WriteLine($"{player.Name} ({player.LocationId}) - {player.Character?.Class}");
    }

    private static MultiplayerMode ChooseMode()
    {
        while (true)
        {
            Console.WriteLine("1) Local (same computer)");
            Console.WriteLine("2) LAN");
            Console.WriteLine("3) Online");
            Console.Write("Network mode: ");
            switch (Console.ReadLine())
            {
                case "1": return MultiplayerMode.Local;
                case "2": return MultiplayerMode.Lan;
                case "3": return MultiplayerMode.Online;
                default: Console.WriteLine("Choose 1, 2, or 3."); break;
            }
        }
    }

    private static int ReadPort()
    {
        Console.Write("Port (default 5050): ");
        return int.TryParse(Console.ReadLine(), out var port) && port is > 0 and <= 65535 ? port : 5050;
    }

    private static string ReadAddress()
    {
        Console.Write("Host address: ");
        return Console.ReadLine()?.Trim() ?? string.Empty;
    }

    private static void Pause()
    {
        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
    }
}
