using System.Text;
using CustomsSim.Core;
using CustomsSim.Scenario;
using TMPro;
using UnityEngine;

namespace CustomsSim.UI
{
    /// <summary>Post-run results panel, shown when the scenario finishes.</summary>
    public sealed class ResultsView : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private TMP_Text body;

        private void OnEnable()
        {
            EventBus.Subscribe<ScenarioFinished>(OnFinished);
            if (root != null) root.SetActive(false);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<ScenarioFinished>(OnFinished);
        }

        private void OnFinished(ScenarioFinished evt)
        {
            if (root != null) root.SetActive(true);
            if (body == null) return;

            var sb = new StringBuilder();
            sb.AppendLine($"Scenario: {evt.Scenario?.DisplayName ?? "(unknown)"}");
            if (evt.Result != null && evt.Result.IsValid)
            {
                sb.AppendLine($"Total: {evt.Result.Total:F1}");
                sb.AppendLine($"Correctness: {evt.Result.Correctness:F1}");
                sb.AppendLine($"Procedural: {evt.Result.Procedural:F1}");
                sb.AppendLine($"Speed bonus: {evt.Result.SpeedBonus:F1}");
                if (evt.Result.MissedFraudPatterns.Count > 0)
                {
                    sb.AppendLine("Missed patterns:");
                    foreach (var m in evt.Result.MissedFraudPatterns)
                    {
                        sb.AppendLine($" - {m}");
                    }
                }
            }
            else
            {
                sb.AppendLine($"Invalid result: {evt.Result?.Error}");
            }

            body.text = sb.ToString();
        }
    }
}
