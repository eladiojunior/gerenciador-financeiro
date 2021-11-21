FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["gfin.webapi/gfin.webapi.csproj", "gfin.webapi/"]
RUN dotnet restore "gfin.webapi/gfin.webapi.csproj"
COPY . .
WORKDIR "/src/gfin.webapi"
RUN dotnet build "gfin.webapi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "gfin.webapi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "gfin.webapi.dll"]
