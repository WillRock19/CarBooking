# # Loading image to use in build-env 
# FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env

# # Set the working directory
# WORKDIR /app

# # Copy the project file and restore dependencies
# COPY src/CarReservation.Api/*.csproj ./
# RUN dotnet restore

# # Copy the remaining source code
# COPY src/CarReservation.Api/ ./

# # Build the application
# RUN dotnet build --configuration Release

# # Run tests (optional)
# RUN dotnet test --configuration Release --no-build --verbosity normal

# # Run dotnet publish
# RUN dotnet publish -c Release -o out

# # Loading image to run
# FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final

# # Set the working directory
# WORKDIR /app
# COPY --from=build-env /app/out .
# ENTRYPOINT ["dotnet", "CarReservation.Api.dll"]

# Loading image to use in build-env 
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env

# Set the working directory
WORKDIR /app

# Copy the solution and the API/tests .csproj files
COPY *.sln .
COPY src/CarReservation.Api/*.csproj ./src/CarReservation.Api/
COPY test/CarReservation.Api.Tests.Unit/*.csproj ./test/CarReservation.Api.Tests.Unit/
COPY test/CarReservation.Api.Tests.Integration/*.csproj ./test/CarReservation.Api.Tests.Integration/

# Restore package depedencies for solution
RUN dotnet restore

# Copy the remaining source code
COPY . .

# Build the application
RUN dotnet build

# run the unit tests
FROM build-env AS test

# set the directory to be within the test folder
WORKDIR /app/test

# run the unit tests
RUN dotnet test --logger:trx

# Loading image to run
FROM build-env AS publish

# Set the working directory
WORKDIR /app/src/CarReservation.Api

# Publish the web api project to a directory called out
RUN dotnet publish -c Release -o out

# create a new layer using aspnet runtime image
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS runtime
WORKDIR /app

# copy over the files produced when publishing the service
COPY --from=publish /app/src/CarReservation.Api/out .

# run the web api when the docker image is started
ENTRYPOINT ["dotnet", "CarReservation.Api.dll"]