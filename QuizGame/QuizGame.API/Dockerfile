FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

RUN apt-get update && apt-get install -y libkrb5-3

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["QuizGame.API/QuizGame.API.csproj", "QuizGame.API/"]
RUN dotnet restore "QuizGame.API/QuizGame.API.csproj"
COPY . .
WORKDIR "/src/QuizGame.API"
RUN dotnet publish "./QuizGame.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "QuizGame.API.dll"]


