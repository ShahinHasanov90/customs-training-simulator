using UnityEngine;

namespace CustomsSim.Scenario
{
    /// <summary>A single scripted moment inside a scenario. Authored as an asset.</summary>
    [CreateAssetMenu(fileName = "ScenarioStep", menuName = "CustomsSim/Scenario Step", order = 11)]
    public sealed class ScenarioStep : ScriptableObject
    {
        public enum StepKind
        {
            SpawnContainer,
            ShowBriefing,
            AwaitDecision,
            AwaitChecklistItem,
            AdvanceTime
        }

        public StepKind Kind = StepKind.SpawnContainer;
        public string Reference;
        public string DisplayText;
        [Min(0f)] public float HoldSeconds;
    }
}
