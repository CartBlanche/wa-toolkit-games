namespace Microsoft.Samples.SocialGames.Entities
{
    using System;

    public class InventoryItem
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string SmlIcon { get; set; }

        public string LargIcon { get; set; }

        public int OffsetX { get; set; }

        public int OffsetY { get; set; }

        public int Price { get; set; }

        public string Type { get; set; }
    }
}