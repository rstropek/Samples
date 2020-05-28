# Durable Functions Introduction

## Slides

https://slides.com/rainerstropek/azure-durable-functions/fullscreen

## Demo

### Basics

* Create empty *Function App* called `DurableFunctionsIntro`.

* Add file *DataTransferObjects.cs*, use snippet *010*

* Replace generated function with snippet *020*

* Add function simulating storage for speed violations, snippet *030*

* Replace `log.LogError($$"{nameof(SpeedViolationRecognition)} not yet implemented");` with snippet 040

### Durable Orchestration Function

* Add file *ManuallyApproveRecognition.cs* and add orchestration function with snippet 050
  * Demo function without durable part
  * Show state handling in *Azure Storage Explorer*

* Replace `log.LogError($$"Manual recognition not yet implemented"); return new HttpResponseMessage(HttpStatusCode.InternalServerError);` with snippet 060
  * Demo function, wait for timeout

* Add function simulating sending request to slack with snippet 070

* Replace `// Send request to Slack log.LogError("Communication with Slack not yet implemented");` with snippet 080

* Turn `StoreSpeedViolation` into an activity with snippet 090

* Add Slack response handling with three steps:
  * Add receive approval event name with snippet 100
  * Replace `// Wait for the event that will be raised once we have received the response from Slack. log.LogError("Waiting for Slack not yet implemented");` with snippet 110
  * Add HTTP triggered webhook for Slack response with snippet 120
  * Replace `return false;` statement with `return winner == approvalResponse && approvalResponse.Result;`
  * Demo whole workflow

### Durable Entities

* Create new file *TrafficSpeedViolation.cs* and add snippet 130

* Add lawsuite access functions with snippet 140

* Create lawsuite instance in orchestration function with snippet 150
  * Demo whole process with sample requests.
