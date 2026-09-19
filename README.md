# Horizon Adventure Park Management System

A .NET console application that models visitor, ticket, ride, reservation, facility, and employee operations for a theme park.

## Features

- Visitor registration with category and age validation
- Ticket issue, validation, pricing, expiration, and deactivation
- Ride access rules for age, height, adult accompaniment, capacity, and status
- Reservation creation and cancellation
- Thrill, family, and water ride types
- Employee registration and conflict-aware shift assignments
- Facilities and ride occupancy reporting
- Friendly business-rule errors through a custom exception
- Seed data for immediate demonstration

## Technology and design

- C# and .NET 10
- Object-oriented domain models
- Inheritance and polymorphism for ride types
- Collections and service-style operations
- In-memory storage; data resets when the program exits

## Run

```powershell
dotnet restore
dotnet run
```

Or open `HorizonParkSystem.slnx` in a compatible Visual Studio version.

## Suggested demo

1. Start the program; sample rides, facilities, and one employee already exist.
2. Choose **1** and register visitor `V-1001`.
3. Choose **2** and issue a matching ticket.
4. Choose **3** to validate access to **Enchanted Carousel**.
5. Choose **4** to reserve a time slot.
6. Choose **9** to review ride occupancy and status.

The complete field-by-field walkthrough and business rules are documented in [docs/DETAILED_GUIDE.md](docs/DETAILED_GUIDE.md).

## Build validation

```powershell
dotnet build
```

## Limitations

This educational version stores all records in memory and has no authentication, database, or automated test project. Production use would require persistent storage, concurrency handling, access control, structured logging, and automated tests.

