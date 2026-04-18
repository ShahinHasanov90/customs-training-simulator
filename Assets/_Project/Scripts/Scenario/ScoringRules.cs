using UnityEngine;

namespace CustomsSim.Scenario
{
    /// <summary>ScriptableObject that configures how a completed session is scored.</summary>
    [CreateAssetMenu(fileName = "ScoringRules", menuName = "CustomsSim/Scoring Rules", order = 20)]
    public sealed class ScoringRules : ScriptableObject
    {
        [Range(0f, 1f)] public float CorrectnessWeight = 0.6f;
        [Range(0f, 1f)] public float ProceduralWeight = 0.3f;
        [Range(0f, 1f)] public float SpeedWeight = 0.1f;
        [Min(0f)] public float MaxSpeedBonus = 20f;
        [Range(0f, 100f)] public float FailedCorrectnessCap = 50f;

        public bool WeightsSumIsSane()
        {
            var sum = CorrectnessWeight + ProceduralWeight + SpeedWeight;
            return sum >= 0.99f && sum <= 1.01f;
        }
    }
}
