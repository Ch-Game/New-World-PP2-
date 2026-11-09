using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using NewWorldPP2.Entities;

namespace NewWorldPP2.Services.Multiplayer;

public sealed class MultiplayerSession : IAsyncDisposable
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        Converters = { new JsonStringEnumConverter() }
    };
    private readonly ConcurrentDictionary<string, ClientConnection> _clients = new();
    private readonly ConcurrentDictionary<string, PlayerState> _players = new();
    private readonly CancellationTokenSource _shutdown = new();
    private TcpListener? _listener;
    private TcpClient? _serverConnection;
    private StreamWriter? _serverWriter;
    private string? _localPlayerId;
    private readonly TaskCompletionSource<bool> _welcomeReceived = new(TaskCreationOptions.RunContinuationsAsynchronously);

    private MultiplayerSession(bool isHost, MultiplayerMode mode, int port)
    {
        IsHost = isHost;
        Mode = mode;
        Port = port;
    }

    public bool IsHost { get; }
    public MultiplayerMode Mode { get; }
    public int Port { get; }
    public IReadOnlyCollection<PlayerState> Players => _players.Values.ToArray();
    public event Action<MultiplayerMessage>? MessageReceived;

    public static async Task<MultiplayerSession> StartHostAsync(MultiplayerMode mode, int port = 5050)
    {
        var session = new MultiplayerSession(true, mode, port);
        session._listener = new TcpListener(IPAddress.Any, port);
        session._listener.Start();
        _ = session.AcceptClientsAsync();
        await Task.CompletedTask;
        return session;
    }

    public static async Task<MultiplayerSession> ConnectAsync(
        MultiplayerMode mode,
        string address,
        int port,
        PlayerCharacter character)
    {
        var session = new MultiplayerSession(false, mode, port);
        session._serverConnection = new TcpClient();
        await session._serverConnection.ConnectAsync(address, port);
        session._serverWriter = new StreamWriter(session._serverConnection.GetStream(), new UTF8Encoding(false))
        {
            AutoFlush = true
        };

        var hello = new MultiplayerMessage
        {
            Type = MultiplayerMessageType.Hello,
            Player = new PlayerState { Name = character.Name, Character = character }
        };
        await session.SendAsync(session._serverWriter, hello);
        _ = session.ReadServerMessagesAsync(session._serverConnection);
        await session._welcomeReceived.Task.WaitAsync(TimeSpan.FromSeconds(10));
        return session;
    }

    public async Task SetPlayerStateAsync(PlayerCharacter character, string locationId)
    {
        var player = new PlayerState
        {
            Id = _localPlayerId ?? string.Empty,
            Name = character.Name,
            Character = character,
            LocationId = locationId
        };

        if (IsHost)
        {
            if (_localPlayerId == null) _localPlayerId = Guid.NewGuid().ToString("N");
            player.Id = _localPlayerId;
            _players[player.Id] = player;
            Broadcast(new MultiplayerMessage { Type = MultiplayerMessageType.PlayerUpdated, Player = player });
            return;
        }

        if (_localPlayerId == null || _serverWriter == null) return;
        await SendAsync(_serverWriter, new MultiplayerMessage
        {
            Type = MultiplayerMessageType.PlayerUpdated,
            PlayerId = _localPlayerId,
            Player = player
        });
    }

    public async Task SendChatAsync(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;
        var message = new MultiplayerMessage { Type = MultiplayerMessageType.Chat, Text = text.Trim() };
        if (IsHost)
        {
            Broadcast(message);
        }
        else if (_serverWriter != null)
        {
            await SendAsync(_serverWriter, message);
        }
    }

    private async Task AcceptClientsAsync()
    {
        if (_listener == null) return;
        try
        {
            while (!_shutdown.IsCancellationRequested)
            {
                var tcpClient = await _listener.AcceptTcpClientAsync(_shutdown.Token);
                _ = HandleClientAsync(tcpClient);
            }
        }
        catch (OperationCanceledException) { }
        catch (ObjectDisposedException) { }
    }

    private async Task HandleClientAsync(TcpClient tcpClient)
    {
        var connection = new ClientConnection(tcpClient);
        try
        {
            using var reader = new StreamReader(tcpClient.GetStream(), Encoding.UTF8);
            connection.Writer = new StreamWriter(tcpClient.GetStream(), new UTF8Encoding(false)) { AutoFlush = true };
            var hello = await ReadAsync(reader, _shutdown.Token);
            if (hello?.Type != MultiplayerMessageType.Hello || hello.Player == null) return;

            var playerId = Guid.NewGuid().ToString("N");
            hello.Player.Id = playerId;
            connection.PlayerId = playerId;
            _clients[playerId] = connection;
            _players[playerId] = hello.Player;

            await SendAsync(connection.Writer, new MultiplayerMessage
            {
                Type = MultiplayerMessageType.Welcome,
                PlayerId = playerId,
                Players = Players.ToList()
            });
            Broadcast(new MultiplayerMessage { Type = MultiplayerMessageType.PlayerJoined, Player = hello.Player }, playerId);

            while (!_shutdown.IsCancellationRequested)
            {
                var message = await ReadAsync(reader, _shutdown.Token);
                if (message == null) break;
                ProcessClientMessage(message, playerId);
            }
        }
        catch (IOException) { }
        catch (OperationCanceledException) { }
        finally
        {
            if (connection.PlayerId != null && _clients.TryRemove(connection.PlayerId, out _))
            {
                _players.TryRemove(connection.PlayerId, out _);
                Broadcast(new MultiplayerMessage
                {
                    Type = MultiplayerMessageType.PlayerLeft,
                    PlayerId = connection.PlayerId
                });
            }
            tcpClient.Dispose();
        }
    }

    private void ProcessClientMessage(MultiplayerMessage message, string playerId)
    {
        if (message.Type == MultiplayerMessageType.PlayerUpdated && message.Player != null)
        {
            message.Player.Id = playerId;
            _players[playerId] = message.Player;
            Broadcast(new MultiplayerMessage
            {
                Type = MultiplayerMessageType.PlayerUpdated,
                Player = message.Player
            }, playerId);
        }
        else if (message.Type == MultiplayerMessageType.Chat)
        {
            Broadcast(new MultiplayerMessage
            {
                Type = MultiplayerMessageType.Chat,
                PlayerId = playerId,
                Text = message.Text
            });
        }
    }

    private async Task ReadServerMessagesAsync(TcpClient tcpClient)
    {
        try
        {
            using var reader = new StreamReader(tcpClient.GetStream(), Encoding.UTF8);
            while (!_shutdown.IsCancellationRequested)
            {
                var message = await ReadAsync(reader, _shutdown.Token);
                if (message == null) break;
                if (message.Type == MultiplayerMessageType.Welcome)
                {
                    _localPlayerId = message.PlayerId;
                    foreach (var player in message.Players) _players[player.Id] = player;
                    _welcomeReceived.TrySetResult(true);
                }
                else if (message.Player != null)
                {
                    _players[message.Player.Id] = message.Player;
                }
                else if (message.Type == MultiplayerMessageType.PlayerLeft)
                {
                    _players.TryRemove(message.PlayerId, out _);
                }
                MessageReceived?.Invoke(message);
            }
        }
        catch (IOException) { }
        catch (OperationCanceledException) { }
    }

    private void Broadcast(MultiplayerMessage message, string? excludedPlayerId = null)
    {
        MessageReceived?.Invoke(message);
        foreach (var pair in _clients)
        {
            if (pair.Key == excludedPlayerId || pair.Value.Writer == null) continue;
            _ = SendAsync(pair.Value.Writer, message);
        }
    }

    private async Task SendAsync(StreamWriter writer, MultiplayerMessage message)
    {
        var json = JsonSerializer.Serialize(message, _jsonOptions);
        await writer.WriteLineAsync(json);
    }

    private async Task<MultiplayerMessage?> ReadAsync(StreamReader reader, CancellationToken cancellationToken)
    {
        var line = await reader.ReadLineAsync(cancellationToken);
        return string.IsNullOrWhiteSpace(line) ? null : JsonSerializer.Deserialize<MultiplayerMessage>(line, _jsonOptions);
    }

    public async ValueTask DisposeAsync()
    {
        _shutdown.Cancel();
        _listener?.Stop();
        _serverConnection?.Dispose();
        foreach (var client in _clients.Values) client.Client.Dispose();
        await Task.CompletedTask;
        _shutdown.Dispose();
    }

    private sealed class ClientConnection
    {
        public ClientConnection(TcpClient client) => Client = client;
        public TcpClient Client { get; }
        public string? PlayerId { get; set; }
        public StreamWriter? Writer { get; set; }
    }
}
