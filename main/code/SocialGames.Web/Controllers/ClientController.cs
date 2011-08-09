namespace Microsoft.Samples.Tankster.GamePlay.Controllers
{
    using System;
    using System.Web.Mvc;
    using Microsoft.Samples.Tankster.GamePlay.Services;
    using Microsoft.Samples.Tankster.Repositories;
    using Microsoft.WindowsAzure;
    using System.Web;
    using System.Web.Script.Serialization;
    using Microsoft.Samples.Tankster.Entities;
    using System.IO;

    public class ClientController : Controller
    {
        private readonly string storageConnectionStringName;
        private readonly IUserRepository userRepository;
        private readonly IInventoryRepository inventoryRepository;
        private IUserProvider userProvider;

        public ClientController()
            : this("DataConnectionString")
        { 
        }

        public ClientController(string storageConnectionStringName)
            : this(storageConnectionStringName, new HttpContextUserProvider(), new UserRepository(), new InventoryRepository())
        {
        }

        public ClientController(string storageConnectionStringName, IUserProvider userProvider, IUserRepository userRepository, IInventoryRepository inventoryRepository)
        {
            this.storageConnectionStringName = storageConnectionStringName;
            this.userProvider = userProvider;
            this.userRepository = userRepository;
            this.inventoryRepository = inventoryRepository;
        }

        public ActionResult MainScreen()
        {
            this.ConfigureBlobEndpointInViewBag();
            return View();
        }

        [Authorize]
        public ActionResult WarRoom()
        {
            this.ConfigureBlobEndpointInViewBag();
            return View();
        }

        [Authorize]
        public ActionResult ManageInventory()
        {
            this.ConfigureBlobEndpointInViewBag();
            this.ConfigureCurrentUser();
            return View();
        }

        [Authorize]
        public ActionResult Leaderboard()
        {
            this.ConfigureCurrentUser();
            var userProfile = this.userRepository.GetUser(this.ViewBag.CurrentUserId);
            if (userProfile != null && !string.IsNullOrEmpty(userProfile.DisplayName))
            {
                this.ViewBag.CurrentUserName = userProfile.DisplayName;
            }
            else
            {
                this.ViewBag.CurrentUserName = this.ViewBag.CurrentUserId;
            }

            return View();
        }

        [Authorize]
        public ActionResult Skirmish()
        {
            this.ConfigureBlobEndpointInViewBag();
            return View();
        }

        [Authorize]
        public ActionResult Invitation()
        {
            this.ConfigureBlobEndpointInViewBag();
            this.ConfigureCurrentUser();
            return View();
        }

        public ActionResult UpdateInventory()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UpdateInventory(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0)
            {
                this.ModelState.AddModelError("File", "You must select a valid file.");
                return View();
            }

            var inventoryString = new StreamReader(file.InputStream).ReadToEnd();

            InventoryItem[] inventoryItems;
            try
            {
                inventoryItems = new JavaScriptSerializer().Deserialize<InventoryItem[]>(inventoryString);
            }
            catch
            {
                this.ModelState.AddModelError("File", "The file format is invalid.");
                return View();
            }

            this.inventoryRepository.AddOrUpdateInventoryItems(inventoryItems);

            return View();
        }

        private void ConfigureBlobEndpointInViewBag()
        {
            var account = CloudStorageAccount.FromConfigurationSetting(this.storageConnectionStringName);

            this.ViewBag.BlobEndpoint = account.BlobEndpoint.AbsoluteUri.EndsWith("/")
                                       ? account.BlobEndpoint.AbsoluteUri
                                       : account.BlobEndpoint.AbsoluteUri + "/";
        }

        private void ConfigureCurrentUser()
        {
            this.ViewBag.CurrentUserId = this.userProvider.UserId;
        }
    }
}