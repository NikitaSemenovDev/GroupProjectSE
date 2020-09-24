FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["GroupProjectSE/GroupProjectSE.csproj", "GroupProjectSE/"]
RUN dotnet restore "GroupProjectSE/GroupProjectSE.csproj"
COPY . .
WORKDIR "/src/GroupProjectSE"
RUN dotnet build "GroupProjectSE.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "GroupProjectSE.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY ["GroupProjectSE/log4net.config", "."]
COPY ["GroupProjectSE/GroupProjectSE.xml", "."]
COPY ["GroupProjectSE/appsettings.json", "."]
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "GroupProjectSE.dll"]