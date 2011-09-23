
function ConnectFourController(viewModel, gameService, board, game) {
    this.viewModel = viewModel;
    this.gameService = gameService;
    this.board = board;
    this.game = game;
    this.started = false;

    var controller = this;

    this.board.onMove = function (x, y) { controller.onMove(x, y); };
};

ConnectFourController.prototype.start = function () {
    var controller = this;

    if (this.viewModel.gameQueueId() != null) {
        this.viewModel.playerColor(C4Color.Cross);
        this.gameService.joinGameQueue(gameQueueId, function (data) { });
        controller.gameService.process(
                gameQueueId,
                function (queue) { controller.processGameQueue(queue); },
                function (action) { controller.processAction(action); }
                );
    }
    else {
        this.viewModel.playerColor(C4Color.Circle);
        this.gameService.createGameQueue(function (data) {
            controller.viewModel.isOwner(true);
            controller.setGameQueueId(data);
            controller.viewModel.inviteURL(document.location.href + "?id=" + data);
            controller.gameService.process(
                data,
                function (queue) { controller.processGameQueue(queue); },
                function (action) { controller.processAction(action); }
                );
        });
    }
};

ConnectFourController.prototype.processGameQueue = function (gameQueue) {
    this.viewModel.players(gameQueue.Users);
    this.viewModel.noPlayers(gameQueue.Users.length);

    if (gameQueue.GameId != null && gameQueue.GameId != nullGameId) {
        this.viewModel.gameId(gameQueue.GameId);
    }

    if (this.viewModel.gameId() == null && this.viewModel.isOwner() && gameQueue.Users.length == 2 && !this.started) {
        this.gameService.startGame(this.viewModel.gameQueueId());
        this.started = true;
    }
};

ConnectFourController.prototype.processAction = function (gameAction) {
    if (gameAction.Type != 1)
        return;

    var x = parseInt(gameAction.CommandData.x);
    var y = parseInt(gameAction.CommandData.y);
    var color = gameAction.CommandData.color;

    if (!this.game.isValid(x, color))
        return;
    if (!this.game.isEmpty(x, y))
        return;

    y = this.game.move(x, color);
    this.board.drawMove(x, y, color);

    this.updateGameStatus();
};

ConnectFourController.prototype.updateGameStatus = function () {
    if (this.game.isTie()) {
        this.viewModel.isTie(true);
        this.viewModel.currentColor(C4Color.Empty);
    }
    else if (this.game.hasWinner()) {
        this.viewModel.winnerColor(this.game.getWinner());
        this.viewModel.currentColor(C4Color.Empty);
    }
    else
        this.viewModel.currentColor(this.game.nextColor(this.viewModel.currentColor()));
};

ConnectFourController.prototype.onMove = function (x) {
    if (this.viewModel.playerColor() != this.viewModel.currentColor())
        return;

    var color = this.viewModel.playerColor();

    if (!this.game.isValid(x, color))
        return;

    var y = this.game.move(x, color);
    this.board.drawMove(x, y, color);

    this.updateGameStatus();

    var action = { type: 1, commandData: { x: x, y: y, color: color} };
    var gameId = this.viewModel.gameId();

    this.gameService.sendGameAction(gameId, action);
};

ConnectFourController.prototype.setGameQueueId = function (gameQueueId) {
    this.viewModel.gameQueueId(gameQueueId);
};

