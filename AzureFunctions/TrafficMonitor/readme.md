# TrafficMonitor Sample

## Preparation Checklist

1. Close all social media tools (e.g. Skype, Slack, etc.)
1. Open Azure Portal
1. Open Visual Studio with the sample solution [*TrafficMonitor.sln*](TrafficMonitor.sln) open
1. Open *Insomnia* with a workspace including the request for the demo
1. Open a console
   * Navigate to an empty folder for CLI demo
1. Open *Azure Document Explorer* connected to the correct Blob container
1. Open *Azure Service Bus Explorer* connected to the correct SB namespace
1. Open *Windows Explorer* with source code folder open
1. Purge all service bus subscriptions
1. Open [*OpenALPR*](https://cloud.openalpr.com/cloudapi/)
1. Start *zoomit*

## Demo Checklist

1. Create function in portal
   * Show file structure
1. Create function locally
   * `func init --no-source-control --docker --sample`
   * `func new`
   * Change auth to `anonymous`
1. Function in Docker
   * `docker build --platform linux -t docker-func .`
   * `docker run --platform linux -p 8083:80 docker-func`
1. Code walkthrough
1. Execute locally
   * Good read
   * Poor read
1. Deploy using VS
1. Showcase Logic App
