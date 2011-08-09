

function User(userId, storageEndpoint) 
{
    this.userId = userId;
    this.storageEndpoint = storageEndpoint;
    this.serverInterface = new ServerInterface();
}

User.prototype.getProfile = function(success, error)
{
    var thisUser = this;

    window.usersCallback = success;

    window.getUserProfile = function (userId) {
        thisUser.serverInterface.sendAjaxJsonpGetWithError(thisUser.storageEndpoint + "users/" + userId + "?callback=?",
                "usersCallback", error);
    };

    this.serverInterface.sendAjaxGet("/user/verify",
        function (userId) {
            setTimeout(function () {
                getUserProfile(userId);
            }, 1000)
        },
        error);
};

User.prototype.buyInventory = function (inventoryId, success, error)
{
    this.serverInterface.sendAjaxJsonPost("/user/inventory/buy/" + inventoryId, null, success, error);
};

User.prototype.setProfile = function (profile, success, error)
{
    this.serverInterface.sendAjaxJsonPost("/user/profile", profile, success, error);
};

User.prototype.getNotifications = function (success, error) {
    window.notificationsCallback = success;

    this.serverInterface.sendAjaxJsonpGetWithError(this.storageEndpoint + "notifications/" + this.userId + "?callback=?",
            "notificationsCallback", error);
};

