# Use the official ASP.NET Core runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Copy the pre-built application from the local bin folder
COPY ["./bin/Release/net8.0", "."]

ENTRYPOINT ["dotnet", "Stock.DataCenter.Insert.Manual.dll"]