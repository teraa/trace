ARG dotnet_version=10.0

FROM mcr.microsoft.com/dotnet/sdk:$dotnet_version AS build
WORKDIR /src

COPY --parents ["**/*.csproj", "."]
RUN dotnet restore "Trace/Trace.csproj" -r linux-x64

COPY . .
WORKDIR /src/Trace
RUN dotnet build -c Release --no-restore -r linux-x64 \
-p:DebugType=None \
-p:GenerateDocumentationFile=false


FROM build AS publish
RUN dotnet publish -c Release --no-build -o /app/publish -r linux-x64 \
-p:UseAppHost=false \
-p:DebugType=None


FROM mcr.microsoft.com/dotnet/aspnet:$dotnet_version AS runtime
WORKDIR /app
COPY --from=publish /app/publish .
ENV Twitch__ClientId=placeholder
ENV Twitch__ClientSecret=placeholder
ENTRYPOINT ["dotnet", "Trace.dll"]
