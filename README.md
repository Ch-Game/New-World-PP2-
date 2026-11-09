## Multiplayer

The main menu now includes a Multiplayer option. The networking layer uses a JSON-lines TCP session shared by all three modes:

- **Local**: run the host and joining client on the same computer using `127.0.0.1`.
- **LAN**: host on one computer and connect using the host computer's local IP address. Allow the selected TCP port through the host's firewall.
- **Online**: connect using a public host name or IP address. The host must expose/forward the selected TCP port, or a relay service must be added later.

The lobby synchronizes player joins and leaves, character data, current location IDs, and chat messages. The existing world, combat, inventory, and save systems still run locally; authoritative server commands for those systems can be added on top of the session protocol.

Quick breakdown

interaction between creatures and civilizations, dynamic weather system, morality system, rich backstory for each civilization, long-term consequences of player actions, magic system tied, character customization, dynamic economy, seamless multiplayer experience, seamless transition between regions, events influenced by player actions and choices.
