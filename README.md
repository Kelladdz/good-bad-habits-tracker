# GoodBad Habits Tracker

[![Developed with ASP.NET](https://img.shields.io/badge/made_with-ASP.NET-8A2BE2)](https://asp.net) [![.NET version](https://img.shields.io/badge/.NET-8.0-8A2BE2)](https://dotnet.microsoft.com/) [![Entity Framework version](https://img.shields.io/badge/Entity_Framework-8.0.0-8A2BE2)](https://learn.microsoft.com/en-us/ef/) 
[![Entity Framework version](https://img.shields.io/badge/ASP.NET_Identity-8.0.0-8A2BE2)](https://learn.microsoft.com/en-us/ef/) 
![API version](https://img.shields.io/badge/API-v1-blue)

Web API for tracking habits powered by ASP.NET and Entity Framework. (Full app with client-side is in development)
## Technology stack

- .NET 8
-  Entity Framework 8.0.0
- ASP.NET Identity 8.0.0
- AutoMapper
- DateOnlyTimeOnly.Asp.Net by Maksym Koshovyi
- NewtonsoftJson
- 

## Run

- Clone this repo 
```
git clone https://github.com/Kelladdz/good-bad-habits-tracker.git
```
- Install .NET 8 [.NET Download Page](https://dotnet.microsoft.com/en-us/download)
- Install Entity Framework (with SQL Server database provider, design components, and tools for NuGet Package Manager Console) and Identity System
```
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.0
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.0
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 8.0.0
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore --version 8.0.0
```
- Install all necessary NuGet Packages
```
dotnet add package Asp.Versioning.Mvc --version 7.1.0
dotnet add package AutoMapper --version 12.0.1
dotnet add package DateOnlyTimeOnly.AspNet --version 2.1.1
dotnet add package DateOnlyTimeOnly.AspNet.Swashbuckle --version 2.1.1
dotnet add package Microsoft.AspNetCore.Http.Abstractions --version 2.2.0
dotnet add package Newtonsoft.Json --version 8.0.0
dotnet add package Swashbuckle.AspNetCore --version 6.5.0
dotnet add package Swashbuckle.AspNetCore.Filters --version 6.5.0
```
- Run API in PowerShell console
```
dotnet build
dotnet run
```
## API Endpoints

## v1


#### GET /API/v1/habits
Get list of currently logged in user's habits

##### Parameters
None

##### Response Scheme
```
```json
[
  {
    "habitId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "string",
    "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "user": {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "userName": "string",
      "normalizedUserName": "string",
      "email": "string",
      "normalizedEmail": "string",
      "emailConfirmed": true,
      "passwordHash": "string",
      "securityStamp": "string",
      "concurrencyStamp": "string",
      "phoneNumber": "string",
      "phoneNumberConfirmed": true,
      "twoFactorEnabled": true,
      "lockoutEnd": "2023-11-27T22:46:29.226Z",
      "lockoutEnabled": true,
      "accessFailedCount": 0,
      "habits": [
        "string"
      ],
      "avatar": "string"
    },
    "isGood": true,
    "isGoalInTime": true,
    "quantity": 0,
    "frequency": "string",
    "isRepeatDaily": true,
    "repeatDaysOfWeek": [
      "string"
    ],
    "repeatDaysOfMonth": [
      0
    ],
    "startDate": "2023-11-27",
    "reminderTime": "13:45:42.0000000",
    "statistics": {
      "streak": 0,
      "complete": 0,
      "failed": 0,
      "skipped": 0,
      "total": 0
    }
  }
]
```
#### GET /API/v1/habits/{habitId}
Get a habit with appropriate id.

##### Parameters
`Guid habitId` - Habit's unique identifier

##### Response Scheme
```
```json
{
  "habitId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "string",
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "user": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "userName": "string",
    "normalizedUserName": "string",
    "email": "string",
    "normalizedEmail": "string",
    "emailConfirmed": true,
    "passwordHash": "string",
    "securityStamp": "string",
    "concurrencyStamp": "string",
    "phoneNumber": "string",
    "phoneNumberConfirmed": true,
    "twoFactorEnabled": true,
    "lockoutEnd": "2023-11-27T22:48:50.484Z",
    "lockoutEnabled": true,
    "accessFailedCount": 0,
    "habits": [
      "string"
    ],
    "avatar": "string"
  },
  "isGood": true,
  "isGoalInTime": true,
  "quantity": 0,
  "frequency": "string",
  "isRepeatDaily": true,
  "repeatDaysOfWeek": [
    "string"
  ],
  "repeatDaysOfMonth": [
    0
  ],
  "startDate": "2023-11-27",
  "reminderTime": "13:45:42.0000000",
  "statistics": {
    "streak": 0,
    "complete": 0,
    "failed": 0,
    "skipped": 0,
    "total": 0
  }
}
```
#### POST /API/v1/habits
Create a habit and add it to database.

##### Parameters
`HabitDto habitDto` - Habit's Data Transfer Object creacted by user

#### PUT  /API/v1/habits/{id}
Update a habit specified by id.

##### Parameters
`HabitDto habitDto` - Habit's Data Transfer Object creacted by user
`Guid habitId` - Habit's unique identifier

#### DELETE /API/v1/habits

##### Parameters
None

#### DELETE /API/v1/habits/{habitId}

##### Parameters
`Guid habitId` - Habit's unique identifier
