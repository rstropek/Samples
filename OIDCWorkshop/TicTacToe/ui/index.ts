/// <reference path="./node_modules/oidc-client/oidc-client.d.ts" />

$(() => {
  function buildBoard(board: JQuery<HTMLElement>, boardData: string[]) {
    for (let row = 0; row < 3; row++) {
      const tr = $('<tr/>');
      for (let col = 0; col < 3; col++) {
        const td = $(
            '<td/>', {'data-row': row.toString(), 'data-col': col.toString()});
        td.appendTo(tr);
        boardData.push(' ');
      }

      tr.appendTo(board);
    }
  }

  function getWinner(boardData: string[]): (string|null) {
    for (var row = 0; row < 3; row++) {
      if (boardData[row * 3] !== ' ' &&
          boardData[row * 3] == boardData[row * 3 + 1] &&
          boardData[row * 3] == boardData[row * 3 + 2]) {
        return boardData[row * 3];
      }
    }

    for (var col = 0; col < 3; col++) {
      if (boardData[col] !== ' ' && boardData[col] == boardData[3 + col] &&
          boardData[col] == boardData[2 * 3 + col]) {
        return boardData[col];
      }
    }

    if (boardData[0] !== ' ' && boardData[0] == boardData[3 + 1] &&
        boardData[0] == boardData[2 * 3 + 2]) {
      return boardData[0];
    }

    if (boardData[2] !== ' ' && boardData[2] == boardData[3 + 1] &&
        boardData[2] == boardData[2 * 3]) {
      return boardData[2];
    }

    return null;
  }

  function refreshMessage(msg: JQuery<HTMLElement>, player: string): void {
    if (player !== 'W') {
      msg.text(`Player ${player}, it is your turn.`);
    }
  }

  async function buildUrlForSaving(winner: string): Promise<string> {
    const client = new Oidc.OidcClient({
      authority: 'http://localhost:5000/',
      client_id: 'spa',
      redirect_uri: 'http://localhost:5002/callback.html',
      response_type: 'id_token token',
      scope: 'openid profile api1'
    });
    const req = await client.createSigninRequest(
        {state: JSON.stringify({winner: winner})});
    return req.url;
  }

  const boardData: string[] = [];
  let player = 'X';
  const msg = $('#msg');
  const ai: Microsoft.ApplicationInsights.IAppInsights =
      (<any>window).appInsights;

  buildBoard($('#board'), boardData);
  refreshMessage(msg, player);
  $('#board td').click(async elem => {
    // User clicked on board cell

    // Return if game has already been ended
    if (player === 'E') return;

    // Get coordinates of clicked cell
    const row = parseInt(<string>elem.target.getAttribute('data-row'));
    const col = parseInt(<string>elem.target.getAttribute('data-col'));
    const ix = row * 3 + col;

    // Check if selected cell is still empty
    if (boardData[ix] === ' ') {
      // Set cell to current player
      boardData[ix] = player;
      elem.target.setAttribute('class', player);

      // Check if there is a winner
      const winner = getWinner(boardData);
      if (winner) {
        // Mark game as ended and display a message
        player = 'E';
        msg.text(`Player ${winner}, you won!`);

        // Track end of game in AI
        ai.trackEvent('game-end', {winner: winner});

        // Display link for saving victory
        $('#save-win').removeAttr('hidden');
        $('#save-link').attr('href', await buildUrlForSaving(winner));
      } else {
        // Check if there are no more free cells
        if (boardData.indexOf(' ') === -1) {
          // Mark game as ended and display a message
          player = 'E';
          msg.text('Game over, no winner.');

          // Track end of game in AI
          ai.trackEvent('game-end', {winner: ' '});
        } else {
          // Set next active player and display message
          player = player === 'X' ? 'O' : 'X';
          refreshMessage(msg, player);
        }
      }
    }
  });
});
