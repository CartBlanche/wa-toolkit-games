
function UserService(apiURL, blobURL, serverInterface) {
    this.serverInterface = serverInterface;
    this.blobURL = blobURL;
    this.apiURL = apiURL;
}

UserService.prototype.verify = function (success, error) {
    this.serverInterface.sendAjaxGet(this.apiURL + "user/verify", success, error);
};

UserService.prototype.getUser = function (userId, error) {
    this.serverInterface.sendAjaxJsonpGet(this.blobURL + "sgusers/" + userId + "?callback=?", "sgusersCallback", error);
};

UserService.prototype.getNotifications = function (userId, error) {
    this.serverInterface.sendAjaxJsonpGet(this.blobURL + "sgnotifications/" + userId + "?callback=?",
            "sgnotificationsCallback", error);
};

UserService.prototype.getFriends = function (success, error) {
    this.serverInterface.sendAjaxJsonPost("/user/friends", null, success, error);
}

UserService.prototype.updateProfile = function (profile, success, error) {
    this.serverInterface.sendAjaxJsonPost("/user/profile", profile, success, error);
}

UserService.prototype.getFriendsInfo = function (success, error) {
    this.serverInterface.sendAjaxJsonPost("/user/friendsinfo", null, success, error);
}

UserService.prototype.addFriend = function (friendId, success, error) {
    this.serverInterface.sendAjaxJsonPost("/user/friend/add/" + friendId, null, success, error);
}

UserService.prototype.getLeaderboard = function (count, success, error) {
    this.serverInterface.sendAjaxGet("/user/leaderboard/" + count, success, error);
}

UserService.prototype.getLeaderboardWithFocus = function (focusCount, focusUserId, success, error) {
    this.serverInterface.sendAjaxGet("/user/leaderboard/" + focusCount + "?user=" + focusUserId, success, error);
}

