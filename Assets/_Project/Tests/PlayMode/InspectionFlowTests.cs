using System.Collections;
using CustomsSim.Inspection;
using CustomsSim.Scenario;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace CustomsSim.Tests.PlayMode
{
    public sealed class InspectionFlowTests
    {
        private ScoringRules _rules;
        private ScenarioAsset _scenario;
        private GameObject _runnerHost;

        [SetUp]
        public void SetUp()
        {
            _rules = ScriptableObject.CreateInstance<ScoringRules>();
            _rules.CorrectnessWeight = 0.6f;
            _rules.ProceduralWeight = 0.3f;
            _rules.SpeedWeight = 0.1f;
            _rules.MaxSpeedBonus = 20f;
            _rules.FailedCorrectnessCap = 50f;

            _scenario = ScriptableObject.CreateInstance<ScenarioAsset>();
            _scenario.ScenarioId = "flow-test";
            _scenario.DisplayName = "Flow Test";
            _scenario.ScoringRules = _rules;
            _scenario.CorrectDecision = Decision.HoldForReview;
            _scenario.TargetSeconds = 999f;
            _scenario.ChecklistItems.Add("scan");

            var waitStep = ScriptableObject.CreateInstance<ScenarioStep>();
            waitStep.Kind = ScenarioStep.StepKind.AwaitDecision;
            waitStep.Reference = "decision";
            _scenario.Steps.Add(waitStep);

            _runnerHost = new GameObject("RunnerHost");
        }

        [TearDown]
        public void TearDown()
        {
            if (_runnerHost != null) Object.Destroy(_runnerHost);
            Object.DestroyImmediate(_scenario);
            Object.DestroyImmediate(_rules);
        }

        [UnityTest]
        public IEnumerator ScenarioRunner_HoldDecision_MatchesCorrect()
        {
            var runner = _runnerHost.AddComponent<ScenarioRunner>();
            var scenarioField = typeof(ScenarioRunner).GetField("scenario",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            scenarioField.SetValue(runner, _scenario);

            ValidationResult captured = null;
            System.Action<ScenarioFinished> handler = evt => captured = evt.Result;
            Core.EventBus.Subscribe(handler);

            runner.Begin();
            runner.CurrentSession.Checklist.Complete("scan");
            runner.SubmitDecision(Decision.HoldForReview);

            yield return null;

            Core.EventBus.Unsubscribe(handler);
            Assert.IsNotNull(captured);
            Assert.IsTrue(captured.IsValid);
            Assert.AreEqual(100f, captured.Correctness);
        }

        [UnityTest]
        public IEnumerator ScenarioRunner_WrongDecision_FailsCorrectness()
        {
            var runner = _runnerHost.AddComponent<ScenarioRunner>();
            var scenarioField = typeof(ScenarioRunner).GetField("scenario",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            scenarioField.SetValue(runner, _scenario);

            ValidationResult captured = null;
            System.Action<ScenarioFinished> handler = evt => captured = evt.Result;
            Core.EventBus.Subscribe(handler);

            runner.Begin();
            runner.SubmitDecision(Decision.Release);

            yield return null;

            Core.EventBus.Unsubscribe(handler);
            Assert.IsNotNull(captured);
            Assert.AreEqual(0f, captured.Correctness);
            Assert.LessOrEqual(captured.Total, _rules.FailedCorrectnessCap);
        }
    }
}
