namespace Microsoft.Samples.SocialGames.Web.Services
{
    using System;
    using System.Json;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Web.Mvc;
    using Microsoft.Samples.SocialGames.Entities;
    using Microsoft.Samples.SocialGames.Repositories;

    public class UserService : ServiceBase, IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly IStatisticsRepository statsRepository;

        public UserService(IUserRepository userRepository, IStatisticsRepository statsRepository, IUserProvider userProvider)
            : base(userProvider)
        {
            this.userRepository = userRepository;
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

            dynamic formContent = request.Content.ReadAsAsync<JsonValue>().Result;
            var displayName = string.Empty;
            if (formContent != null)
            {
                displayName = (string)(formContent.displayName ?? string.Empty);
            }

            var userProfile = this.userRepository.GetUser(CurrentUserId);

            if (userProfile == null)
            {
                return BadRequest("User does not exist");
            }

            if (!string.IsNullOrWhiteSpace(displayName))
            {
                userProfile.DisplayName = displayName;
            }

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

        [Authorize]
        public HttpResponseMessage GetFriends()
        {
            var friends = this.userRepository.GetFriends(this.CurrentUserId).ToArray();

            return HttpResponse<string[]>(friends, contentType: "application/json");
        }

        [Authorize]
        public HttpResponseMessage GetFriendsInfo()
        {
            var friends = this.userRepository.GetFriendsInfo(this.CurrentUserId).ToArray();

            return HttpResponse<UserInfo[]>(friends, contentType: "application/json");
        }

        [Authorize]
        public HttpResponseMessage AddFriend(string friendId)
        {
            this.userRepository.AddFriend(this.CurrentUserId, friendId);

            return SuccessResponse;
        }

        private void UpdateUserName(ref Board[] boards)
        {
            foreach (var board in boards)
            {
                foreach (var score in board.Scores)
                {
                    if (string.IsNullOrEmpty(score.UserId))
                    {
                        continue;
                    }

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
