
var GameServiceStatus = { None: 0, Waiting: 1, Playing: 2 };

function GameService(apiURL, blobURL, serverInterface, socket) {
    this.status = GameServiceStatus.None;
    this.serverInterface = serverInterface;
    this.blobURL = blobURL;
    this.apiURL = apiURL;
    this.socket = socket;

    var service = this;

    window.sggamesqueuesCallback = function (gameQueue) { service.processGameQueue(gameQueue); };
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
    this.socket.emit('command', action);
    //this.serverInterface.sendAjaxPost(this.apiURL + "game/command/" + gameId, action, success, error);
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
            this.socket.emit('join', gameQueue.GameId);
            this.socket.on('command', function (action) { gameService.processAction(action); });
        }
    }
    finally {
        this.setTimer();
    }
};

GameService.prototype.processAction = function (gameAction) {
    if (gameAction.type != null && gameAction.Type == null)
        gameAction.Type = gameAction.type;
    if (gameAction.commandData != null && gameAction.CommandData == null)
        gameAction.CommandData = gameAction.commandData;
    if (this.actionCallback != null)
        this.actionCallback(gameAction);
};

GameService.prototype.refresh = function () {
    var service = this;

    if (this.status == GameServiceStatus.Waiting)
        service.getGameQueueStatus(this.gameQueueId, function (req, status, error) { service.setTimer(); });
};

GameService.prototype.setTimer = function () {
    var service = this;

    setTimeout(function () { service.refresh(); }, 300);
};

GameService.prototype.process = function (gameQueueId, queueCallback, actionCallback)
{
    this.gameQueueId = gameQueueId;
    this.queueCallback = queueCallback;
    this.actionCallback = actionCallback;
    this.status = GameServiceStatus.Waiting;
    this.refresh();
};

