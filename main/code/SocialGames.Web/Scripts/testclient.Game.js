

function Game(gameId, storageEndpoint) 
{
    this.gameId = gameId;
    this.storageEndpoint = storageEndpoint;
    this.serverInterface = new ServerInterface();
}

Game.prototype.leaveGame = function(success, error)
{
    this.serverInterface.sendAjaxJsonPost(
        "/Game/Leave/" + this.gameId, // url
        { reason: "Unknown" }, // data
        success,    // success callback
        error);     // error callback
};

Game.prototype.setWeapons = function (weaponIds, success, error) 
{
    this.serverInterface.sendAjaxJsonPost('/game/weapons/' + this.gameId, { weaponIds: weaponIds }, success, error);
};

Game.prototype.sendCommand = function (command, success, error) 
{
    this.serverInterface.sendAjaxJsonPost('/game/command/' + this.gameId, command, success, error);
};

Game.prototype.postStatisticsEvent = function (command, success, error) {
    this.serverInterface.sendAjaxJsonPost('/event/post/statistics', command, success, error);
};

Game.prototype.postNotificationsEvent = function (command, success, error) {
    this.serverInterface.sendAjaxJsonPost('/event/post/notifications', command, success, error);
};

Game.prototype.sendTestCommand = function (command, success, error) {
    this.serverInterface.sendAjaxJsonPost('/test/command/' + this.gameId, command, success, error);
};

Game.prototype.queue = function (gameType, success, error)
{
    this.serverInterface.sendAjaxJsonPost('/game/queue', { gameType: gameType }, success, error);
};

Game.prototype.getStatus = function (success, error) {
    window.gamesCallback = success;

    this.serverInterface.sendAjaxJsonpGetWithError(this.storageEndpoint + "games/" + this.gameId + "?callback=?",
            "gamesCallback", error);
};
