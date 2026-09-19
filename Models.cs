using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;

namespace HorizonParkSystem
{
    // Represents a park guest.
    public class Visitor
    {
        public string VisitorId { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public int HeightCm { get; set; }
        public VisitorCategory Category { get; set; }

        // True only for a "StaffAccompaniedMinor" who is currently with an adult.
        // Kept simple: staff toggles this on/off when checking the visitor in.
        public bool HasAccompanyingAdult { get; set; }

        public Visitor(string visitorId, string name, int age, int heightCm,
            VisitorCategory category, bool hasAccompanyingAdult)
        {
            VisitorId = visitorId;
            Name = name;
            Age = age;
            HeightCm = heightCm;
            Category = category;
            HasAccompanyingAdult = hasAccompanyingAdult;
        }

        public override string ToString()
        {
            return $"{VisitorId} - {Name} (Age {Age}, {HeightCm}cm, {Category})";
        }
    }

    // Represents a ticket issued to a visitor.
    public class Ticket
    {
        public string TicketId { get; set; }
        public string VisitorId { get; set; }
        public TicketType Type { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public TicketStatus Status { get; set; }
        public decimal Price { get; set; }

        public Ticket(string ticketId, string visitorId, TicketType type,
            DateTime issueDate, DateTime expiryDate, decimal price)
        {
            TicketId = ticketId;
            VisitorId = visitorId;
            Type = type;
            IssueDate = issueDate;
            ExpiryDate = expiryDate;
            Price = price;
            Status = TicketStatus.Active;
        }

        // A ticket is only usable if it is marked Active AND not past its expiry date.
        public bool IsValid()
        {
            if (Status != TicketStatus.Active)
            {
                return false;
            }

            if (DateTime.Now > ExpiryDate)
            {
                // Auto-expire it the moment we notice, so the state stays consistent.
                Status = TicketStatus.Expired;
                return false;
            }

            return true;
        }

        // Decides which ride categories this ticket type is allowed to enter.
        // VIP tickets can access everything, per the assignment rules.
        public bool AllowsAccessTo(RideCategory category)
        {
            switch (Type)
            {
                case TicketType.VIP:
                    // VIP is the only tier that unlocks every ride category,
                    // per the assignment's access tier rule.
                    return true;
                case TicketType.GeneralAdmission:
                    // Regular tickets provide LIMITED access - General
                    // admission covers Family and Water rides, but not
                    // the higher-thrill/higher-risk Thrill category.
                    return category == RideCategory.Family || category == RideCategory.Water;
                case TicketType.Child:
                    // Children can go on Family and Water rides, not Thrill rides.
                    return category == RideCategory.Family || category == RideCategory.Water;
                case TicketType.Senior:
                    return category == RideCategory.Family || category == RideCategory.Water;
                default:
                    return false;
            }
        }

        public override string ToString()
        {
            return $"{TicketId} ({Type}, {Status}, expires {ExpiryDate:d})";
        }
    }

    // Represents a booked slot for a visitor on a ride.
    public class Reservation
    {
        public string ReservationId { get; set; }
        public string VisitorId { get; set; }
        public string RideName { get; set; }
        public string TimeSlot { get; set; }
        public ReservationStatus Status { get; set; }

        public Reservation(string reservationId, string visitorId, string rideName, string timeSlot)
        {
            ReservationId = reservationId;
            VisitorId = visitorId;
            RideName = rideName;
            TimeSlot = timeSlot;
            Status = ReservationStatus.Active;
        }

        public override string ToString()
        {
            return $"{ReservationId} - {RideName} @ {TimeSlot} for {VisitorId} ({Status})";
        }
    }

    // Represents a staff member.
    public class Employee
    {
        public string EmployeeId { get; set; }
        public string Name { get; set; }
        public EmployeeRole Role { get; set; }

        // Where the employee is currently assigned, and during which shift.
        // Null/empty means "not currently assigned".
        public string? AssignedLocation { get; set; }
        public string? AssignedShift { get; set; }

        public Employee(string employeeId, string name, EmployeeRole role)
        {
            EmployeeId = employeeId;
            Name = name;
            Role = role;
            AssignedLocation = null;
            AssignedShift = null;
        }

        public override string ToString()
        {
            string assignment = AssignedLocation == null
                ? "Unassigned"
                : $"Assigned to {AssignedLocation} ({AssignedShift})";
            return $"{EmployeeId} - {Name} ({Role}) - {assignment}";
        }
    }

}
