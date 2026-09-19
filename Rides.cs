using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace HorizonParkSystem
{

    // Abstract base class for every ride in the park.
    // It holds the data every ride shares, and forces every subtype to
    // provide its own eligibility rules through the abstract method below.
    public abstract class Ride
    {
        public string Name { get; set; }
        public int MinAge { get; set; }
        public int MinHeightCm { get; set; }
        public bool RequiresAccompanyingAdult { get; set; }
        public int MaxCapacity { get; set; }
        public RideStatus Status { get; set; }

        // Visitors currently admitted or holding an active reservation.
        public int CurrentOccupancy { get; protected set; }

        // Abstract property: every ride subtype must say what category it is.
        public abstract RideCategory Category { get; }

        protected Ride(string name, int minAge, int minHeightCm,
            bool requiresAccompanyingAdult, int maxCapacity)
        {
            Name = name;
            MinAge = minAge;
            MinHeightCm = minHeightCm;
            RequiresAccompanyingAdult = requiresAccompanyingAdult;
            MaxCapacity = maxCapacity;
            Status = RideStatus.Open;
            CurrentOccupancy = 0;
        }

        // Checks eligibility rules common to ALL rides (age, height, adult).
        // Returns null if eligible, or a specific reason string if not.
        // Marked virtual so a subtype could add extra rules later (polymorphism).
        public virtual string? CheckEligibility(Visitor visitor)
        {
            if (visitor.Age < MinAge)
            {
                return $"Visitor does not meet the minimum age requirement ({MinAge}) for this ride.";
            }

            if (visitor.HeightCm < MinHeightCm)
            {
                return $"Visitor does not meet the minimum height requirement ({MinHeightCm}cm) for this ride.";
            }

            if (RequiresAccompanyingAdult && !visitor.HasAccompanyingAdult)
            {
                return "This ride requires the visitor to be accompanied by an adult.";
            }

            return null; // No problems found -> eligible.
        }

        // Capacity check shared by both direct admission and reservations.
        public bool HasAvailableCapacity()
        {
            return CurrentOccupancy < MaxCapacity;
        }

        public void IncreaseOccupancy()
        {
            CurrentOccupancy++;
        }

        public void DecreaseOccupancy()
        {
            if (CurrentOccupancy > 0)
            {
                CurrentOccupancy--;
            }
        }

        public override string ToString()
        {
            return $"{Name} [{Category}] - {Status} - Occupancy {CurrentOccupancy}/{MaxCapacity} " +
                   $"(MinAge {MinAge}, MinHeight {MinHeightCm}cm, AdultRequired {RequiresAccompanyingAdult})";
        }
    }

    // Thrill rides: usually have age/height limits and no adult requirement.
    public class ThrillRide : Ride
    {
        public override RideCategory Category => RideCategory.Thrill;

        public ThrillRide(string name, int minAge, int minHeightCm, bool requiresAccompanyingAdult, int maxCapacity)
            : base(name, minAge, minHeightCm, requiresAccompanyingAdult, maxCapacity)
        {
        }
    }

    // Family rides: gentle rides, usually no strict age/height limit.
    public class FamilyRide : Ride
    {
        public override RideCategory Category => RideCategory.Family;

        public FamilyRide(string name, int minAge, int minHeightCm, bool requiresAdult, int maxCapacity)
            : base(name, minAge, minHeightCm, requiresAdult, maxCapacity)
        {
        }
    }

    // Water rides: override eligibility to add a water-specific rule
    // (demonstrates overriding a virtual method / polymorphism).
    public class WaterRide : Ride
    {
        public override RideCategory Category => RideCategory.Water;

        // Extra rule just for water rides.
        public bool RequiresSwimSafetyBriefing { get; set; }

        public WaterRide(string name, int minAge, int minHeightCm, bool requiresAdult,
            int maxCapacity, bool requiresSwimSafetyBriefing)
            : base(name, minAge, minHeightCm, requiresAdult, maxCapacity)
        {
            RequiresSwimSafetyBriefing = requiresSwimSafetyBriefing;
        }

        // Extends the base rules with the water-ride-specific one.
        public override string? CheckEligibility(Visitor visitor)
        {
            string? baseResult = base.CheckEligibility(visitor);
            if (baseResult != null)
            {
                return baseResult;
            }

            if (RequiresSwimSafetyBriefing && visitor.Age < 8)
            {
                return "Visitor must complete the swim safety briefing (minimum age 8) before this ride.";
            }

            return null;
        }
    }


}
