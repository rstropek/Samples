# Material for gRPC

## .NET Core

### Getting Started

* Project template specific for *gRPC*

### Protobuf Compiler for CSharp

* See also [protobuf.md](protobuf.md)
* Protobuf Compiler [on NuGet](https://www.nuget.org/packages/Grpc.Tools/)
* Protobuf Compiler [on GitHub](https://github.com/grpc/grpc)
* Protobuf Runtime [on NuGet](https://www.nuget.org/packages/Google.Protobuf/)

### gRPC for CSharp

* What you need for the server:
  * gRPC metapackage for gRPC in ASP.NET Core [on NuGet](https://www.nuget.org/packages/Grpc.AspNetCore)
  * Note call to `MapGrpcService` in `Startup.Configure` and to `services.AddGrpc` in `Startup.ConfigureServices` ([docs](https://docs.microsoft.com/en-us/aspnet/core/grpc/aspnetcore#add-grpc-services-to-an-aspnet-core-app))
* For gRPC Web support (Preview):
  * Add [Grpc.AspNetCore.Web](https://www.nuget.org/packages/Grpc.AspNetCore.Web)
  * Call `app.UseGrpcWeb()` and `endpoints.MapGrpcService<...Service>().EnableGrpcWeb();` for adding gRPC Web support
* What you need for the client:
  * gRPC Client for C# [on NuGet](https://www.nuget.org/packages/Grpc.Net.Client)
* What you need for libraries:
  * Protobuf Runtime [on NuGet](https://www.nuget.org/packages/Google.Protobuf/)
  * Protobuf Compiler [on NuGet](https://www.nuget.org/packages/Grpc.Tools/) (don't forget `PrivateAssets="All"` because tooling is not required at runtime)
  * gRPC metapackage [on NuGet](https://www.nuget.org/packages/Grpc/)

### Referencing *.proto* Files

* Install the [global tool `dotnet-grpc`](https://docs.microsoft.com/en-us/aspnet/core/grpc/dotnet-grpc) using `dotnet tool install -g dotnet-grpc`
* Import [Google's *error model*](https://cloud.google.com/apis/design/errors#error_model) using the global tool installed before:
  * `dotnet-grpc add-url -o google\rpc\status.proto -s Both https://raw.githubusercontent.com/googleapis/googleapis/master/google/rpc/status.proto`
  * `dotnet-grpc add-url -o google\rpc\code.proto -s Both https://raw.githubusercontent.com/googleapis/googleapis/master/google/rpc/code.proto`
* Note that you can refresh the content of the referenced files with `dotnet-grpc refresh [options] [<references>...]` ([docs](https://docs.microsoft.com/en-us/aspnet/core/grpc/dotnet-grpc#refresh))

### Generating TypeScript Client for gRPC Web

* [Details about gRPC Web](https://github.com/grpc/grpc-web)
* Setting up [gRPC Web in C#](https://docs.microsoft.com/en-us/aspnet/core/grpc/browser?view=aspnetcore-3.1)
* `protoc greet.proto --js_out=import_style=commonjs:. --grpc-web_out=import_style=typescript,mode=grpcwebtext:.`
* [Stackblitz client (completed)](https://stackblitz.com/edit/angular-a9wisf)

### Links

* [Documentation](https://docs.microsoft.com/en-us/aspnet/core/grpc/basics?view=aspnetcore-3.0#generated-c-assets)
* [gRPC on .NET Core 3](https://github.com/grpc/grpc-dotnet)
* [Streaming from Server](https://docs.microsoft.com/en-us/aspnet/core/grpc/client#server-streaming-call)
* [Build integration documentation](https://github.com/grpc/grpc/blob/master/src/csharp/BUILD-INTEGRATION.md)

## Go

### Install

* [Install Go](https://golang.org/doc/install)
* Install gRPC: `go get -u google.golang.org/grpc`
* Download proper *ProtoBuf Compiler* (*protoc*) from [https://github.com/google/protobuf/releases](https://github.com/google/protobuf/releases) and make sure it is in the *PATH*
* Install the *protoc plugin* for Go: `go get -u github.com/golang/protobuf/protoc-gen-go`
* Make sure `$GOPATH/bin` is in the *PATH*

