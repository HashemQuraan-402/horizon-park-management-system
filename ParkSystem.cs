using System;
using System.Collections.Generic;
using System.Text;

namespace HorizonParkSystem
{
    // This class holds all the park's data in memory using plain ARRAYS
    // (not List<> or Dictionary<>, since those were not covered in the course).
    // Each array starts small and grows using the generic Grow<T>() helper
    // below whenever it becomes full - this is where we apply the Generics
    // lesson (a single reusable method that works for any array type).
    public class ParkSystem
    {
        private Visitor[] visitors = new Visitor[5];
        private int visitorCount = 0;

        private Ticket[] tickets = new Ticket[5];
        private int ticketCount = 0;

        private Ride[] rides = new Ride[5];
        private int rideCount = 0;

        private Reservation[] reservations = new Reservation[5];
        private int reservationCount = 0;

        private Employee[] employees = new Employee[5];
        private int employeeCount = 0;

        // Non-ride locations staff can be assigned to (Ticket Booth,
        // Operations Office, etc). The assignment requires employees to be
        // assignable to "a ride or facility", so this exists alongside
        // the rides array rather than forcing every location to be a ride.
        private string[] facilities = new string[5];
        private int facilityCount = 0;

        // Simple counters used to auto-generate IDs.
        private int nextTicketNumber = 1;
        private int nextReservationNumber = 1;

        // ---------- Generic helper methods (Generics topic) ----------

        // Doubles the size of any array once it becomes full, copying the
        // existing items across. Works for Visitor[], Ticket[], Ride[], etc.
        private static T[] Grow<T>(T[] array)
        {
            T[] biggerArray = new T[array.Length * 2];
            for (int i = 0; i < array.Length; i++)
            {
                biggerArray[i] = array[i];
            }
            return biggerArray;
        }

        // Returns a new array containing only the first "count" real items,
        // used when handing data back out to the menu (Program.cs) so there
        // are no empty/null slots at the end.
        private static T[] Trim<T>(T[] array, int count)
        {
            T[] result = new T[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = array[i];
            }
            return result;
        }

        // ---------- Ticket pricing (Ticket Pricing rule) ----------
        // A simple switch instead of a Dictionary lookup table.
        private static decimal GetPrice(TicketType type)
        {
            switch (type)
            {
                case TicketType.GeneralAdmission:
                    return 49.99m;
                case TicketType.VIP:
                    return 129.99m;
                case TicketType.Child:
                    return 29.99m;
                case TicketType.Senior:
                    return 34.99m;
                default:
                    return 0m;
            }
        }

        // =========================================================
        // VISITOR & TICKETING
        // =========================================================

        // Lets the menu check for a duplicate ID right after it's typed,
        // before asking the rest of the registration questions.
        // Case-insensitive, so "V-1001" and "v-1001" count as the same ID.
        public bool VisitorExists(string visitorId)
        {
            for (int i = 0; i < visitorCount; i++)
            {
                if (visitors[i].VisitorId.Equals(visitorId, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        // Business rule: the visitor's age must be realistic for the
        // category chosen (e.g. an 8-year-old can't be registered as a
        // Senior). VIP has no age restriction since it's a ticket tier,
        // not a life-stage.
        private void ValidateAgeMatchesCategory(int age, VisitorCategory category)
        {
            switch (category)
            {
                case VisitorCategory.Child:
                    if (age >= 18)
                    {
                        throw new ParkException(
                            $"Age {age} does not match category 'Child' (expected under 18).");
                    }
                    break;
                case VisitorCategory.StaffAccompaniedMinor:
                    if (age >= 18)
                    {
                        throw new ParkException(
                            $"Age {age} does not match category 'StaffAccompaniedMinor' (expected under 18).");
                    }
                    break;
                case VisitorCategory.Senior:
                    if (age < 60)
                    {
                        throw new ParkException(
                            $"Age {age} does not match category 'Senior' (expected 60 or older).");
                    }
                    break;
                case VisitorCategory.General:
                    if (age < 18 || age > 59)
                    {
                        throw new ParkException(
                            $"Age {age} does not match category 'General' (expected 18 to 59).");
                    }
                    break;
                case VisitorCategory.VIP:
                    // No age restriction for VIP.
                    break;
            }
        }

        public Visitor RegisterVisitor(string visitorId, string name, int age, int heightCm,
            VisitorCategory category, bool hasAccompanyingAdult)
        {
            // Duplicate check - loop through the array manually (case-insensitive).
            if (VisitorExists(visitorId))
            {
                throw new ParkException($"A visitor with ID '{visitorId}' is already registered.");
            }

            // Age must be realistic for the chosen category.
            ValidateAgeMatchesCategory(age, category);

            if (visitorCount == visitors.Length)
            {
                visitors = Grow(visitors);
            }

            Visitor visitor = new Visitor(visitorId, name, age, heightCm, category, hasAccompanyingAdult);
            visitors[visitorCount] = visitor;
            visitorCount++;
            return visitor;
        }

        public Ticket IssueTicket(string visitorId, TicketType requestedType, int validDays)
        {
            Visitor visitor = FindVisitor(visitorId);

            // Ticket category must be consistent with the visitor's category.
            if (!IsTicketAllowedForCategory(requestedType, visitor.Category))
            {
                throw new ParkException(
                    $"Ticket type '{requestedType}' is not valid for a visitor in category '{visitor.Category}'.");
            }

            string ticketId = "T-" + nextTicketNumber.ToString("D4");
            nextTicketNumber++;

            decimal price = GetPrice(requestedType);
            DateTime issueDate = DateTime.Now;
            DateTime expiryDate = issueDate.AddDays(validDays);

            Ticket ticket = new Ticket(ticketId, visitorId, requestedType, issueDate, expiryDate, price);

            if (ticketCount == tickets.Length)
            {
                tickets = Grow(tickets);
            }
            tickets[ticketCount] = ticket;
            ticketCount++;

            return ticket;
        }

        // Encodes which ticket types make sense for which visitor category.
        private bool IsTicketAllowedForCategory(TicketType type, VisitorCategory category)
        {
            switch (category)
            {
                case VisitorCategory.VIP:
                    return type == TicketType.VIP;
                case VisitorCategory.Child:
                case VisitorCategory.StaffAccompaniedMinor:
                    return type == TicketType.Child || type == TicketType.VIP;
                case VisitorCategory.Senior:
                    return type == TicketType.Senior || type == TicketType.VIP;
                case VisitorCategory.General:
                    return type == TicketType.GeneralAdmission || type == TicketType.VIP;
                default:
                    return false;
            }
        }

        // Returns the most recent ticket belonging to a visitor, or null if none.
        public Ticket? GetLatestTicket(string visitorId)
        {
            Ticket? latest = null;
            for (int i = 0; i < ticketCount; i++)
            {
                if (tickets[i].VisitorId.Equals(visitorId, StringComparison.OrdinalIgnoreCase))
                {
                    if (latest == null || tickets[i].IssueDate > latest.IssueDate)
                    {
                        latest = tickets[i];
                    }
                }
            }
            return latest;
        }

        // Validates that a visitor currently holds a usable ticket.
        // Throws ParkException with a specific reason if not.
        public Ticket ValidateTicket(string visitorId)
        {
            FindVisitor(visitorId); // Confirms the visitor exists first.

            Ticket? ticket = GetLatestTicket(visitorId);
            if (ticket == null)
            {
                throw new ParkException("Visitor has no ticket on record.");
            }

            if (ticket.Status == TicketStatus.Cancelled)
            {
                throw new ParkException("Visitor's ticket has been cancelled.");
            }

            if (!ticket.IsValid())
            {
                throw new ParkException("Visitor's ticket has expired.");
            }

            return ticket;
        }

        public void DeactivateTicket(string ticketId)
        {
            Ticket ticket = FindTicket(ticketId);
            ticket.Status = TicketStatus.Cancelled;
        }

        // =========================================================
        // RIDE ACCESS
        // =========================================================

        // Full access check combining: ticket validity, ticket access tier,
        // ride status, and the ride's own eligibility rules.
        // Throws ParkException with the specific reason on failure.
        public void ValidateRideAccess(string visitorId, string rideName)
        {
            Visitor visitor = FindVisitor(visitorId);
            Ride ride = FindRide(rideName);

            // 1. Ticket must be valid.
            Ticket ticket = ValidateTicket(visitorId);

            // 2. Ride must be open.
            if (ride.Status != RideStatus.Open)
            {
                throw new ParkException($"'{ride.Name}' is currently {ride.Status} and not accepting visitors.");
            }

            // 3. Ticket tier must cover this ride's category.
            if (!ticket.AllowsAccessTo(ride.Category))
            {
                throw new ParkException(
                    $"Visitor's ticket ({ticket.Type}) does not include access to {ride.Category} rides.");
            }

            // 4. Ride-specific eligibility rules (age, height, adult, etc.).
            string? eligibilityFailure = ride.CheckEligibility(visitor);
            if (eligibilityFailure != null)
            {
                throw new ParkException(eligibilityFailure);
            }

            // 5. Capacity check for direct admission. CurrentOccupancy only
            // counts people admitted on the spot - it does NOT include
            // reservations, which are tracked separately per time slot
            // (see CreateReservation / CountActiveReservationsForSlot).
            // Both are capped at the same MaxCapacity value, but as two
            // independent counters rather than one merged total.
            if (!ride.HasAvailableCapacity())
            {
                throw new ParkException($"'{ride.Name}' is at maximum capacity ({ride.MaxCapacity}).");
            }

            // Everything passed -> admit the visitor.
            ride.IncreaseOccupancy();
        }

        // =========================================================
        // RESERVATIONS
        // =========================================================

        public Reservation CreateReservation(string visitorId, string rideName, string timeSlot)
        {
            Visitor visitor = FindVisitor(visitorId);
            Ride ride = FindRide(rideName);
            Ticket ticket = ValidateTicket(visitorId);

            if (ride.Status != RideStatus.Open)
            {
                throw new ParkException($"'{ride.Name}' is currently {ride.Status} and not accepting reservations.");
            }

            if (!ticket.AllowsAccessTo(ride.Category))
            {
                throw new ParkException(
                    $"Visitor's ticket ({ticket.Type}) does not include access to {ride.Category} rides.");
            }

            string? eligibilityFailure = ride.CheckEligibility(visitor);
            if (eligibilityFailure != null)
            {
                throw new ParkException(eligibilityFailure);
            }

            // Duplicate prevention: same visitor can't double-book the same ride+slot.
            for (int i = 0; i < reservationCount; i++)
            {
                Reservation existing = reservations[i];
                if (existing.VisitorId.Equals(visitorId, StringComparison.OrdinalIgnoreCase) &&
                    existing.RideName.Equals(rideName, StringComparison.OrdinalIgnoreCase) &&
                    existing.TimeSlot == timeSlot && existing.Status == ReservationStatus.Active)
                {
                    throw new ParkException("Visitor already has an active reservation for this ride and time slot.");
                }
            }

            // Capacity check: reservations are checked against the ride's
            // capacity FOR THE SELECTED TIME SLOT specifically (Phase 1
            // requirement 6 and the Phase 6 sample both describe capacity
            // this way - "maximum capacity for the selected time slot").
            // Every slot is independently capped at the SAME declared
            // MaxCapacity - there's only ever one capacity number per ride,
            // it's just counted per period rather than as one giant total,
            // since a reservation for 15:00 and one for 14:00 don't
            // physically compete for the same seats.
            int activeReservationsForSlot = CountActiveReservationsForSlot(rideName, timeSlot);
            if (activeReservationsForSlot >= ride.MaxCapacity)
            {
                throw new ParkException("Ride has reached maximum capacity for the selected time slot.");
            }

            string reservationId = "R-" + nextReservationNumber.ToString("D4");
            nextReservationNumber++;

            Reservation reservation = new Reservation(reservationId, visitorId, rideName, timeSlot);

            if (reservationCount == reservations.Length)
            {
                reservations = Grow(reservations);
            }
            reservations[reservationCount] = reservation;
            reservationCount++;

            return reservation;
        }

        // Counts how many ACTIVE reservations already exist for a specific
        // ride + time slot combination. This is what makes each time slot's
        // capacity independent of every other slot.
        private int CountActiveReservationsForSlot(string rideName, string timeSlot)
        {
            int count = 0;
            for (int i = 0; i < reservationCount; i++)
            {
                Reservation r = reservations[i];
                if (r.RideName.Equals(rideName, StringComparison.OrdinalIgnoreCase) &&
                    r.TimeSlot == timeSlot && r.Status == ReservationStatus.Active)
                {
                    count++;
                }
            }
            return count;
        }

        public void CancelReservation(string reservationId)
        {
            Reservation reservation = FindReservation(reservationId);

            if (reservation.Status == ReservationStatus.Cancelled)
            {
                throw new ParkException("This reservation has already been cancelled.");
            }

            reservation.Status = ReservationStatus.Cancelled;

            // No occupancy adjustment needed here - a cancelled reservation
            // simply stops counting in CountActiveReservationsForSlot(),
            // which automatically frees up a spot in that slot for the
            // next reservation.
        }

        // =========================================================
        // RIDE & FACILITY MANAGEMENT
        // =========================================================

        public void AddRide(Ride ride)
        {
            for (int i = 0; i < rideCount; i++)
            {
                if (rides[i].Name.Equals(ride.Name, StringComparison.OrdinalIgnoreCase))
                {
                    throw new ParkException($"A ride named '{ride.Name}' already exists.");
                }
            }

            if (rideCount == rides.Length)
            {
                rides = Grow(rides);
            }
            rides[rideCount] = ride;
            rideCount++;
        }

        public void UpdateRideStatus(string rideName, RideStatus newStatus)
        {
            Ride ride = FindRide(rideName);
            ride.Status = newStatus;
        }

        // =========================================================
        // STAFF MANAGEMENT
        // =========================================================

        public Employee RegisterEmployee(string employeeId, string name, EmployeeRole role)
        {
            for (int i = 0; i < employeeCount; i++)
            {
                if (employees[i].EmployeeId.Equals(employeeId, StringComparison.OrdinalIgnoreCase))
                {
                    throw new ParkException($"An employee with ID '{employeeId}' already exists.");
                }
            }

            if (employeeCount == employees.Length)
            {
                employees = Grow(employees);
            }

            Employee employee = new Employee(employeeId, name, role);
            employees[employeeCount] = employee;
            employeeCount++;
            return employee;
        }

        // =========================================================
        // FACILITIES (non-ride staff locations)
        // =========================================================

        public void AddFacility(string name)
        {
            if (FacilityExists(name))
            {
                throw new ParkException($"A facility named '{name}' already exists.");
            }

            if (facilityCount == facilities.Length)
            {
                facilities = Grow(facilities);
            }
            facilities[facilityCount] = name;
            facilityCount++;
        }

        public bool FacilityExists(string name)
        {
            for (int i = 0; i < facilityCount; i++)
            {
                if (facilities[i].Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        // True if a ride with this name exists, without throwing - used so
        // AssignEmployee can check "is this a ride OR a facility?" cleanly.
        public bool RideExists(string rideName)
        {
            for (int i = 0; i < rideCount; i++)
            {
                if (rides[i].Name.Equals(rideName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public string[] GetAllFacilities() => Trim(facilities, facilityCount);

        public void AssignEmployee(string employeeId, string location, string shift)
        {
            Employee employee = FindEmployee(employeeId);

            // The location must be an existing ride OR an existing
            // facility - staff can be assigned to either, per the
            // assignment's "ride or facility" requirement.
            if (!RideExists(location) && !FacilityExists(location))
            {
                throw new ParkException($"No ride or facility found named '{location}'.");
            }

            // Conflict rule: can't assign an employee to a new location during
            // a shift they are already working somewhere else.
            if (employee.AssignedLocation != null && employee.AssignedShift == shift &&
                employee.AssignedLocation != location)
            {
                throw new ParkException(
                    $"{employee.Name} is already assigned to '{employee.AssignedLocation}' during the '{shift}' shift.");
            }

            employee.AssignedLocation = location;
            employee.AssignedShift = shift;
        }

        // =========================================================
        // LOOKUPS / REPORTING (used by the menu to find & list things)
        // =========================================================

        public Visitor FindVisitor(string visitorId)
        {
            for (int i = 0; i < visitorCount; i++)
            {
                if (visitors[i].VisitorId.Equals(visitorId, StringComparison.OrdinalIgnoreCase))
                {
                    return visitors[i];
                }
            }
            throw new ParkException($"No visitor found with ID '{visitorId}'.");
        }

        public Ticket FindTicket(string ticketId)
        {
            for (int i = 0; i < ticketCount; i++)
            {
                if (tickets[i].TicketId.Equals(ticketId, StringComparison.OrdinalIgnoreCase))
                {
                    return tickets[i];
                }
            }
            throw new ParkException($"No ticket found with ID '{ticketId}'.");
        }

        public Ride FindRide(string rideName)
        {
            for (int i = 0; i < rideCount; i++)
            {
                if (rides[i].Name.Equals(rideName, StringComparison.OrdinalIgnoreCase))
                {
                    return rides[i];
                }
            }
            throw new ParkException($"No ride found named '{rideName}'.");
        }

        public Reservation FindReservation(string reservationId)
        {
            for (int i = 0; i < reservationCount; i++)
            {
                if (reservations[i].ReservationId.Equals(reservationId, StringComparison.OrdinalIgnoreCase))
                {
                    return reservations[i];
                }
            }
            throw new ParkException($"No reservation found with ID '{reservationId}'.");
        }

        public Employee FindEmployee(string employeeId)
        {
            for (int i = 0; i < employeeCount; i++)
            {
                if (employees[i].EmployeeId.Equals(employeeId, StringComparison.OrdinalIgnoreCase))
                {
                    return employees[i];
                }
            }
            throw new ParkException($"No employee found with ID '{employeeId}'.");
        }

        // These return plain arrays trimmed to the real number of items,
        // so Program.cs can just "foreach" over them like any other array.
        public Ride[] GetAllRides() => Trim(rides, rideCount);
        public Visitor[] GetAllVisitors() => Trim(visitors, visitorCount);
        public Employee[] GetAllEmployees() => Trim(employees, employeeCount);
        public Reservation[] GetAllReservations() => Trim(reservations, reservationCount);

        // Builds a one-line summary of active reservations for a ride,
        // broken down by time slot (e.g. "14:00: 3/10 reserved, 15:00:
        // 1/10 reserved"). This exists because direct-admission
        // CurrentOccupancy and slot reservations are tracked as two
        // separate counters (see the "Capacity model" note in README.md),
        // so Option 9 needs this to show the reservation side of the
        // picture, not just Ride.ToString()'s admission-only occupancy.
        public string GetReservationSummaryForRide(string rideName)
        {
            Ride ride = FindRide(rideName);

            // Collect distinct time slots and their active reservation
            // counts using two parallel arrays (grouping by hand, since
            // Dictionary<> isn't used in this project).
            string[] slots = new string[4];
            int[] slotCounts = new int[4];
            int slotTotal = 0;

            for (int i = 0; i < reservationCount; i++)
            {
                Reservation r = reservations[i];
                if (r.Status != ReservationStatus.Active ||
                    !r.RideName.Equals(rideName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                int foundIndex = -1;
                for (int j = 0; j < slotTotal; j++)
                {
                    if (slots[j] == r.TimeSlot)
                    {
                        foundIndex = j;
                        break;
                    }
                }

                if (foundIndex >= 0)
                {
                    slotCounts[foundIndex]++;
                }
                else
                {
                    if (slotTotal == slots.Length)
                    {
                        slots = Grow(slots);
                        slotCounts = Grow(slotCounts);
                    }
                    slots[slotTotal] = r.TimeSlot;
                    slotCounts[slotTotal] = 1;
                    slotTotal++;
                }
            }

            if (slotTotal == 0)
            {
                return "No active reservations.";
            }

            string summary = "";
            for (int i = 0; i < slotTotal; i++)
            {
                if (i > 0)
                {
                    summary += ", ";
                }
                summary += $"{slots[i]}: {slotCounts[i]}/{ride.MaxCapacity} reserved";
            }
            return summary;
        }
    }

}
