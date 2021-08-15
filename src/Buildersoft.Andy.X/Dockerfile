FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/Buildersoft.Andy.X/Buildersoft.Andy.X.csproj", "src/Buildersoft.Andy.X/"]
COPY ["src/Buildersoft.Andy.X.Data.Model/Buildersoft.Andy.X.Data.Model.csproj", "src/Buildersoft.Andy.X.Data.Model/"]
COPY ["src/Buildersoft.Andy.X.Logic/Buildersoft.Andy.X.Logic.csproj", "src/Buildersoft.Andy.X.Logic/"]
COPY ["src/Buildersoft.Andy.X.Utilities/Buildersoft.Andy.X.Utilities.csproj", "src/Buildersoft.Andy.X.Utilities/"]
COPY ["src/Buildersoft.Andy.X.Router/Buildersoft.Andy.X.Router.csproj", "src/Buildersoft.Andy.X.Router/"]
COPY ["src/Buildersoft.Andy.X.FileConfig/Buildersoft.Andy.X.IO.csproj", "src/Buildersoft.Andy.X.FileConfig/"]
RUN dotnet restore "src/Buildersoft.Andy.X/Buildersoft.Andy.X.csproj"
COPY . .
WORKDIR "/src/src/Buildersoft.Andy.X"
RUN dotnet build "Buildersoft.Andy.X.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Buildersoft.Andy.X.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Buildersoft.Andy.X.dll"]