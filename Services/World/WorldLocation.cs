using System.Collections.Generic;

namespace NewWorldPP2.Services.World
{
    public class WorldLocation
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string TownName { get; set; } = string.Empty;
        public LocationType Type { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<string> Connections { get; set; } = new();
        public List<string> Features { get; set; } = new();
        public List<WorldInteraction> Interactions { get; set; } = new();
    }
}
