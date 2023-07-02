[![dotnet package](https://github.com/WillRock19/CarBooking/actions/workflows/build_and_run_tests.yml/badge.svg?branch=master)](https://github.com/WillRock19/CarBooking/actions/workflows/build_and_run_tests.yml)

# Introduction

At this moment, this project is a basic Reservation API. The idea is to allow any user that sends a request to reserv a car to be used at a specific moment in time in the next 24 hours.

This API supports basically two flows:

* Car CRUD flow - basically, all operations of Create, Update, Remove, GetAll (without pagination) and GetById associated with the car entity;
* Reservation flow - basically, the reservation of a car for a specific moment in time and a GetAll upcoming reservations;

All API communication is made via **JSON** objects.

# Domain

There's a couple of rules important to understand about how the car and the reservations work. In this section, we'll explain them.

## Car

The car is represented by a **Make**, a **Model** and an **ID**, which should follow the pattern **C<number>** for each car.

## Reservation

The reservation is always associated with a car. If no car exists, an error message is return for a user that tries to register a Reservation.

The reservation is represented by an **Id**, a **CarId**, an **InitialDate**, a **DurationInMinutes** and an **EndDate**, which is computed using the InitialDate and the DurationInMinutes.

The reservation can be taken up to 24 hours ahead and have a duration up to 2 hours (e.g. if now you are at 10 AM of a Saturday, your reservation can be made until 10 AM of Sunday and have the maximum duration of 2 hours).

# How to run

<Explain how to run this project>

# Technologies

The API is written in .NET Core 7, and uses the following libraries:

* AutoMapper;
* FluentValidation;
* Swagger;

The unit tests are made with the following technologies:

* NUnit;
* FluentAssertions;

The api tests are made with the following technologies:

* NUnit;
* FluentAssertions;
* Microsoft.AspNetCore.Mvc.Testing;
* Microsoft.AspNetCore.TestHost;

# Architectural Decisions

<Explain the architectural decisions.>

