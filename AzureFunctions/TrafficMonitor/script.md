1. Create function app

1. Speak about options

1. Create function in portal

```CSharp
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

1. Try requests (see *requests.http*)

1. Create local function

```bash
func init --worker-runtime node --docker --language javascript
func new --language javascript --name SayHello
```

* Change *authLevel* to *anonymous*

```bash
func start
```

1. Run in Docker

```bash
docker build -t funcsayhello .
docker run -t --name sayhello -p 7071:80 funcsayhello
docker rm -f sayhello
```

1. Show more complex solution in VS

1. Discuss C# programming model

1. Publish to folder and discuss files

1. Speak about deployment options

1. Demo license plate recognition
