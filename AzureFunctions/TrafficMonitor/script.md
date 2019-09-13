# Script

1. Talk about what *serverless* means

1. Create *function app* in portal

```
#r "Newtonsoft.Json"

using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

public static async Task<IActionResult> Run(HttpRequestMessage req, ILogger log) {
    log.LogInformation("Received Tic-Tac-Toe request");
    var board = await req.Content.ReadAsAsync<string[]>();

    if (board.Length != 9) {
        return new BadRequestObjectResult("No valid tic-tac-toe board");
    }

    for(var row = 0; row < 3; row ++) {
        if (!string.IsNullOrWhiteSpace(board[row * 3]) 
            && board[row * 3] == board[row * 3 + 1] && board[row * 3] == board[row * 3 + 2]) {
                return BuildResponse(req, board[row * 3]);
            }
    }

    for(var column = 0; column < 3; column ++) {
        if (!string.IsNullOrWhiteSpace(board[column]) 
            && board[column] == board[3 + column] && board[column] == board[2 * 3 + column]) {
                return BuildResponse(req, board[column]);
            }
    }

    if (!string.IsNullOrWhiteSpace(board[0]) 
        && board[0] == board[3 + 1] && board[0] == board[2 * 3 + 2]) {
            return BuildResponse(req, board[0]);
        }

    if (!string.IsNullOrWhiteSpace(board[2]) 
        && board[2] == board[3 + 1] && board[2] == board[2 * 3]) {
            return BuildResponse(req, board[1]);
        }

    return BuildResponse(req);
}

private static IActionResult BuildResponse(HttpRequestMessage req, string winner = null)
    => new OkObjectResult(new { winner = winner });

```