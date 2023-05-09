FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /App
COPY . ./
RUN dotnet restore
RUN dotnet publish -c Debug -o out

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /App
COPY --from=build-env /App/out .
ENTRYPOINT ["dotnet", "WebApp.dll"]

# docker build -t acrsbwa7dm5lugos.azurecr.io/azmgedid .
# docker push acrsbwa7dm5lugos.azurecr.io/azmgedid
