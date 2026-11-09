using System.Text.Json.Serialization;
using NewWorldPP2.Entities;

namespace NewWorldPP2.Services.Multiplayer;

public enum MultiplayerMode
{
    Local,
    Lan,
    Online
}

public enum MultiplayerMessageType
{
    Hello,
    Welcome,
    PlayerJoined,
    PlayerUpdated,
    PlayerLeft,
    Snapshot,
    Chat,
    Error,
    Goodbye
}

public sealed class MultiplayerMessage
{
    public MultiplayerMessageType Type { get; set; }
    public string PlayerId { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public PlayerState? Player { get; set; }
    public List<PlayerState> Players { get; set; } = new();
}

public sealed class PlayerState
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string LocationId { get; set; } = "silvermere-tavern";
    public PlayerCharacter? Character { get; set; }
}
