# OOP 2017 - Azure Functions Live Demo

## Create C# Function (HTTP Trigger)

* Create C# function called `TicTacToe`:
  ```
  using System.Net;

  public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log) {
      log.Info("Received Tic-Tac-Toe request");
      var board = await req.Content.ReadAsAsync<string[]>();

      if (board.Length != 9) {
          req.CreateResponse(HttpStatusCode.BadRequest, "No valid tic-tac-toe board");
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

  private static HttpResponseMessage BuildResponse(HttpRequestMessage req, string winner = null)
      => req.CreateResponse(HttpStatusCode.OK, new { winner = winner });
  ```

* Try function using demo requests in [test-requests.http](test-requests.http)

* Speak about...
  * HTTP binding
  * Function and admin keys
  * Function app settings

## SCM Integration

* Configure Continuous Integration

* Show Git URL in portal

* `git clone...`

* Demo Azure Functions CLI and speak about working locally (e.g. create, configure, run, debug, etc.)
  ```
  func azure account list
  func azure functionapp list
  ```

## Create Node.js Function (Queue Trigger)

* Create Node.js function called `TicTacToeNode`. Queue name is `tictactoe-request`.
  ```
  module.exports = function (context, req) {
    // Parse request body
    var board = JSON.parse(req.body);

    // Make sure that body is a properly formed array
    if (Array.isArray(board) && board.length == 9) {
        // Body is ok -> send message to trigger analysis
        context.bindings.outputSbMsg = JSON.stringify({ Message: board });

        // Send OK result to caller
        context.res = { status: 200 };
        context.done();
    }
    else {
        // Body is malformed -> send Bad Request to caller
        context.res = { status: 400, body: "No valid tic-tac-toe board" };
        context.done();
    }
  };
  ```

* Try function using demo requests in [test-requests.http](test-requests.http)

* Show message in queue in portal

## Create C# Function (Queue Trigger)

* Create C# function called `TicTacToeQueue`:
  ```
  #r "Newtonsoft.Json"

  using System;
  using System.Threading.Tasks;
  using Newtonsoft.Json;

  public class BoardMessage
  {
      public string[] Message { get; set; }
  }

  public static void Run(string boardMsg, TraceWriter log)
  {
      log.Info("Received Tic-Tac-Toe request");
 
      var boardContent = JsonConvert.DeserializeObject<BoardMessage>(boardMsg);
      log.Info(String.Join("", boardContent.Message).Replace(" ", "."));
  }
  ```

* Try function using demo requests in [test-requests.http](test-requests.http)

## Create Logic App

* Build workflow
  * DropBox: When file is created
  * Service Bus
  * DropBox: Delete file
  * Post to Slack (`#random` channel)

* Create file in DropBox:
  ```
  { "Message": [ 
      " ", "X", " ",
      " ", "X", " ",
      " ", "X", " " ] 
  }
  ```




