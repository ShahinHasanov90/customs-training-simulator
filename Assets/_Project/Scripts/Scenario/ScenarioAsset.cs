using System.Collections.Generic;
using CustomsSim.Inspection;
using UnityEngine;

namespace CustomsSim.Scenario
{
    /// <summary>Top-level scenario definition. Holds steps, scoring, and ground truth.</summary>
    [CreateAssetMenu(fileName = "ScenarioAsset", menuName = "CustomsSim/Scenario", order = 10)]
    public sealed class ScenarioAsset : ScriptableObject
    {
        public string ScenarioId;
        public string DisplayName;
        [TextArea(3, 8)] public string Briefing;

        public List<ScenarioStep> Steps = new();
        public List<string> ChecklistItems = new();
        public ScoringRules ScoringRules;

        public Decision CorrectDecision = Decision.Release;
        public int SealsExpectedToBreak;
        [Min(10f)] public float TargetSeconds = 120f;
        public List<FraudPattern> ExpectedFraudPatterns = new();

        public ScenarioGroundTruth BuildGroundTruth()
        {
            return new ScenarioGroundTruth
            {
                CorrectDecision = CorrectDecision,
                SealsExpectedToBreak = SealsExpectedToBreak,
                TargetSeconds = TargetSeconds,
                ExpectedFraudPatterns = new List<FraudPattern>(ExpectedFraudPatterns)
            };
        }
    }
}
