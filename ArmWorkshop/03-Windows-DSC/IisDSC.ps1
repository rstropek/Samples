configuration IisDSC 
{ 
  Node localhost {
    WindowsFeature Web-WebServer 
    { 
      Ensure = "Present" 
      Name = "Web-WebServer"
    }
    WindowsFeature Web-Static-Content
    { 
      Ensure = "Present" 
      Name = "Web-Static-Content"
    }
    File HelloWorld {
      DestinationPath = "C:\inetpub\wwwroot\hello-devops.html"
      Contents = "<html><body><h1>Hello DevOps!</h1></body></html>"
    }
  }
}
