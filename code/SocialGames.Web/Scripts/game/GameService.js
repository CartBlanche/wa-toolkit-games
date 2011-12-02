
var GameServiceStatus = { None: 0, Waiting: 1, Playing: 2 };

function GameService(apiURL, blobURL, serverInterface) {
    this.status = GameServiceStatus.None;
    this.serverInterface = serverInterface;
    this.blobURL = blobURL;
    this.apiURL = apiURL;

    var service = this;

    window.sggamesqueuesCallback = function (gameQueue) { service.processGameQueue(gameQueue); };
    window.sggamesCallback = function (game) { service.processGame(game); };
}


GameService.prototype.createGameQueue = function (success, error) {
    this.serverInterface.sendAjaxPost(this.apiURL + "game/create", { gameType: "invitation" }, success, error);
};

GameService.prototype.joinGameQueue = function (queueId, success, error) {
    this.serverInterface.sendAjaxPost(this.apiURL + "game/join/" + queueId, { gameType: "invitation" }, success, error);
};

GameService.prototype.startGame = function (queueId, success, error) {
    this.serverInterface.sendAjaxPost(this.apiURL + "game/start/" + queueId, { gameType: "invitation" }, success, error);
};

GameService.prototype.sendGameAction = function (gameId, action, success, error) {
    this.serverInterface.sendAjaxPost(this.apiURL + "game/command/" + gameId, action, success, error);
};

GameService.prototype.postStatisticsEvent = function (action, success, error) {
    this.serverInterface.sendAjaxPost(this.apiURL + "event/post/statistics", action, success, error);
};

GameService.prototype.getGameQueueStatus = function (queueId, error) {
    this.serverInterface.sendAjaxJsonpGet(this.blobURL + "sggamesqueues/" + queueId + "?callback=?", "sggamesqueuesCallback", error);
};

GameService.prototype.getGameStatus = function (gameId, error) {
    this.serverInterface.sendAjaxJsonpGet(this.blobURL + "sggames/" + gameId + "?callback=?", "sggamesCallback", error);
};

GameService.prototype.processGameQueue = function (gameQueue) {
    var gameService = this;
    try {
        if (this.queueCallback != null)
            this.queueCallback(gameQueue);
        if (this.status != GameServiceStatus.Playing && gameQueue.GameId != null && gameQueue.GameId != nullGameId) {
            this.gameId = gameQueue.GameId;
            this.status = GameServiceStatus.Playing;
        }
    }
    finally {
        this.setTimer();
    }
};

GameService.prototype.processGame = function (game) {
    try {
        if (this.actionCallback == null)
            return;

        for (var n in game.GameActions) {
            var gameAction = game.GameActions[n];
            this.actionCallback(gameAction);
        }
    }
    finally {
        this.setTimer();
    }
};

GameService.prototype.refresh = function () {
    var service = this;

    if (this.status == GameServiceStatus.Waiting)
        service.getGameQueueStatus(this.gameQueueId, function (req, status, error) { service.setTimer(); });
    if (this.status == GameServiceStatus.Playing)
        service.getGameStatus(this.gameId, function (req, status, error) { service.setTimer(); });
};

GameService.prototype.setTimer = function () {
    var service = this;

    setTimeout(function () { service.refresh(); }, 300);
};

GameService.prototype.inviteUser = function (gameQueueId, user, message, url, success, error) {
    this.serverInterface.sendAjaxJsonPost('/game/invite/' + gameQueueId, { users: [user], message: message, url: url }, success, error);
};

GameService.prototype.process = function (gameQueueId, queueCallback, actionCallback)
{
    this.gameQueueId = gameQueueId;
    this.queueCallback = queueCallback;
    this.actionCallback = actionCallback;
    this.status = GameServiceStatus.Waiting;
    this.refresh();
};

