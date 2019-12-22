FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["KurirServer/KurirServer.csproj", "KurirServer/"]
RUN dotnet restore "KurirServer/KurirServer.csproj"
COPY . .
WORKDIR "/src/KurirServer"
RUN dotnet build "KurirServer.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "KurirServer.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "KurirServer.dll"]