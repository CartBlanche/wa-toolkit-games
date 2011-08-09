namespace Microsoft.Samples.SocialGames.Repositories
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Samples.SocialGames.Entities;

    public interface IInventoryRepository
    {
        InventoryItem GetInventoryItem(Guid inventoryItemId);

        IEnumerable<InventoryItem> GetAllInventoryItems();

        void AddOrUpdateInventoryItems(InventoryItem[] inventoryItems);

        string AddOrUpdateInventoryImage(string imageName, byte[] imageData);
    }
}