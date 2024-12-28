ARG dotnet_version=8.0

FROM mcr.microsoft.com/dotnet/sdk:$dotnet_version AS build
WORKDIR /src
ARG github_username
ARG github_pat

RUN dotnet nuget add source \
--username $github_username \
--password $github_pat \
--store-password-in-clear-text \
--name github "https://nuget.pkg.github.com/teraa/index.json"

COPY ["Trace/Trace.csproj", "Trace/"]
COPY ["Shared/AspNetCore/Shared.AspNetCore.csproj", "Shared/AspNetCore/"]
COPY ["Shared/Configuration.Vault/Shared.Configuration.Vault.csproj", "Shared/Configuration.Vault/"]
COPY ["Shared/Configuration/Shared.Configuration.csproj", "Shared/Configuration/"]
COPY ["Shared/AspNetCore.Identity/Shared.AspNetCore.Identity.csproj", "Shared/AspNetCore.Identity/"]
COPY ["Shared/Serilog.Seq/Shared.Serilog.Seq.csproj", "Shared/Serilog.Seq/"]
COPY ["Shared/Serilog.Systemd/Shared.Serilog.Systemd.csproj", "Shared/Serilog.Systemd/"]
RUN dotnet restore "Trace/Trace.csproj"

COPY . .
WORKDIR "/src/Trace"
RUN dotnet build "Trace.csproj" -c Release -o /app/build


FROM build AS publish
RUN dotnet publish "Trace.csproj" -c Release -o /app/publish /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:$dotnet_version AS runtime
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_ENVIRONMENT=Docker
ENTRYPOINT ["dotnet", "Trace.dll"]
