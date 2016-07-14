$(function startup() {
    // Set SignalR base URL
    $.connection.hub.url = "/signalr";

    // Get SignalR hub for current game
    var hub = $.connection.gameHub;

    // ID of the current game
    var gameId;

    // Helper methods
    function switchToGameMode(gameId) {
        $("#menu").addClass("hidden");
        $("#game").removeClass("hidden");
        $("#runningGameId").text(gameId.toString());
    }

    function setPartnerId(partnerId) {
        $("#partnerId").text(partnerId.toString());
        $("#partnerMsg").removeClass("hidden");
        $("#fire").removeClass("hidden");
    }

    // Add client-side methods to SignalR. The server can call these methods.
    hub.client.playerArrived = function (partnerId) { setPartnerId(partnerId); };
    hub.client.gotShot = function () { $("#log").append("<li>I got shot!</li>"); };

    // Start SignalR hub
    $.connection.hub.start();

    // Button client handlers

    $("#startGame").click(function () {
        // Generate random game ID
        gameId = Math.floor((Math.random() * 100000) + 1);

        // Tell server to start a new game
        hub.server.startGame(gameId);

        // Switch UI to game mode
        switchToGameMode(gameId);
    });

    $("#fire").click(function () {
        // Send FIRE to server
        hub.server.fire();
    });

    $("#joinGame").click(function () {
        if ($("#gameId").val()) {
            // Get ID of game from input control
            gameId = parseInt($("#gameId").val());

            // Join the game
            hub.server.joinGame(gameId).done(function (partnerId) {
                if (partnerId) {
                    // Switch UI to game mode
                    switchToGameMode(gameId);

                    // Display partner ID
                    setPartnerId(partnerId);
                } else {
                    // There was an error joining the game
                    alert("Could not join this game. Already full or unknown game ID!");
                }
            });
        } else {
            alert("Please enter the ID of the game you want to join!");
        }
    });
});