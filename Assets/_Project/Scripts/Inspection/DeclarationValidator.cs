using System.Collections.Generic;
using System.Linq;
using CustomsSim.Scenario;

namespace CustomsSim.Inspection
{
    /// <summary>Compares a completed InspectionSession against the scenario ground truth.</summary>
    public static class DeclarationValidator
    {
        public static ValidationResult Evaluate(InspectionSession session, ScenarioGroundTruth truth, ScoringRules rules)
        {
            if (session == null) return ValidationResult.Invalid("session-null");
            if (truth == null) return ValidationResult.Invalid("truth-null");
            if (rules == null) return ValidationResult.Invalid("rules-null");

            var correctness = session.FinalDecision == truth.CorrectDecision ? 100f : 0f;

            var procedural = ComputeProceduralScore(session, truth);
            var elapsedSeconds = (float)session.Elapsed.TotalSeconds;
            var speedBonus = ComputeSpeedBonus(elapsedSeconds, truth.TargetSeconds, rules.MaxSpeedBonus);

            var weighted = correctness * rules.CorrectnessWeight
                           + procedural * rules.ProceduralWeight
                           + speedBonus * rules.SpeedWeight;

            if (correctness < 100f)
            {
                weighted = System.Math.Min(weighted, rules.FailedCorrectnessCap);
            }

            var missed = truth.ExpectedFraudPatterns
                .Where(p => !session.DiscoveredFraudPatterns.Contains(p.ToString()))
                .ToList();

            return new ValidationResult(
                correctness: correctness,
                procedural: procedural,
                speedBonus: speedBonus,
                total: System.Math.Clamp(weighted, 0f, 100f),
                missedFraudPatterns: missed);
        }

        private static float ComputeProceduralScore(InspectionSession session, ScenarioGroundTruth truth)
        {
            var score = 100f;
            if (!session.Checklist.AllComplete())
            {
                score -= (1f - session.Checklist.Progress) * 60f;
            }

            var unjustifiedBreaks = session.BrokenSeals.Count - truth.SealsExpectedToBreak;
            if (unjustifiedBreaks > 0)
            {
                score -= unjustifiedBreaks * 20f;
            }

            if (score < 0f) score = 0f;
            return score;
        }

        private static float ComputeSpeedBonus(float elapsed, float target, float max)
        {
            if (target <= 0f) return 0f;
            if (elapsed <= 0f) return max;
            if (elapsed >= target) return 0f;
            var ratio = 1f - (elapsed / target);
            return System.Math.Clamp(ratio * max, 0f, max);
        }
    }

    public sealed class ValidationResult
    {
        public float Correctness { get; }
        public float Procedural { get; }
        public float SpeedBonus { get; }
        public float Total { get; }
        public IReadOnlyList<FraudPattern> MissedFraudPatterns { get; }
        public bool IsValid { get; }
        public string Error { get; }

        public ValidationResult(float correctness, float procedural, float speedBonus, float total, IReadOnlyList<FraudPattern> missedFraudPatterns)
        {
            Correctness = correctness;
            Procedural = procedural;
            SpeedBonus = speedBonus;
            Total = total;
            MissedFraudPatterns = missedFraudPatterns;
            IsValid = true;
        }

        private ValidationResult(string error)
        {
            Error = error;
            IsValid = false;
            MissedFraudPatterns = System.Array.Empty<FraudPattern>();
        }

        public static ValidationResult Invalid(string error) => new(error);
    }

    public sealed class ScenarioGroundTruth
    {
        public Decision CorrectDecision { get; set; }
        public int SealsExpectedToBreak { get; set; }
        public float TargetSeconds { get; set; } = 120f;
        public List<FraudPattern> ExpectedFraudPatterns { get; set; } = new();
    }
}
