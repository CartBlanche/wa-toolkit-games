namespace Microsoft.Samples.SocialGames.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Samples.SocialGames;
    using Microsoft.Samples.SocialGames.Common.Storage;
    using Microsoft.Samples.SocialGames.Entities;
    using Microsoft.WindowsAzure;

    public class InventoryRepository : IInventoryRepository
    {
        private readonly IAzureBlobContainer<InventoryItem[]> inventoryContainer;

        public InventoryRepository()
            : this(CloudStorageAccount.FromConfigurationSetting("DataConnectionString"))
        { 
        }

        public InventoryRepository(CloudStorageAccount account)
            : this(account, ConfigurationConstants.InventoryContainerName)
        { 
        }

        public InventoryRepository(CloudStorageAccount account, string inventoryContainerName)
            : this(new AzureBlobContainer<InventoryItem[]>(account, inventoryContainerName, true))
        {
        }

        public InventoryRepository(IAzureBlobContainer<InventoryItem[]> inventoryContainer)
        {
            if (inventoryContainer == null)
            {
                throw new ArgumentNullException("inventoryContainer");
            }

            this.inventoryContainer = inventoryContainer;

            this.inventoryContainer.EnsureExist(true);
        }

        public InventoryItem GetInventoryItem(Guid inventoryItemId)
        {
            if (inventoryItemId == Guid.Empty)
            {
                throw new ArgumentException("Inventory Item Id cannot be empty");
            }

            var items = this.GetAllInventoryItems();

            return
                items != null ?
                    items.FirstOrDefault(i => i.Id == inventoryItemId) :
                    null;
        }

        public IEnumerable<InventoryItem> GetAllInventoryItems()
        {
            return this.inventoryContainer.Get(ConfigurationConstants.InventoryItemsBlobName);
        }

        public void AddOrUpdateInventoryItems(InventoryItem[] inventoryItems)
        {
            if (inventoryItems == null)
            {
                throw new ArgumentNullException("inventoryItems");
            }

            this.inventoryContainer.Save(ConfigurationConstants.InventoryItemsBlobName, inventoryItems);
        }

        public string AddOrUpdateInventoryImage(string imageName, byte[] imageData)
        {
            if (string.IsNullOrWhiteSpace(imageName))
            {
                throw new ArgumentNullException("imageName");
            }
  
            if (imageData == null)
            {
                throw new ArgumentNullException("imageData");
            }

            return this.inventoryContainer.SaveFile("images/" + imageName, imageData, "image/jpeg");   
        }
    }
}