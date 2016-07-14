using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using System.Linq;

namespace SignalrOwinHelloWorld
{
    public class GameHub : Hub
    {
        // Internal list of games
        private static List<Game> games = new List<Game>();

        /// <summary>
        /// Method called by a client if a user wants to start a game
        /// </summary>
        /// <param name="gameId">ID of the new game</param>
        public void StartGame(int gameId)
        {
            // Store the game's data in memory
            GameHub.games.Add(new Game
            {
                GameId = gameId,
                Connection1 = this.Context.ConnectionId
            });
        }

        /// <summary>
        /// Method called by a client if a user wants to join a game
        /// </summary>
        /// <param name="gameId">ID of the game to join</param>
        /// <returns>
        /// SignalR connection ID of the partner or null if <paramref name="gameID"/> is unknown.
        /// </returns>
        public string JoinGame(int gameId)
        {
            // Find game with specified game ID
            var game = GameHub.games.SingleOrDefault(g => g.GameId == gameId);
            if (game == null)
            {
                // No such game found
                return null;
            }

            // Make sure that game does not already have two players
            if (game.Connection2 == null)
            {
                // Add second player to game
                game.Connection2 = this.Context.ConnectionId;

                // Inform first player that second player has arrived
                this.Clients.Client(game.Connection1).PlayerArrived(game.Connection2);

                return game.Connection1;
            }
            else
            {
                // Game already has to players
                return null;
            }
        }

        /// <summary>
        /// Method called by a client when player fired
        /// </summary>
        public void Fire()
        {
            // Find game associated with the current connection ID
            var game = GameHub.games.SingleOrDefault(g => g.Connection1 == this.Context.ConnectionId || g.Connection2 == this.Context.ConnectionId);
            if (game != null)
            {
                // Inform other player that she got shot
                this.Clients.Client(game.Connection1 == this.Context.ConnectionId ? game.Connection2 : game.Connection1).GotShot();
            }
        }
    }
}
