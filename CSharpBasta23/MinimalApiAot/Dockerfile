FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
RUN apt update \
    && apt install -y clang zlib1g-dev
COPY . /app
WORKDIR /app

RUN dotnet publish -c Release -r linux-x64 -o out

#FROM ubuntu
FROM scratch
ENV ASPNETCORE_URLS=http://+:5000
COPY --from=build /app/out /
EXPOSE 5000
CMD ["MinimalApiAot"]
