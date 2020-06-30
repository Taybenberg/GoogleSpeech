#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["GoogleSpeechUkrBotWorker/GoogleSpeechUkrBotWorker.csproj", "GoogleSpeechUkrBotWorker/"]
COPY ["GoogleSpeechUkrBot/GoogleSpeechUkrBot.csproj", "GoogleSpeechUkrBot/"]
COPY ["GoogleTTS/GoogleTTS.csproj", "GoogleTTS/"]
COPY ["GoogleSTT/GoogleSTT.csproj", "GoogleSTT/"]
RUN dotnet restore "GoogleSpeechUkrBotWorker/GoogleSpeechUkrBotWorker.csproj"
COPY . .
WORKDIR "/src/GoogleSpeechUkrBotWorker"
RUN dotnet build "GoogleSpeechUkrBotWorker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GoogleSpeechUkrBotWorker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GoogleSpeechUkrBotWorker.dll"]