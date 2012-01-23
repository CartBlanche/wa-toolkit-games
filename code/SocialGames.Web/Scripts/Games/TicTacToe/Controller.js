
function TicTacToeController(viewModel, gameService, board, game) {
    this.viewModel = viewModel;
    this.gameService = gameService;
    this.board = board;
    this.game = game;
    this.started = false;

    var controller = this;

    this.board.onMove = function (x, y) { controller.onMove(x, y); };
};

TicTacToeController.prototype.start = function () {
    var controller = this;

    if (this.viewModel.gameQueueId() != null) {
        this.viewModel.playerColor(TTTColor.Cross);
        this.gameService.joinGameQueue(gameQueueId, function (data) { });
        controller.gameService.process(
                gameQueueId,
                function (queue) { controller.processGameQueue(queue); },
                function (action) { controller.processAction(action); }
                );
    }
    else {
        this.viewModel.playerColor(TTTColor.Circle);
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

TicTacToeController.prototype.processGameQueue = function (gameQueue) {
    for (var n in gameQueue.Users) {
        var user = gameQueue.Users[n];
        if (user.UserName == null || user.UserName == "")
            user.UserName = user.UserId;
    }

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

TicTacToeController.prototype.processAction = function (gameAction) {
    if (gameAction.Type != 1)
        return;

    var x = parseInt(gameAction.CommandData.x);
    var y = parseInt(gameAction.CommandData.y);
    var color = gameAction.CommandData.color;

    if (!this.game.isValid(x, y, color))
        return;

    this.game.move(x, y, color);
    this.board.drawMove(x, y, color);

    this.updateGameStatus();
};

TicTacToeController.prototype.updateGameStatus = function () {
    if (this.game.isTie()) {
        var action = { type: 1, commandData: { Victories: 0, Defeats: 0, GameCount: 1} };
        this.gameService.postStatisticsEvent(action);

        this.viewModel.isTie(true);
        this.viewModel.currentColor(TTTColor.Empty);
    }
    else if (this.game.hasWinner()) {
        var winner = this.game.getWinner();

        var victories = 0;
        var defeats = 0;
        if (this.viewModel.playerColor() == winner) {
            victories = 1;
        }
        else {
            defeats = 1;
        }

        var action = { type: 1, commandData: { Victories: victories, Defeats: defeats, GameCount: 1} };
        this.gameService.postStatisticsEvent(action);

        this.viewModel.winnerColor(winner);
        this.viewModel.currentColor(TTTColor.Empty);
    }
    else
        this.viewModel.currentColor(this.game.nextColor(this.viewModel.currentColor()));
};

TicTacToeController.prototype.onMove = function (x, y) {
    if (this.viewModel.playerColor() != this.viewModel.currentColor())
        return;

    var color = this.viewModel.playerColor();

    if (!this.game.isValid(x, y, color))
        return;

    this.game.move(x, y, color);
    this.board.drawMove(x, y, color);

    this.updateGameStatus();

    var action = { type: 1, commandData: { x: x, y: y, color: color} };
    var gameId = this.viewModel.gameId();

    this.gameService.sendGameAction(gameId, action);
};

TicTacToeController.prototype.setGameQueueId = function (gameQueueId) {
    this.viewModel.gameQueueId(gameQueueId);
};

