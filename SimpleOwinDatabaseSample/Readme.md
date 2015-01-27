# Simple OWIN Database Sample

## Introduction

I created this sample for the [OOP 2015](http://www.oop-konferenz.de/oop2015/startseite-englisch/conference.html)
conference in Munich (Germany). My topic is *Application Lifecycle Management in the Cloud*.
As always I try to show mostly practical samples instead of boring slides. 
This sample code should allow participants to reproduce my demos and play with the
results.

## Content of the Sample

I have written a detailed blog article describing the background of the sample.
You can find it [in my blog](http://www.software-architects.com/devblog/2015/01/27/OOP-2015-ALM-in-the-Cloud-with-Visual-Studio-Online-and-Azure).

The code is structured as follows:

* [SimpleOwinDatabaseSample](SimpleOwinDatabaseSample) contains a very simple web
  application (OWIN). For demo purposes it accesses a database (just listing the
  tables inside of the database). Imagine that this sample is a complex LOB web app
  that you want to manage in the cloud.

* [SimpleOwinDatabaseSample.Test](SimpleOwinDatabaseSample.Test) contains 
  unit tests for the sample mentioned above. For demo purposes I also added an
  integration test accessing the database.

* The last sample in [JiraWebhook](JiraWebhook) shows how to implement a 
  [Jira webhook](https://developer.atlassian.com/display/JIRADEV/JIRA+Webhooks+Overview)
  that receives a message whenever a new Jira issue is created. In this
  example I use [*Visual Studio Online*'s REST API](http://www.visualstudio.com/en-us/integrate/reference/reference-vso-overview-vsi) 
  to automatically create the issue as a *Product Backlog Item* in VSO.

## Hands-on Lab

Do you want to play with the sample yourself? Here is what you have to do:

* Read my [blog article](http://www.software-architects.com/devblog/2015/01/27/OOP-2015-ALM-in-the-Cloud-with-Visual-Studio-Online-and-Azure)
  to understand the background of the sample.

* Get an [Azure](http://azure.microsoft.com) and a [Visual Studio Online](http://www.visualstudio.com)
  account. All the components I use in this sample (Azure Websites, Azure SQL DB,
  Visual Studio Online) are free if you accept a certain quota. It should be enough
  for your first experiments.

* Create a new VSO project and explore the project management capabilities.

* Check in the sample code.

* Create a new Azure website and change the connection string settings so that
  it points to a DB in the cloud (maybe you have to create one in Azure).

* Setup continuous deployment using the Azure portal.

* Change the sample code and check in. You should see that a build is started
  automatically. If everything works, you should see the web app in Azure a few
  minutes later.

Do you want to try the Jira Webhook, too?

* Deploy the JiraWebook sample into a new Azure website. Don't forget to set the
  website configuration settings accordingly.

* Configure the webhook in Jira ([details](https://developer.atlassian.com/display/JIRADEV/JIRA+Webhooks+Overview))

* Create an issue in Jira. It should be created in VSO automatically by your webhook.

* Do you want to see what's happening behind the scenes? Try Azure website's 
  [remote debugging feature](http://blogs.msdn.com/b/webdev/archive/2013/11/05/remote-debugging-a-window-azure-web-site-with-visual-studio-2013.aspx)
  to break into your webhook when it is called. Nice, isn't it? 

Have fun!

## Questions

Please post your questions as comments in my 
[blog article](http://www.software-architects.com/devblog/2015/01/27/OOP-2015-ALM-in-the-Cloud-with-Visual-Studio-Online-and-Azure).
