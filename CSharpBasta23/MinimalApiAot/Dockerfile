FROM mcr.microsoft.com/dotnet/nightly/sdk:8.0-jammy AS build
RUN apt update && apt install -y clang zlib1g-dev
COPY . /app
WORKDIR /app

RUN dotnet publish -c Release -r linux-x64 -o out

#FROM ubuntu
FROM mcr.microsoft.com/dotnet/nightly/runtime-deps:8.0-jammy-chiseled-aot
COPY --from=build /app/out /app
WORKDIR /app
CMD ["./MinimalApiAot"]
