# APICon - Azure Functions Live Demo

## Introduction

* Discuss what *serverless* means?
* Discuss Azure Functions' [consumption pricing plan](https://azure.microsoft.com/en-us/pricing/details/functions/)

## Create C# Function (HTTP Trigger) in Portal

* Create C# function called `TicTacToe`:

  ```
  using System.Net;

  public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log) {
      log.Info("Received Tic-Tac-Toe request");
      var board = await req.Content.ReadAsAsync<string[]>();

      if (board.Length != 9) {
          return req.CreateResponse(HttpStatusCode.BadRequest, "No valid tic-tac-toe board");
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


## Create C# Function (HTTP Trigger) in Visual Studio

* Create *Functions* project in Visual Studio 2017

* Add function *TicTacToe* to project

* Add C# code from above and show local debugging using demo requests in [test-requests.http](test-requests.http)

* Publish function from Visual Studio

* Trigger function using demo requests in [test-requests.http](test-requests.http)


## Create Node.js Function (HTTP Trigger) in CLI

* Create Function with [Functions CLI](https://github.com/Azure/azure-functions-cli):

  ```
  func azure login
  func azure functionapp list
  func init
  func new --language JavaScript --template HttpTrigger --name TicTacToeCli
  func settings add ApiConFunctionsBus Endpoint=sb://apicon-functions-bus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=tiptDMyaMLGfNvGq8OFUMo+4N+QHdbfNJDqNCmYl1bU=
  ```

* Show local settings

* Add [output binding](https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-service-bus#service-bus-output-binding) to function:

  ```
  {
    "name" : "outputSbMsg",
    "queueName" : "tictactoequeue",
    "connection" : "ApiConFunctionsBus",
    "accessRights" : "manage",
    "type" : "serviceBus",
    "direction" : "out"
  }
  ```

* Change *authLevel* to *anonymous*

* Add Node.js implementation:

  ```
  module.exports = function(context, req) {
    // Parse request body
    var board = JSON.parse(req.body);

    // Make sure that body is a properly formed array
    if (Array.isArray(board) && board.length == 9) {
      // Body is ok -> send message to trigger analysis
      context.bindings.outputSbMsg = JSON.stringify({Message: board});

      // Send ACCEPTED result to caller
      context.res = {status: 202};
      context.done();
    } else {
      // Body is malformed -> send Bad Request to caller
      context.res = {status: 400, body: 'No valid tic-tac-toe board'};
      context.done();
    }
  };
  ```


* Run function locally

  ```
  cd TicTacToeCli
  func run .
  ```

* Trigger function using demo requests in [test-requests.http](test-requests.http) and show how message is added to queue.


## Add C# Function using Visual Studio

```
[FunctionName("TicTacToeQueue")]                    
public static void Run(
    [ServiceBusTrigger("tictactoequeue", AccessRights.Manage, Connection = "ApiConFunctionsBus")]string boardMsg,
    TraceWriter log)
{
    log.Info("Received Tic-Tac-Toe request");

    var boardContent = JsonConvert.DeserializeObject<BoardMessage>(boardMsg);
    log.Info(String.Join("", boardContent.Message).Replace(" ", "."));
}
```


## SCM Integration

* Configure Continuous Integration

* Show Git URL in portal

* `git remote add azure https://rainerdeploy@apicon-functions-git.scm.azurewebsites.net:443/apicon-functions-git.git`

* Publish code using Git.

* Copy setting from local settings into Azure portal.

* Trigger function using demo requests in [test-requests.http](test-requests.http) and show how message is added to queue.

