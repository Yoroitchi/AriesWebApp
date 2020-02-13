#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
COPY /home/indy/Téléchargements/lib AriesWebApp/
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY [".", "AriesWebApp/"]
RUN dotnet restore "AriesWebApp/AriesWebApp.csproj" \
    -s "https://api.nuget.org/v3/index.json"
COPY . .
WORKDIR "/src/AriesWebApp"
RUN dotnet build "AriesWebApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AriesWebApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AriesWebApp.dll"]
