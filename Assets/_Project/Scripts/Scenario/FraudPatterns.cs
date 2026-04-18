using System;
using System.Collections.Generic;

namespace CustomsSim.Scenario
{
    /// <summary>Enumeration of fraud patterns the simulator can generate.</summary>
    public enum FraudPattern
    {
        None,
        Undervaluation,
        Misclassification,
        ProhibitedGoods,
        OriginMisdeclaration,
        FalseCompartment,
        DocumentForgery
    }

    /// <summary>Static rule definitions used by validator heuristics and UI copy.</summary>
    public static class FraudPatternRules
    {
        private static readonly IReadOnlyDictionary<FraudPattern, string> Descriptions = new Dictionary<FraudPattern, string>
        {
            { FraudPattern.None, "No irregularity expected." },
            { FraudPattern.Undervaluation, "Declared value is materially below market reference." },
            { FraudPattern.Misclassification, "HS code does not match the physical goods." },
            { FraudPattern.ProhibitedGoods, "Shipment contains goods prohibited or restricted for import." },
            { FraudPattern.OriginMisdeclaration, "Declared country of origin is inconsistent with packaging or markings." },
            { FraudPattern.FalseCompartment, "Physical container contains a concealed compartment." },
            { FraudPattern.DocumentForgery, "One or more supporting documents show signs of forgery." }
        };

        public static string Describe(FraudPattern pattern)
        {
            return Descriptions.TryGetValue(pattern, out var text) ? text : "Unknown pattern.";
        }

        public static Inspection.Decision RecommendedDecision(FraudPattern pattern)
        {
            return pattern switch
            {
                FraudPattern.None => Inspection.Decision.Release,
                FraudPattern.Undervaluation => Inspection.Decision.HoldForReview,
                FraudPattern.Misclassification => Inspection.Decision.HoldForReview,
                FraudPattern.OriginMisdeclaration => Inspection.Decision.HoldForReview,
                FraudPattern.DocumentForgery => Inspection.Decision.Seize,
                FraudPattern.ProhibitedGoods => Inspection.Decision.Seize,
                FraudPattern.FalseCompartment => Inspection.Decision.Seize,
                _ => throw new ArgumentOutOfRangeException(nameof(pattern), pattern, null)
            };
        }
    }
}
