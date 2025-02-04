# Use uma imagem base do .NET para produção
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

# Use a imagem SDK do .NET para build
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["WebApplication0/WebApplication0.csproj", "WebApplication0/"]
RUN dotnet restore "WebApplication0/WebApplication0.csproj"
COPY . .
WORKDIR "/src/WebApplication0"
RUN dotnet build "WebApplication0.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebApplication0.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApplication0.dll"]
