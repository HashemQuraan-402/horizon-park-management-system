using System;
using System.Collections.Generic;
using System.Text;

namespace HorizonParkSystem
{
    // A custom exception used for any "expected" business rule failure
    // (visitor not found, ride full, invalid ticket, etc).
    // We use this instead of a generic Exception so Program.cs can catch it
    // and print a clean, specific message to the staff member.
    public class ParkException : Exception
    {
        public ParkException(string message) : base(message)
        {
        }
    }

}
