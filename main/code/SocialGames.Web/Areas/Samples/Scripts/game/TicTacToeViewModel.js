
function TicTacToeViewModel() {
    this.playerName = ko.observable("user");
    this.isOwner = ko.observable(false);
    this.players = ko.observableArray();

    this.gameQueueId = ko.observable(null);
    this.gameId = ko.observable(null);

    this.noPlayers = ko.observable(0);
    this.inviteURL = ko.observable(null);

    this.playerColor = ko.observable(TTTColor.Empty);
    this.winnerColor = ko.observable(TTTColor.Empty);
    this.currentColor = ko.observable(TTTColor.Cross);
    this.isTie = ko.observable(false);
}



