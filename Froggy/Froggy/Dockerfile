FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Froggy/Froggy.csproj", "Froggy/"]
RUN dotnet restore "Froggy/Froggy.csproj"
COPY . .
WORKDIR "/src/Froggy"
RUN dotnet build "Froggy.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Froggy.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Froggy.dll"]
