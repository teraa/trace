ARG dotnet_version=8.0

FROM mcr.microsoft.com/dotnet/sdk:$dotnet_version AS build
WORKDIR /src
ARG configuration=Release
ARG github_username
ARG github_pat

RUN dotnet nuget add source \
--username $github_username \
--password $github_pat \
--store-password-in-clear-text \
--name github "https://nuget.pkg.github.com/teraa/index.json"

COPY ["Trace/Trace.csproj", "Trace/"]
COPY ["Extensions/AspNetCore/Extensions.AspNetCore.csproj", "Extensions/AspNetCore/"]
COPY ["Extensions/Configuration.Vault.Options/Extensions.Configuration.Vault.Options.csproj", "Extensions/Configuration.Vault.Options/"]
COPY ["Extensions/Configuration.Vault/Extensions.Configuration.Vault.csproj", "Extensions/Configuration.Vault/"]
COPY ["Extensions/Configuration/Extensions.Configuration.csproj", "Extensions/Configuration/"]
COPY ["Extensions/Serilog.Seq/Extensions.Serilog.Seq.csproj", "Extensions/Serilog.Seq/"]
COPY ["Extensions/Serilog.Systemd/Extensions.Serilog.Systemd.csproj", "Extensions/Serilog.Systemd/"]
RUN dotnet restore "Trace/Trace.csproj"

COPY . .
WORKDIR "/src/Trace"
RUN dotnet build "Trace.csproj" -c $configuration -o /app/build


FROM build AS publish
RUN dotnet publish "Trace.csproj" -c $configuration -o /app/publish /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:$dotnet_version AS runtime
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Trace.dll"]
