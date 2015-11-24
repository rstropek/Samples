configuration IisDSC 
{ 
  Node localhost {
    WindowsFeature Web-WebServer 
    { 
      Ensure = "Present" 
      Name = "Web-WebServer"
    }
    WindowsFeature Web-Default-Doc
    { 
      Ensure = "Present" 
      Name = "Web-Default-Doc"
    }
    WindowsFeature Web-Http-Errors
    { 
      Ensure = "Present" 
      Name = "Web-Http-Errors"
    }
    WindowsFeature Web-Static-Content
    { 
      Ensure = "Present" 
      Name = "Web-Static-Content"
    }
    WindowsFeature Web-Http-Logging
    { 
      Ensure = "Present" 
      Name = "Web-Http-Logging"
    }
    WindowsFeature Web-Stat-Compression
    { 
      Ensure = "Present" 
      Name = "Web-Stat-Compression"
    }
    WindowsFeature Web-Mgmt-Console
    { 
      Ensure = "Present" 
      Name = "Web-Mgmt-Console"
    }
    WindowsFeature Web-Mgmt-Service
    { 
      Ensure = "Present" 
      Name = "Web-Mgmt-Service"
    }
    Package PlatformHandlerPackage
    {
        Ensure = "Present"
        Path  = "http://download.microsoft.com/download/8/1/3/813AC4E6-9203-4F7A-8DD5-F3D54D10C5CD/httpPlatformHandler_amd64.msi"
        Name = "Microsoft HTTP Platform Handler 1.2"
        Arguments = "ADDLOCAL=all"
        ProductId = "49FE726A-F8A3-426F-9448-337D47E355FA"
    } 

    Package WebDeployPackage
    {
        Ensure = "Present"
        Path  = "http://download.microsoft.com/download/D/4/4/D446D154-2232-49A1-9D64-F5A9429913A4/WebDeploy_amd64_en-US.msi"
        Name = "Microsoft Web Deploy 3.5"
        Arguments = "ADDLOCAL=all"
        ProductId = "1A81DA24-AF0B-4406-970E-54400D6EC118"
    } 

    Script SetupDefaultWebsiteWebDeploy
    {
        SetScript = { 
            # Enable Default Web Site for WebDeploy (non-admin user)
            $wdscripts = "$Env:PROGRAMFILES\IIS\Microsoft Web Deploy V3\Scripts";
            cd $wdscripts
            .\SetupSiteForPublish.ps1 -siteName "Default Web Site" -deploymentUserName deploy -deploymentUserPassword P@zzw0rd! -publishSettingSavePath C:\temp -publishSettingFileName rainer.PublishSettings -sitePort 80
            New-Item -Path C:\ -Name "WebDeployEnabled.txt" -ItemType File
        }
        TestScript = { Test-Path "C:\WebDeployEnabled.txt" }
        GetScript = { <# This must return a hash table #> }          
        DependsOn = "[Package]WebDeployPackage"
    }
  }
}
