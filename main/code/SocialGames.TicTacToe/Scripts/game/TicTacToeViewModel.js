
TicTacToeStage = { Create: 0, Join: 1, Wait: 2, Play: 3, Leave: 4 };

function TicTacToeViewModel() {
    this.stage = ko.observable(TicTacToeStage.Create);
    this.playerName = ko.observable("user");
    this.isOwner = ko.observable(false);
    this.players = ko.observableArray();
    this.gameQueueId = ko.observable(null);
    this.gameId = ko.observable(null);
    this.noPlayers = ko.observable(0);
    this.inviteURL = ko.observable(null);
}

