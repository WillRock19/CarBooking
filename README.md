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

> **IMPORTANT**: All reservation's dates are [UTC dates](https://stackoverflow.com/questions/16307563/utc-time-explanation), so every request for a reservation should take this in consideration.

# How to run

## Running with docker

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

## Running with .NET Core 7.0

> **IMPORTANT**: You should already have [.NET Core 7](https://dotnet.microsoft.com/en-us/download/dotnet/7.0) installed. 

* Clonet the project;
* Navigate to the project root;
* Go to **src\CarReservation.Api**;
* Run **dotnet run**;
* Go to your favorite browser;
* Type: **http://localhost:5225/index.html**;

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

<Explain the architectural decisions.>

