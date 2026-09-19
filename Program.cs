using HorizonParkSystem;
using System;

namespace HorizonParkSystem
{
    public class Program
    {
        // The one ParkSystem instance that holds all data for this session.
        private static ParkSystem park = new ParkSystem();

        public static void Main(string[] args)
        {
            SeedSampleData(); // Adds a couple of rides/staff so the menu isn't empty on first run.

            bool running = true;
            while (running)
            {
                PrintMenu();
                string? choice = Console.ReadLine();

                // Wrapping every operation in try/catch means any ParkException
                // (business rule failure) is shown as a clean message instead
                // of crashing the program.
                try
                {
                    switch (choice)
                    {
                        case "1":
                            RegisterVisitorMenu();
                            break;
                        case "2":
                            IssueTicketMenu();
                            break;
                        case "3":
                            ValidateRideAccessMenu();
                            break;
                        case "4":
                            CreateReservationMenu();
                            break;
                        case "5":
                            ManageRideMenu();
                            break;
                        case "6":
                            AssignStaffMenu();
                            break;
                        case "7":
                            CancelReservationMenu();
                            break;
                        case "8":
                            DeactivateTicketMenu();
                            break;
                        case "9":
                            ViewRideStatusMenu();
                            break;
                        case "10":
                            RegisterEmployeeMenu();
                            break;
                        case "0":
                            running = false;
                            Console.WriteLine("Shutting down Horizon Adventure Park system. Goodbye!");
                            break;
                        default:
                            Console.WriteLine("Invalid option. Please choose a number from the menu.");
                            break;
                    }
                }
                catch (ParkException ex)
                {
                    // Expected business rule failure -> friendly, specific message.
                    Console.WriteLine($"\nRESULT: FAILED\nReason: {ex.Message}\n");
                }
                catch (FormatException)
                {
                    // Happens if the user typed letters where a number was expected.
                    Console.WriteLine("\nInvalid input: please enter a number where required.\n");
                }
                catch (Exception ex)
                {
                    // Safety net for anything truly unexpected.
                    Console.WriteLine($"\nUnexpected error: {ex.Message}\n");
                }
            }
        }

        private static void PrintMenu()
        {
            Console.WriteLine("=== Horizon Adventure Park - Operations System ===");
            Console.WriteLine("1. Register Visitor");
            Console.WriteLine("2. Issue Ticket");
            Console.WriteLine("3. Validate Ride Access");
            Console.WriteLine("4. Create Reservation");
            Console.WriteLine("5. Manage Ride (Add / Update Status)");
            Console.WriteLine("6. Assign Staff");
            Console.WriteLine("7. Cancel Reservation");
            Console.WriteLine("8. Deactivate Ticket");
            Console.WriteLine("9. View Ride Occupancy/Status");
            Console.WriteLine("10. Register Employee");
            Console.WriteLine("0. Exit");
            Console.Write("Select an option: ");
        }

        // ---------- Small helper methods for reading console input ----------

        // Keeps re-asking until the staff member types something that
        // isn't blank. Used for any field that can't reasonably be empty
        // (names, ride names, time slots, etc.).
        private static string ReadRequiredString(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input))
                {
                    return input.Trim();
                }
                Console.WriteLine("This field can't be empty. Please enter a value.");
            }
        }

        // Keeps re-asking until the staff member enters a whole number
        // greater than 0. Used for fields like capacity/validity days
        // where 0 or a negative number would be nonsense.
        private static int ReadPositiveInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();
                if (int.TryParse(input, out int value) && value > 0)
                {
                    return value;
                }
                Console.WriteLine("Please enter a whole number greater than 0.");
            }
        }

        // Keeps re-asking until the staff member enters a whole number
        // within a realistic range (e.g. age 1-120, height 30-250cm).
        // Prevents nonsense values like a height of 2cm from being accepted.
        private static int ReadIntInRange(string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();
                if (int.TryParse(input, out int value) && value >= min && value <= max)
                {
                    return value;
                }
                Console.WriteLine($"Please enter a whole number between {min} and {max}.");
            }
        }

        // Like ReadIntInRange, but allows 0 as the floor - used for ride
        // minimums (e.g. a ride with no minimum age is a valid 0).
        private static int ReadNonNegativeInt(string prompt, int max)
        {
            return ReadIntInRange(prompt, 0, max);
        }

        // Keeps re-asking until the staff member enters a valid number
        // matching one of the listed enum options. Uses TryParse (not
        // Parse) so non-numeric input like "abc" is handled gracefully
        // with a re-prompt instead of throwing a FormatException.
        private static T ReadEnum<T>(string prompt) where T : struct, Enum
        {
            T[] options = Enum.GetValues<T>();

            Console.WriteLine(prompt);
            foreach (T value in options)
            {
                Console.WriteLine($"  {(int)(object)value}. {value}");
            }

            while (true)
            {
                Console.Write("Choose a number: ");
                string? input = Console.ReadLine();

                if (int.TryParse(input, out int index))
                {
                    foreach (T value in options)
                    {
                        if ((int)(object)value == index)
                        {
                            return value;
                        }
                    }
                }

                Console.WriteLine($"Invalid input. Please enter a number from 0 to {options.Length - 1}.");
            }
        }

        // ---------- Menu actions ----------

        private static void RegisterVisitorMenu()
        {
            string id = ReadRequiredString("Enter Visitor ID (e.g. V-1001): ");

            // Check for a duplicate ID immediately, before asking every
            // other question, so staff aren't stuck re-typing everything
            // only to be told at the very end that the ID is already used.
            if (park.VisitorExists(id))
            {
                throw new ParkException($"A visitor with ID '{id}' is already registered.");
            }

            string name = ReadRequiredString("Enter Visitor Name: ");
            int age = ReadIntInRange("Enter Age: ", 1, 120);
            int height = ReadIntInRange("Enter Height (cm): ", 30, 250);
            VisitorCategory category = ReadEnum<VisitorCategory>("Select Visitor Category:");

            bool hasAdult = false;
            if (category == VisitorCategory.StaffAccompaniedMinor || category == VisitorCategory.Child)
            {
                string answer = ReadRequiredString("Is an adult currently accompanying this visitor? (y/n): ");
                hasAdult = answer.Trim().ToLower() == "y";
            }

            Visitor visitor = park.RegisterVisitor(id, name, age, height, category, hasAdult);
            Console.WriteLine($"\nRESULT: SUCCESS\nVisitor registered: {visitor}\n");
        }

        private static void IssueTicketMenu()
        {
            string visitorId = ReadRequiredString("Enter Visitor ID: ");
            TicketType type = ReadEnum<TicketType>("Select Ticket Type:");
            int days = ReadPositiveInt("Enter validity period in days: ");

            Ticket ticket = park.IssueTicket(visitorId, type, days);
            Console.WriteLine($"\nRESULT: SUCCESS\nTicket issued: {ticket} - Price: {ticket.Price:C}\n");
        }

        private static void ValidateRideAccessMenu()
        {
            string visitorId = ReadRequiredString("Enter Visitor ID: ");
            string rideName = ReadRequiredString("Enter Ride Name: ");

            park.ValidateRideAccess(visitorId, rideName);
            Console.WriteLine($"\nRESULT: ACCESS GRANTED\nVisitor {visitorId} has been admitted to '{rideName}'.\n");
        }

        private static void CreateReservationMenu()
        {
            string visitorId = ReadRequiredString("Enter Visitor ID: ");
            string rideName = ReadRequiredString("Enter Ride Name: ");
            string timeSlot = ReadRequiredString("Enter Desired Time Slot (e.g. 14:00): ");

            Reservation reservation = park.CreateReservation(visitorId, rideName, timeSlot);
            Console.WriteLine($"\nRESULT: RESERVATION CONFIRMED\n{reservation}\n");
        }

        private static void CancelReservationMenu()
        {
            string reservationId = ReadRequiredString("Enter Reservation ID: ");
            park.CancelReservation(reservationId);
            Console.WriteLine($"\nRESULT: SUCCESS\nReservation {reservationId} has been cancelled.\n");
        }

        private static void DeactivateTicketMenu()
        {
            string ticketId = ReadRequiredString("Enter Ticket ID: ");
            park.DeactivateTicket(ticketId);
            Console.WriteLine($"\nRESULT: SUCCESS\nTicket {ticketId} has been deactivated.\n");
        }

        private static void ManageRideMenu()
        {
            Console.WriteLine("1. Add a new ride");
            Console.WriteLine("2. Update ride status");
            Console.WriteLine("3. Add a facility (non-ride staff location)");
            string subChoice = ReadRequiredString("Choose an option: ");

            if (subChoice == "1")
            {
                string name = ReadRequiredString("Enter Ride Name: ");
                RideCategory category = ReadEnum<RideCategory>("Select Ride Category:");
                int minAge = ReadNonNegativeInt("Enter Minimum Age: ", 120);
                int minHeight = ReadNonNegativeInt("Enter Minimum Height (cm): ", 250);
                string adultAnswer = ReadRequiredString("Does this ride require an accompanying adult? (y/n): ");
                bool requiresAdult = adultAnswer.Trim().ToLower() == "y";
                int capacity = ReadPositiveInt("Enter Maximum Capacity: ");

                Ride ride;
                switch (category)
                {
                    case RideCategory.Thrill:
                        ride = new ThrillRide(name, minAge, minHeight, requiresAdult, capacity);
                        break;
                    case RideCategory.Water:
                        string briefingAnswer = ReadRequiredString("Requires swim safety briefing? (y/n): ");
                        bool requiresBriefing = briefingAnswer.Trim().ToLower() == "y";
                        ride = new WaterRide(name, minAge, minHeight, requiresAdult, capacity, requiresBriefing);
                        break;
                    default: // Family
                        ride = new FamilyRide(name, minAge, minHeight, requiresAdult, capacity);
                        break;
                }

                park.AddRide(ride);
                Console.WriteLine($"\nRESULT: SUCCESS\nRide added: {ride}\n");
            }
            else if (subChoice == "2")
            {
                string name = ReadRequiredString("Enter Ride Name: ");
                RideStatus status = ReadEnum<RideStatus>("Select New Status:");
                park.UpdateRideStatus(name, status);
                Console.WriteLine($"\nRESULT: SUCCESS\n'{name}' status updated to {status}.\n");
            }
            else if (subChoice == "3")
            {
                string name = ReadRequiredString("Enter Facility Name (e.g. Ticket Booth): ");
                park.AddFacility(name);
                Console.WriteLine($"\nRESULT: SUCCESS\nFacility added: {name}\n");
            }
            else
            {
                Console.WriteLine("Invalid sub-option.");
            }
        }

        private static void ViewRideStatusMenu()
        {
            Console.WriteLine("\n--- Current Ride Status ---");
            foreach (Ride ride in park.GetAllRides())
            {
                Console.WriteLine(ride);
                // CurrentOccupancy above only reflects direct admissions.
                // Reservations are tracked separately per time slot, so
                // show them on their own line to give the full picture.
                Console.WriteLine($"  Reservations: {park.GetReservationSummaryForRide(ride.Name)}");
            }
            Console.WriteLine();
        }

        private static void RegisterEmployeeMenu()
        {
            string id = ReadRequiredString("Enter Employee ID (e.g. E-01): ");
            string name = ReadRequiredString("Enter Employee Name: ");
            EmployeeRole role = ReadEnum<EmployeeRole>("Select Employee Role:");

            Employee employee = park.RegisterEmployee(id, name, role);
            Console.WriteLine($"\nRESULT: SUCCESS\nEmployee registered: {employee}\n");
        }

        private static void AssignStaffMenu()
        {
            string employeeId = ReadRequiredString("Enter Employee ID: ");
            string location = ReadRequiredString("Enter Ride/Facility Name: ");
            string shift = ReadRequiredString("Enter Shift (e.g. Morning/Afternoon): ");

            park.AssignEmployee(employeeId, location, shift);
            Console.WriteLine($"\nRESULT: SUCCESS\nEmployee {employeeId} assigned to {location} ({shift} shift).\n");
        }

        // Adds a few starting rides, facilities, and an employee so the
        // system is usable right away.
        private static void SeedSampleData()
        {
            park.AddRide(new ThrillRide("Thunder Peak Coaster", 12, 140, false, 20));
            park.AddRide(new FamilyRide("Enchanted Carousel", 0, 0, false, 15));
            park.AddRide(new WaterRide("Splash Voyage", 6, 110, true, 10, true));

            park.AddFacility("Ticket Booth");
            park.AddFacility("Operations Office");

            park.RegisterEmployee("E-01", "Alex Rivera", EmployeeRole.RideOperator);
        }
    }

}
