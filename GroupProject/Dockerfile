#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["GroupProject/GroupProject.csproj", "GroupProject/"]
RUN dotnet restore "GroupProject/GroupProject.csproj"
COPY . .
WORKDIR "/src/GroupProject"
RUN dotnet build "GroupProject.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GroupProject.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY ["GroupProject/log4net.config", "."]
COPY ["GroupProject/GroupProject.xml", "."]
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GroupProject.dll"]