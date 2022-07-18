# Buildersoft Andy X Docker File

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Andy.X.App/Andy.X.App.csproj", "src/Andy.X.App/"]
COPY ["src/Andy.X.Utility/Andy.X.Utility.csproj", "src/Andy.X.Utility/"]
COPY ["src/Andy.X.Router/Andy.X.Router.csproj", "src/Andy.X.Router/"]
COPY ["src/Andy.X.IO/Andy.X.IO.csproj", "src/Andy.X.IO/"]
COPY ["src/Andy.X.Core/Andy.X.Core.csproj", "src/Andy.X.Core/"]
COPY ["src/Andy.X.Model/Andy.X.Model.csproj", "src/Andy.X.Model/"]
RUN dotnet restore "src/Andy.X.App/Andy.X.App.csproj"
COPY . .
WORKDIR "/src/src/Andy.X.App"
RUN dotnet build "Andy.X.App.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Andy.X.App.csproj" -c Release -o /app/publish


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Andy.X.App.dll"]
