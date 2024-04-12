# docker run -d -e TZ=Australia/Sydney --name stockasx.data.collection -p 9901:9902 --log-opt max-size=10m --log-opt max-file=3 stockasx/datacollection
# docker build -t stockasx/datacollection .

# Use the official ASP.NET Core runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Copy the pre-built application from the local bin folder
COPY ["./bin/Release/net8.0", "."]

ENTRYPOINT ["dotnet", "Stock.DataCenter.Insert.Manual.dll"]