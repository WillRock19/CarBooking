 # Loading image to use in build-env 
 FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env

# Set the working directory
WORKDIR /app

# # Copy the project file and restore dependencies
COPY src/CarReservation.Api/*.csproj ./
RUN dotnet restore

# # Copy the remaining source code
COPY src/CarReservation.Api/ ./

# # Build the application
RUN dotnet build --configuration Release

# # Run dotnet publish
RUN dotnet publish --configuration Release --output out

# # Loading image to run
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
EXPOSE 80

# # Set the working directory
 WORKDIR /app
 COPY --from=build-env /app/out ./
 ENTRYPOINT ["dotnet", "CarReservation.Api.dll"]