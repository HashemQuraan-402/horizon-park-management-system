using System;
using System.Collections.Generic;
using System.Text;

namespace HorizonParkSystem
{
    // Category of the visitor. This decides which ticket types are allowed
    // and affects eligibility checks (e.g. Child needs an adult on some rides).
    public enum VisitorCategory
    {
        General,
        Child,
        Senior,
        VIP,
        StaffAccompaniedMinor
    }

    // Type of ticket a visitor can be issued.
    // VIP tickets unlock every ride. The others only unlock certain ride categories.
    public enum TicketType
    {
        GeneralAdmission,
        VIP,
        Child,
        Senior
    }

    // The status of a ticket over its lifetime.
    public enum TicketStatus
    {
        Active,
        Expired,
        Cancelled
    }

    // What kind of ride this is. Used to decide which ticket types can enter it.
    public enum RideCategory
    {
        Family,
        Thrill,
        Water
    }

    // Operational status of a ride.
    public enum RideStatus
    {
        Open,
        Closed,
        UnderMaintenance
    }

    // Status of a reservation.
    public enum ReservationStatus
    {
        Active,
        Cancelled
    }

    // Roles an employee can have.
    public enum EmployeeRole
    {
        TicketBoothStaff,
        RideOperator,
        OperationsManager
    }

}
