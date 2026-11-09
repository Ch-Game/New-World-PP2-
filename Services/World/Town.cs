using System.Collections.Generic;

namespace NewWorldPP2.Services.World
{
    // Groups related locations under a named settlement.
    public class Town
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> LocationIds { get; set; } = new();
    }
}
