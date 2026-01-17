ARG dotnet_version=10.0

FROM mcr.microsoft.com/dotnet/sdk:$dotnet_version AS build
WORKDIR /src

COPY --parents ["**/*.csproj", "."]
RUN dotnet restore "Trace/Trace.csproj" -r linux-x64

COPY . .
WORKDIR /src/Trace
RUN dotnet build -c Release -r linux-x64 --no-restore \
-p:DebugType=None \
-p:GenerateDocumentationFile=false


FROM build AS publish
RUN dotnet publish -c Release -r linux-x64 --no-build -o /app/publish \
-p:DebugType=None \
-p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:$dotnet_version AS runtime
WORKDIR /app
COPY --from=publish /app/publish .
ENV Twitch__ClientId=placeholder
ENV Twitch__ClientSecret=placeholder
ENTRYPOINT ["dotnet", "Trace.dll"]
