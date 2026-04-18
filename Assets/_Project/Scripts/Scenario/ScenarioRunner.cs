using System.Collections;
using CustomsSim.Core;
using CustomsSim.Inspection;
using UnityEngine;

namespace CustomsSim.Scenario
{
    /// <summary>Executes a ScenarioAsset step by step at runtime.</summary>
    public sealed class ScenarioRunner : MonoBehaviour
    {
        [SerializeField] private ScenarioAsset scenario;

        public InspectionSession CurrentSession { get; private set; }
        public bool IsRunning { get; private set; }

        public void Begin()
        {
            if (scenario == null)
            {
                Debug.LogWarning("ScenarioRunner: no ScenarioAsset assigned.");
                return;
            }

            CurrentSession = new InspectionSession(scenario.ScenarioId, scenario.ChecklistItems);
            IsRunning = true;
            EventBus.Raise(new ScenarioBegan(scenario));
            StartCoroutine(RunSteps());
        }

        public void SubmitDecision(Decision decision)
        {
            if (CurrentSession == null || !IsRunning) return;
            CurrentSession.Submit(decision);
            Finish();
        }

        private IEnumerator RunSteps()
        {
            foreach (var step in scenario.Steps)
            {
                if (!IsRunning || step == null) yield break;
                EventBus.Raise(new ScenarioStepEntered(step));

                switch (step.Kind)
                {
                    case ScenarioStep.StepKind.ShowBriefing:
                    case ScenarioStep.StepKind.SpawnContainer:
                    case ScenarioStep.StepKind.AdvanceTime:
                        if (step.HoldSeconds > 0f) yield return new WaitForSeconds(step.HoldSeconds);
                        break;
                    case ScenarioStep.StepKind.AwaitDecision:
                        while (IsRunning && CurrentSession.FinalDecision == Decision.Pending)
                        {
                            yield return null;
                        }
                        break;
                    case ScenarioStep.StepKind.AwaitChecklistItem:
                        while (IsRunning && !CurrentSession.Checklist.IsComplete(step.Reference))
                        {
                            yield return null;
                        }
                        break;
                }
            }

            if (IsRunning)
            {
                Finish();
            }
        }

        private void Finish()
        {
            IsRunning = false;
            var result = DeclarationValidator.Evaluate(CurrentSession, scenario.BuildGroundTruth(), scenario.ScoringRules);
            EventBus.Raise(new ScenarioFinished(scenario, result));
        }
    }

    public readonly struct ScenarioBegan
    {
        public readonly ScenarioAsset Scenario;
        public ScenarioBegan(ScenarioAsset scenario) => Scenario = scenario;
    }

    public readonly struct ScenarioStepEntered
    {
        public readonly ScenarioStep Step;
        public ScenarioStepEntered(ScenarioStep step) => Step = step;
    }

    public readonly struct ScenarioFinished
    {
        public readonly ScenarioAsset Scenario;
        public readonly ValidationResult Result;

        public ScenarioFinished(ScenarioAsset scenario, ValidationResult result)
        {
            Scenario = scenario;
            Result = result;
        }
    }
}
