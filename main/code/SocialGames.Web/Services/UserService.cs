namespace Microsoft.Samples.SocialGames.GamePlay.Services
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Json;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Runtime.Serialization.Json;
    using System.Web;
    using Microsoft.ApplicationServer.Http;
    using Microsoft.Samples.SocialGames.Entities;
    using Microsoft.Samples.SocialGames.GamePlay.Extensions;
    using Microsoft.Samples.SocialGames.Repositories;

    public class UserService : ServiceBase, IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly IInventoryRepository inventoryRepository;
        private readonly IStatisticsRepository statsRepository;

        public UserService()
            : this(new UserRepository(), new InventoryRepository(), new StatisticsRepository("StatisticsConnectionString"), new HttpContextUserProvider())
        {            
        }

        public UserService(IUserRepository userRepository, IInventoryRepository inventoryRepository, IStatisticsRepository statsRepository, IUserProvider userProvider)
            : base(userProvider)
        {
            this.userRepository = userRepository;
            this.inventoryRepository = inventoryRepository;
            this.statsRepository = statsRepository;
        }

        public HttpResponseMessage Verify(HttpRequestMessage request)
        {
            if (string.IsNullOrWhiteSpace(this.CurrentUserId))
            {
                return BadRequest("The user is not authenticated");
            }

            return HttpResponse<string>(this.CurrentUserId);
        }

        public HttpResponseMessage UpdateProfile(HttpRequestMessage request)
        {
            var formContent = GetFormContent(request);
            var displayName = (string)(formContent.displayName ?? string.Empty);
            var customizationsIds = 
                formContent.customizationsIds != null ? 
                ((JsonArray)formContent.customizationsIds).ToObjectArray().Select(o => new Guid(o.ToString())) : 
                null;

            var userProfile = this.userRepository.GetUser(CurrentUserId);

            if (userProfile == null)
            {
                return BadRequest("User does not exist");
            }

            if (!string.IsNullOrWhiteSpace(displayName))
            {
                userProfile.DisplayName = displayName;
            }

            if (customizationsIds != null)
            {
                foreach (var customizationId in customizationsIds)
                {
                    var item = userProfile.Inventory.First(i => i.Id == customizationId);
                    if (item != null)
                    {
                        if (userProfile.Customizations.ContainsKey(item.Type))
                        {
                            userProfile.Customizations[item.Type] = item;
                        }
                        else
                        {
                            userProfile.Customizations.Add(item.Type, item);
                        }
                    }
                }
            }

            this.userRepository.AddOrUpdateUser(userProfile);

            return SuccessResponse;
        }

        public HttpResponseMessage BuyInventory(Guid inventoryId, HttpRequestMessage request)
        {
            if (inventoryId == Guid.Empty)
            {
                return BadRequest("InventoryId cannot be empty");
            }
            
            var userProfile = this.userRepository.GetUser(CurrentUserId);
            if (userProfile == null)
            {
                return BadRequest("User does not exist");
            }

            var inventoryItem = this.inventoryRepository.GetInventoryItem(inventoryId);
            if (inventoryItem == null)
            {
                return BadRequest(string.Format(CultureInfo.InvariantCulture, "Inventory with id '{0}' does not exist", inventoryId));
            }

            if (inventoryItem.Price > userProfile.Credits)
            {
                return BadRequest("Not enough credits");
            }

            if (userProfile.Inventory.Any(c => c.Id == inventoryId))
            {
                return BadRequest("Item already in inventory");
            }

            userProfile.Inventory.Add(inventoryItem);
            userProfile.Credits -= inventoryItem.Price;

            this.userRepository.AddOrUpdateUser(userProfile);
            
            return SuccessResponse;
        }

        public HttpResponseMessage Leaderboard(int count)
        {
            try
            {
                var boards = this.statsRepository.GenerateLeaderboard(count).ToArray();
                this.UpdateUserName(ref boards);

                var response = HttpResponse<Board[]>(boards, contentType: "application/json");

                response.Headers.CacheControl = new CacheControlHeaderValue();
                response.Headers.CacheControl.Public = false;
                response.Headers.CacheControl.NoStore = true;
                response.Headers.CacheControl.NoCache = true;

                return response;
            }
            catch (Exception ex)
            {
                return BadRequest("Could not retrieve statistics from the database. " + ex.Message);
            }
        }

        public HttpResponseMessage LeaderboardWithFocus(string focusUserId, int focusCount)
        {
            try
            {
                var boards = this.statsRepository.GenerateLeaderboard(focusUserId, focusCount).ToArray();
                this.UpdateUserName(ref boards);

                return HttpResponse<Board[]>(boards, contentType: "application/json");
            }
            catch (Exception ex)
            {
                return BadRequest("Could not retrieve statistics from the database. " + ex.Message);
            }
        }

        private void UpdateUserName(ref Board[] boards)
        {
            foreach (var board in boards)
            {
                foreach (var score in board.Scores)
                {
                    var profile = this.userRepository.GetUser(score.UserId);

                    if (profile != null && !string.IsNullOrEmpty(profile.DisplayName))
                    {
                        score.UserName = profile.DisplayName;
                    }
                }
            }
        }
    }
}