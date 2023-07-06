[![dotnet package](https://github.com/WillRock19/CarBooking/actions/workflows/build_and_run_tests.yml/badge.svg?branch=master)](https://github.com/WillRock19/CarBooking/actions/workflows/build_and_run_tests.yml)

# Introduction

At this moment, this project is a basic Reservation API. The idea is to allow any user that sends a request to reserve a car during a specific time in the next 24 hours.

This API supports basically two flows:

* Car flow - basically, all CRUD operations associated with a car, like Create, Update, Remove, GetAll (without pagination) and GetById;
* Reservation flow - basically, operations associated with the reservation of a car, like reserving for a specific moment in time and a GetAll upcoming reservations;

All APIs communication should be made using **JSON** objects.

# Domain

There's a couple of rules important to understand about how the car and the reservations works. In this section, we'll explain them.

## Car

The car is represented by a **Make**, a **Model** and an **ID**, which is a string that follows the pattern **C<number>**.

## Reservation

The reservation is always associated with a car. If no car exists, an error message is return for a user that tries to register a Reservation.

The reservation is represented by an **Id**, which is a Guid, a **CarId**, an **InitialDate**, a **DurationInMinutes** and an **EndDate**, which is computed using the InitialDate + DurationInMinutes.

The reservation can be taken up to 24 hours ahead and have a duration up to 2 hours (e.g. if now you are at 10 AM of a Saturday, your reservation can be made at most for 10 AM of Sunday and have the maximum duration of 2 hours).

To avoid milliseconds problems when receiving the request, the reservations should be made for at least 5 minutes from current date (in UTC time).

> **IMPORTANT**: All reservation's dates are [UTC dates](https://stackoverflow.com/questions/16307563/utc-time-explanation), so every request for a reservation should take this in consideration.

# How to run

## Running the API with docker

> **IMPORTANT**: You should already have [docker](https://docs.docker.com/engine/install/) installed.

There's a Dockerfile in the root directory that will build and run the project inside a container. 

To execute it, you can run the **build.ps1** (for Windows) or **build.sh** (for UNIX base OS) scripts. Both scripts are going to create an image called **carbooking-image** and then run the containner mapping it to localhost port 5000. 

### Running inside a Windows

* Clone the project;
* Open a Powershell terminal;
* Navigate to the project root;
* Run **./build.ps1**
* Wait for the console to print the container ID (e.g. 1720dbf178abbfd5f817e79748da1e86368c954bdf19a10993e4b17e7aee491b)
* Go to your favorite browser;
* Type **http://localhost:5000/index/html**
* Enjoy the application :)

### Running inside a Linux base OD

* Clone the project;
* Open your terminal;
* Navigate to the project root;
* Make the script file executable with **chmod +x run-container.sh**;
* Run **./build.sh**
* Wait for the console to print the container ID (e.g. 1720dbf178abbfd5f817e79748da1e86368c954bdf19a10993e4b17e7aee491b)
* Go to your favorite browser;
* Type **http://localhost:5000/index/html**
* Enjoy the application :)

## Running the API with .NET Core 7.0

> **IMPORTANT**: You should already have [.NET Core 7](https://dotnet.microsoft.com/en-us/download/dotnet/7.0) installed. 

* Clonet the project;
* Navigate to the project root;
* Go to **src\CarReservation.Api**;
* Run **dotnet run**;
* Go to your favorite browser;
* Type: **http://localhost:5225/index.html**;

## Running the API with .NET + docker

You could also generate a docker image if you have the .NET Core Cmdline installed. 

This is a new feature, that comes with .NET Core 7.0. To do this, you basically needs to:

1. Run the command to generate a docker image with .NET

```
dotnet publish --os linux --arch x64 -p:PublishProfile=DefaultContainer -p:ContainerImageName=carreservation-api
``` 

2. Run docker with the generated image, **carreservation-api:<VERSION_GENERATED>**. In our example, the version is **1.0.0**.

```
docker run -p 5000:80 carreservation-api:1.0.0
```

To learn more, check this link.

# Technologies

The API is written in .NET Core 7, and uses the following libraries:

* [AutoMapper](https://automapper.org/);
* [FluentValidation](https://docs.fluentvalidation.net/en/latest/);
* [Swagger](https://swagger.io/);

The unit tests are made with the following technologies:

* [NUnit](https://nunit.org/);
* [FluentAssertions](https://fluentassertions.com/);
* [Moq](https://github.com/moq/moq);

The api tests are made with the following technologies:

* [NUnit](https://nunit.org/);
* [FluentAssertions](https://fluentassertions.com/);
* [Microsoft.AspNetCore.Mvc.Testing](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.testing?view=aspnetcore-7.0);
* [Microsoft.AspNetCore.TestHost](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.testhost?view=aspnetcore-7.0);

# Architectural Decisions

## API structure

The project basically uses a Controller manage the flow, a Service to apply the logic and a Repositories. Each of then could be interpreted as different layers for the API. Since this is a small project, we decided to keep all of them inside the **CarReservation.Api**. 

If the project gets bigger, we could follow one of the following approachs: 

* 1. Export the classes of each folder into it's own project's layer;
* 2. Keep it as it is and treat the CarReservation.Api as a microsservice; 

The decision should be made only when needed. For now, this simple structure solves the client's need. 

## Validations

The model validations for the user's requests are being made in two steps:

* 1. Validating if the data can be used;
* 2. Validating if the data is acceptable;

The first step is beng made at controller level, where the controller checks if the parameter can be processed and then returns any possible error.

The second step is being made inside a specific validation class. We are using FluentValidation to add all business validations, which should check if the received data can be processed (is valid).

## Request/Response models

To keep our api context separated from the outside world, we decided to create models for the request/response operations, which are in the folder DTO. 

The idea here is to allow the development of the API without affecting any consumer, or even evolve the communication with the consumers without affecting the internal logic of the API. 

Basically, applying some concepts of the [Hexagonal Architecture pattern](https://alistair.cockburn.us/hexagonal-architecture/), where we have the Adapters (here represented by our AutoMapper) to adapt any outside request to an internal model, and any internal model to an outside response.

## Choosing an available car to create a reservation

Currently, the logic of choosing an available car for a reservation might create some pitfalls. It could happen that only the same car will be chosen more times than any other. 

Since the original problem statement didn't specified any rule to choose between all the available cars, we are always using the first one that's free.

A possible improvement could be applying some rule like **if the car wasn't under use for the past hour**, o even **a random car from the list**, but that depends on the further specifications.

## Automated tests

There are two types of tests in this project:

* 1. **Unit tests:** usually checks the methods execution by itself, mocking it's depedencies;
* 2. **Api tests:** tests the api from a user standpoint, making requests and expecting responses in a [blackbox approach](https://en.wikipedia.org/wiki/Black-box_testing);
