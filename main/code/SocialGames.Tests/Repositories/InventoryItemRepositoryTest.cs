namespace Microsoft.Samples.SocialGames.Tests.Repositories
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using Microsoft.Samples.SocialGames.Entities;
    using Microsoft.Samples.SocialGames.Repositories;
    using Microsoft.Samples.SocialGames.Tests.Repositories;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.StorageClient;

    [TestClass]
    public class InventoryItemRepositoryTest : RepositoryTest
    {
        private static string inventoryContainerName = "inventorytest";
        private static string emptyContainerName = "emptycontainertest";
        private static InventoryItem[] initialItems = new[]
        {
            new InventoryItem { Id = new Guid("1b6e4f64-0eee-4a86-9d7b-9cd667b1ec46"), Name = "Better Weapon 1", SmlIcon = "someimage.jpg", Price = 12, Type = "Tank" },
            new InventoryItem { Id = new Guid("dd9752fd-1117-44df-8bae-3f0e62393600"), Name = "Better Weapon 2", SmlIcon = "someimage.jpg", Price = 4, Type = "Tank" },
            new InventoryItem { Id = new Guid("9a4a45cd-da7f-4597-9892-4ec70fa4be97"), Name = "Better Weapon 3", SmlIcon = "someimage.jpg", Price = 7, Type = "Tank" },
            new InventoryItem { Id = new Guid("bd43aa1e-3cc1-4273-bc1f-a133e9d47d3c"), Name = "Better Tank 1", SmlIcon = "someimage.jpg", Price = 2, Type = "Tank" },
            new InventoryItem { Id = new Guid("1e3cf1ff-96fe-4375-a6e1-99fa4423bf0c"), Name = "Better Tank 2", SmlIcon = "someimage.jpg", Price = 4, Type = "Tank" },
            new InventoryItem { Id = new Guid("b2cbd0e0-ed2b-4740-88ad-db38106dcacd"), Name = "Better Tank 3", SmlIcon = "someimage.jpg", Price = 54, Type = "Tank" },
            new InventoryItem { Id = new Guid("021ca00e-774f-423d-8165-e4bc37b4a2a6"), Name = "Custom Tank Image", SmlIcon = "someimage.jpg", Price = 453, Type = "Tank" },
            new InventoryItem { Id = new Guid("a6128ee2-6460-4ef6-9b20-fdc8e223d3c5"), Name = "Image To Prank", SmlIcon = "someimage.jpg", Price = 54, Type = "Tank" }
        };

        [ClassInitialize]
        public static void MyClassInitialize(TestContext context)
        {
            new EnsureWindowsAzureStorageEmulatorIsRunning().DoIt();
        }

        [TestCleanup]
        public void MyClassCleanup()
        {
            Cleanup();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InventoryRepositoryConstructorWithNullAccount()
        {
            new InventoryRepository(account: null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InventoryRepositoryConstructorWithNullBlobContainer()
        {
            new InventoryRepository(inventoryContainer: null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InventoryRepositoryConstructorWithNullAccountAndContainer()
        {
            new InventoryRepository(null, inventoryContainerName);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InventoryRepositoryConstructorWithNullInventoryContainerName()
        {
            new InventoryRepository(CloudStorageAccount.DevelopmentStorageAccount, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InventoryRepositoryConstructorWithEmptyInventoryContainerName()
        {
            new InventoryRepository(CloudStorageAccount.DevelopmentStorageAccount, string.Empty);
        }

        [TestMethod]
        public void GetInventoryItem()
        {
            var itemId = initialItems[2].Id;
            var target = new InventoryRepository(CloudStorageAccount.DevelopmentStorageAccount, inventoryContainerName);
            target.AddOrUpdateInventoryItems(initialItems);
            
            var item = target.GetInventoryItem(itemId);

            Assert.AreEqual(itemId, item.Id);
            Assert.AreEqual(initialItems[2].SmlIcon, item.SmlIcon);
            Assert.AreEqual(initialItems[2].Name, item.Name);
            Assert.AreEqual(initialItems[2].Price, item.Price);
            Assert.AreEqual(initialItems[2].Type, item.Type);
        }

        [TestMethod]
        public void GetInventoryItemReturnsNullIfItemDoesNotExist()
        {
            var itemId = Guid.NewGuid();
            var target = new InventoryRepository(CloudStorageAccount.DevelopmentStorageAccount, inventoryContainerName);
            target.AddOrUpdateInventoryItems(initialItems);
            
            var item = target.GetInventoryItem(itemId);

            Assert.IsNull(item);
        }

        [TestMethod]
        public void GetInventoryItemReturnsNullIfBlobDoesNotExist()
        {
            var itemId = Guid.NewGuid();
            var target = new InventoryRepository(CloudStorageAccount.DevelopmentStorageAccount, emptyContainerName);
            var item = target.GetInventoryItem(itemId);

            Assert.IsNull(item);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetInventoryItemThorwsExceptionWhenInventoryItemIdIsEmpty()
        {
            var target = new InventoryRepository(CloudStorageAccount.DevelopmentStorageAccount, inventoryContainerName);
            target.GetInventoryItem(Guid.Empty);
        }

        [TestMethod]
        public void GetAllInventoryItems()
        {
            var target = new InventoryRepository(CloudStorageAccount.DevelopmentStorageAccount, inventoryContainerName);
            target.AddOrUpdateInventoryItems(initialItems);

            var items = target.GetAllInventoryItems();

            Assert.IsNotNull(items);
            Assert.AreEqual(initialItems.Count(), items.Count());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddOrUpdateInventoryItemsThrowsExceptionIfArrayIsNull()
        {
            var target = new InventoryRepository(CloudStorageAccount.DevelopmentStorageAccount, inventoryContainerName);
            target.AddOrUpdateInventoryItems(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddOrUpdateInventoryImageThrowsExceptionIfImageNameIsNullOrEmpty()
        {
            var target = new InventoryRepository(CloudStorageAccount.DevelopmentStorageAccount, inventoryContainerName);
            target.AddOrUpdateInventoryImage(string.Empty, new byte[] { });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddOrUpdateInventoryImageThrowsExceptionIfArrayIsNull()
        {
            var target = new InventoryRepository(CloudStorageAccount.DevelopmentStorageAccount, inventoryContainerName);
            target.AddOrUpdateInventoryImage("Image", null);
        }

        [TestMethod]
        [DeploymentItem(@"Resources\CharactersGeneral.jpg")]
        public void AddOrUpdateInventoryImages()
        {
            var target = new InventoryRepository(CloudStorageAccount.DevelopmentStorageAccount, inventoryContainerName);

            var image = File.ReadAllBytes("CharactersGeneral.jpg");

            var imageUrl = target.AddOrUpdateInventoryImage("General", image);

            var client = new WebClient();
            var actualImage = client.DownloadData(imageUrl);

            Assert.IsNotNull(image);
            Assert.AreEqual(image.Count(), actualImage.Count());
        }

        [TestMethod]
        public void InventoryRepositoryDefaultConstructor()
        {
            bool wasSetterCalled = false;
            CloudStorageAccount.SetConfigurationSettingPublisher((configName, configSetter) =>
                {
                    if (configName == "DataConnectionString")
                    {
                        wasSetterCalled = true;
                        configSetter("UseDevelopmentStorage=true");
                    }
            });

            new InventoryRepository();
            Assert.IsTrue(wasSetterCalled);
        }

        private static void Cleanup()
        {
            var client = CloudStorageAccount.DevelopmentStorageAccount.CreateCloudBlobClient();

            try
            {
                var inventoryContainer = client.GetContainerReference(inventoryContainerName);
                inventoryContainer.Delete();

                var emptyContainer = client.GetContainerReference(emptyContainerName);
                emptyContainer.Delete();
            }
            catch (StorageClientException)
            { 
            }
        }
    }
}