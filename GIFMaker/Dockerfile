FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["GIFMaker/GIFMaker.csproj", "GIFMaker/"]
RUN dotnet restore "GIFMaker/GIFMaker.csproj"
COPY . .
WORKDIR "/src/GIFMaker"
RUN dotnet build "GIFMaker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GIFMaker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GIFMaker.dll"]