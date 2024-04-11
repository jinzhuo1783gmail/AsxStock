# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
WORKDIR /src
COPY ["Stock.Extraction.Library/Stock.Extraction.Library.csproj", "Stock.Extraction.Library/"]
COPY ["Stock.Information.Extraction/Stock.Information.Extraction.csproj", "Stock.Information.Extraction/"]

RUN dotnet restore "Stock.Information.Extraction/Stock.Information.Extraction.csproj"

# Copy everything else and build
COPY . ./

WORKDIR "/src/Stock.Information.Extraction"
RUN dotnet build "Stock.Information.Extraction.csproj" -c Release -o /app/out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:3.1
WORKDIR /app
EXPOSE 80
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "Stock.Information.Extraction.dll"]