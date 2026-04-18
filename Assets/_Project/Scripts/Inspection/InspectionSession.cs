using System;
using System.Collections.Generic;
using CustomsSim.Scenario;

namespace CustomsSim.Inspection
{
    /// <summary>Stateful record of a single inspection run. Fed to DeclarationValidator at the end.</summary>
    public sealed class InspectionSession
    {
        public string ScenarioId { get; }
        public DateTime StartedAt { get; }
        public DateTime? FinishedAt { get; private set; }
        public InspectionChecklist Checklist { get; }
        public Decision FinalDecision { get; private set; } = Decision.Pending;
        public List<string> BrokenSeals { get; } = new();
        public List<string> ScannedDocuments { get; } = new();
        public List<string> DiscoveredFraudPatterns { get; } = new();

        public InspectionSession(string scenarioId, IReadOnlyList<string> checklistItems)
        {
            ScenarioId = scenarioId ?? throw new ArgumentNullException(nameof(scenarioId));
            StartedAt = DateTime.UtcNow;
            Checklist = new InspectionChecklist();
            if (checklistItems != null)
            {
                foreach (var item in checklistItems)
                {
                    Checklist.AddItem(item);
                }
            }
        }

        public TimeSpan Elapsed => (FinishedAt ?? DateTime.UtcNow) - StartedAt;

        public void Submit(Decision decision)
        {
            if (FinishedAt.HasValue) return;
            FinalDecision = decision;
            FinishedAt = DateTime.UtcNow;
        }

        public void MarkSealBroken(string sealId)
        {
            if (!string.IsNullOrWhiteSpace(sealId) && !BrokenSeals.Contains(sealId))
            {
                BrokenSeals.Add(sealId);
            }
        }

        public void MarkDocumentScanned(string documentId)
        {
            if (!string.IsNullOrWhiteSpace(documentId) && !ScannedDocuments.Contains(documentId))
            {
                ScannedDocuments.Add(documentId);
            }
        }

        public void NoteFraudPattern(FraudPattern pattern)
        {
            var key = pattern.ToString();
            if (!DiscoveredFraudPatterns.Contains(key))
            {
                DiscoveredFraudPatterns.Add(key);
            }
        }
    }

    public enum Decision
    {
        Pending,
        Release,
        HoldForReview,
        Seize
    }
}
