# Use the official .NET SDK image as the base image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory inside the container
WORKDIR /app

# Install Python and pip
RUN apt-get update && \
    apt-get install -y python3 python3-pip python3-venv && \
    rm -rf /var/lib/apt/lists/*

# Set up a virtual environment and install youtube-transcript-api
RUN python3 -m venv /app/venv && \
    /app/venv/bin/pip install youtube-transcript-api

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

# Copy the virtual environment with Python packages from the build stage
COPY --from=build /app/venv /app/venv

# Set environment variable to use the virtual environment's Python as the default Python
ENV PATH="/app/venv/bin:$PATH"

# Set the entry point to run the .NET application
ENTRYPOINT ["dotnet", "Stock.DataCenter.Insert.Manual.dll"]
