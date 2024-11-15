# Use the official .NET SDK image as the base image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory inside the container
WORKDIR /app

# Copy project files
COPY Stock.DataCenter.Insert.Manual/Stock.DataCenter.Insert.Manual/Stock.DataCenter.Insert.Manual.csproj ./Stock.DataCenter.Insert.Manual/Stock.DataCenter.Insert.Manual/
RUN dotnet restore ./Stock.DataCenter.Insert.Manual/Stock.DataCenter.Insert.Manual/Stock.DataCenter.Insert.Manual.csproj

# Copy all source files
COPY . .

# Build the application
RUN dotnet build ./Stock.DataCenter.Insert.Manual/Stock.DataCenter.Insert.Manual/Stock.DataCenter.Insert.Manual.csproj -c Release -o /app/build

# Use a smaller runtime image to run the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Set the working directory inside the runtime container
WORKDIR /app

# Copy build output from the build stage
COPY --from=build /app/build ./


# Set the entry point to run the .NET application
ENTRYPOINT ["dotnet", "Stock.DataCenter.Insert.Manual.dll"]
