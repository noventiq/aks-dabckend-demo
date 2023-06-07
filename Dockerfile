#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# custom installation for RPI Grove dependencies defined in requirements.txt
RUN apt-get update 
RUN apt-get install -y curl

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["WebApiBritanico/WebApiBritanico.csproj", "WebApiBritanico/"]
RUN dotnet restore "WebApiBritanico/WebApiBritanico.csproj"
COPY . .
WORKDIR "/src/WebApiBritanico"
RUN dotnet build "WebApiBritanico.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebApiBritanico.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApiBritanico.dll"]