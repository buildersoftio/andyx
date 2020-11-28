FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Buildersoft.Andy.X/Buildersoft.Andy.X.csproj", "Buildersoft.Andy.X/"]
COPY ["Buildersoft.Andy.X.Data.Model/Buildersoft.Andy.X.Data.Model.csproj", "Buildersoft.Andy.X.Data.Model/"]
COPY ["Buildersoft.Andy.X.Logic/Buildersoft.Andy.X.Logic.csproj", "Buildersoft.Andy.X.Logic/"]
COPY ["Buildersoft.Andy.X.Utilities/Buildersoft.Andy.X.Utilities.csproj", "Buildersoft.Andy.X.Utilities/"]
COPY ["Buildersoft.Andy.X.Router/Buildersoft.Andy.X.Router.csproj", "Buildersoft.Andy.X.Router/"]
COPY ["Buildersoft.Andy.X.FileConfig/Buildersoft.Andy.X.FileConfig.csproj", "Buildersoft.Andy.X.FileConfig/"]
RUN dotnet restore "Buildersoft.Andy.X/Buildersoft.Andy.X.csproj"
COPY . .
WORKDIR "/src/Buildersoft.Andy.X"
RUN dotnet build "Buildersoft.Andy.X.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Buildersoft.Andy.X.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Buildersoft.Andy.X.dll"]