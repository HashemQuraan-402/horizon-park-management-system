# Horizon Adventure Park — Detailed Operations Guide

1. Project Overview

This project is a C# console application for Assignment 1: Theme Park Operations & Visitor Management System.

The system simulates daily theme park operations, including:

Visitor registration

Ticket issuing and validation

Ride access checking

Ride reservations

Ride capacity and occupancy

Ride status management

Employee registration and assignment

Facility management

Reservation cancellation

Ticket deactivation

Clear error handling for invalid operations

The application uses C# and .NET 10 and is implemented using beginner-level Object-Oriented Programming concepts.

2. Requirements

To run the project, install:

Visual Studio with the .NET 10 development tools, or

.NET 10 SDK for command-line use

The project targets:

net10.0

No database or external services are required.

All data is stored in memory while the program is running. Data is reset when the program is closed.

3. How to Run the Project

Option A — Visual Studio

Extract the ZIP file.

Open the project folder:

HorizonParkSystem

Open:

HorizonParkSystem.slnx

Build the solution:

Build → Build Solution

Run the program using:

F5

or:

Debug → Start Without Debugging

Option B — Command Line

Open a terminal inside the project folder:

HorizonParkSystem

Then run:

dotnet run

4. Main Menu

When the program starts, the following menu is displayed:

=== Horizon Adventure Park - Operations System ===
1. Register Visitor
2. Issue Ticket
3. Validate Ride Access
4. Create Reservation
5. Manage Ride (Add / Update Status)
6. Assign Staff
7. Cancel Reservation
8. Deactivate Ticket
9. View Ride Occupancy/Status
10. Register Employee
0. Exit

The program continues running until the user selects:

0. Exit

5. Starting Sample Data

The program automatically creates some sample data when it starts.

Rides

Thunder Peak Coaster
Category: Thrill
Minimum Age: 12
Minimum Height: 140 cm
Adult Required: No
Capacity: 20
Status: Open

Enchanted Carousel
Category: Family
Minimum Age: 0
Minimum Height: 0 cm
Adult Required: No
Capacity: 15
Status: Open

Splash Voyage
Category: Water
Minimum Age: 6
Minimum Height: 110 cm
Adult Required: Yes
Capacity: 10
Swim Safety Briefing: Yes
Status: Open

Facilities

Ticket Booth
Operations Office

Employee

Employee ID: E-01
Name: Alex Rivera
Role: RideOperator

These sample records allow the TA to start testing the system immediately without creating a ride or employee first.

6. Recommended First Test

A simple successful workflow is:

Step 1 — Register a visitor

Choose:

1

Example:

Visitor ID: V-1001
Visitor Name: Sara Ahmed
Age: 25
Height: 165
Category: General

Step 2 — Issue a ticket

Choose:

2

Enter:

Visitor ID: V-1001
Ticket Type: GeneralAdmission
Validity: 1

Step 3 — Check ride access

Choose:

3

Enter:

Visitor ID: V-1001
Ride Name: Enchanted Carousel

The result should be:

RESULT: ACCESS GRANTED

Step 4 — Create a reservation

Choose:

4

Enter:

Visitor ID: V-1001
Ride Name: Enchanted Carousel
Desired Time Slot: 14:00

If all rules are satisfied, the reservation is confirmed.

7. Important Menu Operations

Option 1 — Register Visitor

Registers a new visitor.

The system checks:

Visitor ID is not already registered

Name is not empty

Age is valid

Height is valid

Visitor category matches the visitor's age where applicable

Visitor categories include:

General
Child
Senior
VIP
StaffAccompaniedMinor

For children and staff-accompanied minors, the system also asks whether an adult is accompanying the visitor.

Option 2 — Issue Ticket

Issues a ticket to an existing visitor.

The ticket type must be appropriate for the visitor category.

Ticket types are:

GeneralAdmission
VIP
Child
Senior

The current ticket prices are:

GeneralAdmission = $49.99
VIP              = $129.99
Child            = $29.99
Senior           = $34.99

The ticket also receives a validity period entered by the user.

Option 3 — Validate Ride Access

Checks whether a visitor can enter a specific ride.

The system checks:

Visitor exists

Visitor has a ticket

Ticket is active

Ticket has not expired

Ticket type allows the ride category

Ride is open

Age requirement

Height requirement

Accompanying-adult requirement

Current direct-admission capacity

If access is denied, the program gives a specific reason.

Example:

RESULT: FAILED
Reason: Visitor does not meet the minimum height requirement (140cm) for this ride.

Option 4 — Create Reservation

Creates a reservation for a specific ride and time slot.

Example:

Visitor ID: V-1001
Ride Name: Splash Voyage
Desired Time Slot: 14:00

The system checks:

Visitor exists

Valid ticket

Ticket access level

Ride status

Ride eligibility

Duplicate reservation

Capacity for the selected time slot

The same visitor cannot make an active duplicate reservation for the same ride and time slot.

Option 5 — Manage Ride

Option 5 contains:

1. Add a new ride
2. Update ride status
3. Add a facility

Add a Ride

The user can create:

Thrill ride

Family ride

Water ride

The user enters the ride's:

Name

Category

Minimum age

Minimum height

Accompanying-adult requirement

Maximum capacity

Water rides also have a swim safety briefing option.

Update Ride Status

A ride can be:

Open
Closed
UnderMaintenance

Closed and maintenance rides do not accept new admissions or reservations.

Add a Facility

Facilities can be added as staff locations, for example:

Guest Services
First Aid Station
Operations Office

Option 6 — Assign Staff

Assigns an employee to a ride or facility for a shift.

Example:

Employee ID: E-01
Ride/Facility Name: Thunder Peak Coaster
Shift: Morning

The system prevents the same employee from being assigned to different locations during the same shift.

Different shifts are allowed.

Example:

E-01 → Thunder Peak Coaster → Morning
E-01 → Splash Voyage       → Afternoon

is allowed.

Option 7 — Cancel Reservation

Cancels an existing reservation using its reservation ID.

Example:

R-0001

Cancelling a reservation changes its status to Cancelled.

The cancelled reservation no longer counts as an active reservation for that time slot.

Option 8 — Deactivate Ticket

Deactivates an active ticket.

The ticket status becomes:

Cancelled

A cancelled ticket cannot be used for ride access or reservations.

Expired tickets are also automatically detected by the system when they are validated.

Option 9 — View Ride Occupancy/Status

Displays every ride with:

Ride category

Current status

Direct-admission occupancy

Maximum capacity

Minimum age

Minimum height

Adult requirement

Active reservation counts by time slot

Example:

Thunder Peak Coaster [Thrill] - Open - Occupancy 2/20
  Reservations: 14:00: 3/20 reserved, 15:00: 1/20 reserved

Option 10 — Register Employee

Registers a new employee.

The available roles are:

TicketBoothStaff
RideOperator
OperationsManager

Employee IDs must be unique.

Option 0 — Exit

Ends the current session:

Shutting down Horizon Adventure Park system. Goodbye!

All data is lost when the program exits because the assignment does not require persistent storage.

8. Ticket Access Rules

VIP tickets provide access to all ride categories.

Regular ticket types have limited access.

The current implementation uses:

VIP              → Family, Thrill, Water
GeneralAdmission → Family, Water
Child            → Family, Water
Senior           → Family, Water

This keeps VIP as the full-access ticket while regular tickets have limited access.

9. Reservation Capacity Design

Reservations are tracked by:

Ride + Time Slot

For example, if Splash Voyage has a capacity of 10:

14:00 → maximum 10 reservations
15:00 → maximum 10 reservations

A full 14:00 slot does not automatically make the 15:00 slot full.

Direct ride admissions are tracked separately using the ride's current occupancy.

Both direct admissions and reservations use the ride's declared MaxCapacity value.

10. Error Handling

The program is designed to continue running when an invalid operation is entered.

Examples of handled cases include:

Non-numeric input

Duplicate visitor ID

Invalid visitor category/age

Nonexistent visitor

Invalid ticket type for visitor category

Visitor without a ticket

Expired ticket

Cancelled ticket

Nonexistent ride

Closed ride

Ride under maintenance

Age requirement failure

Height requirement failure

Accompanying-adult requirement failure

Ticket access-tier failure

Full ride capacity

Full reservation time slot

Duplicate reservation

Nonexistent reservation

Conflicting employee assignment

Nonexistent employee

Nonexistent ride/facility

The program uses specific messages such as:

RESULT: FAILED
Reason: Visitor has no ticket on record.

instead of only displaying a generic error message.

11. Project Structure

HorizonParkSystem/
│
├── HorizonParkSystem.slnx
├── HorizonParkSystem.csproj
├── Program.cs
├── ParkSystem.cs
├── Models.cs
├── Rides.cs
├── Enums.cs
├── ParkException.cs
└── README.md

File descriptions

File

Purpose

Program.cs

Main menu, console input/output, and menu operations

ParkSystem.cs

Stores data and applies the main business rules

Models.cs

Visitor, Ticket, Reservation, and Employee classes

Rides.cs

Ride base class and Thrill, Family, and Water ride classes

Enums.cs

Visitor, ticket, ride, reservation, employee role, and status enums

ParkException.cs

Custom exception used for business-rule errors

HorizonParkSystem.csproj

.NET project configuration

HorizonParkSystem.slnx

Visual Studio solution file

12. Testing

A detailed test plan is included in the project documentation.

The tests cover:

Visitor registration

Ticket issuing

Ride access

Reservations

Reservation cancellation

Ride management

Staff management

Ticket deactivation

Ride status/occupancy viewing

Invalid input

Phase 5 error scenarios

Specific eligibility failure messages

For the assignment's Phase 5 testing, important scenarios include:

Non-numeric input

Visitor without a ticket

Expired ticket

Cancelled ticket

Full reservation capacity

Age requirement failure

Height requirement failure

Nonexistent ride

Closed ride

Ride under maintenance

Reuse of cancelled/expired ticket

Conflicting employee assignment

Correct reason when one eligibility rule fails

Meaningful error messages

13. Notes for the Teaching Assistant

This is a console-based, in-memory application.

No database setup or external configuration is required.

To test the application quickly:

Run the project.

Use the seeded rides and employee.

Register a visitor using Option 1.

Issue a suitable ticket using Option 2.

Test access using Option 3.

Test reservations using Option 4.

Use Option 5 to add/update rides or facilities.

Use Option 6 to assign staff.

Use Option 7 to cancel reservations.

Use Option 8 to deactivate tickets.

Use Option 9 to view ride status and reservation counts.

Use Option 0 to exit.

The system does not save data between runs, so restarting the application restores the initial sample data.
