using System.Collections.Generic;
using CustomsSim.Inspection;
using CustomsSim.Scenario;
using NUnit.Framework;
using UnityEngine;

namespace CustomsSim.Tests.EditMode
{
    public sealed class DeclarationValidatorTests
    {
        private ScoringRules _rules;

        [SetUp]
        public void SetUp()
        {
            _rules = ScriptableObject.CreateInstance<ScoringRules>();
            _rules.CorrectnessWeight = 0.6f;
            _rules.ProceduralWeight = 0.3f;
            _rules.SpeedWeight = 0.1f;
            _rules.MaxSpeedBonus = 20f;
            _rules.FailedCorrectnessCap = 50f;
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_rules);
        }

        [Test]
        public void CorrectDecision_WithCompletedChecklist_ScoresNearMax()
        {
            var session = new InspectionSession("scen-1", new[] { "a", "b" });
            session.Checklist.Complete("a");
            session.Checklist.Complete("b");
            session.Submit(Decision.Seize);

            var truth = new ScenarioGroundTruth
            {
                CorrectDecision = Decision.Seize,
                TargetSeconds = 999f,
                SealsExpectedToBreak = 0,
                ExpectedFraudPatterns = new List<FraudPattern> { FraudPattern.ProhibitedGoods }
            };

            var result = DeclarationValidator.Evaluate(session, truth, _rules);

            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(100f, result.Correctness);
            Assert.AreEqual(100f, result.Procedural);
            Assert.GreaterOrEqual(result.Total, 90f);
        }

        [Test]
        public void WrongDecision_IsCappedByFailedCorrectnessCap()
        {
            var session = new InspectionSession("scen-1", new[] { "a" });
            session.Checklist.Complete("a");
            session.Submit(Decision.Release);

            var truth = new ScenarioGroundTruth
            {
                CorrectDecision = Decision.Seize,
                TargetSeconds = 999f,
                SealsExpectedToBreak = 0
            };

            var result = DeclarationValidator.Evaluate(session, truth, _rules);

            Assert.LessOrEqual(result.Total, _rules.FailedCorrectnessCap);
        }

        [Test]
        public void UnjustifiedSealBreak_PenalisesProceduralScore()
        {
            var session = new InspectionSession("scen-1", new[] { "a" });
            session.Checklist.Complete("a");
            session.MarkSealBroken("S1");
            session.MarkSealBroken("S2");
            session.Submit(Decision.Release);

            var truth = new ScenarioGroundTruth
            {
                CorrectDecision = Decision.Release,
                TargetSeconds = 999f,
                SealsExpectedToBreak = 0
            };

            var result = DeclarationValidator.Evaluate(session, truth, _rules);

            Assert.Less(result.Procedural, 100f);
        }

        [Test]
        public void MissedFraudPatterns_AreReported()
        {
            var session = new InspectionSession("scen-1", new[] { "a" });
            session.Checklist.Complete("a");
            session.Submit(Decision.HoldForReview);

            var truth = new ScenarioGroundTruth
            {
                CorrectDecision = Decision.HoldForReview,
                TargetSeconds = 999f,
                SealsExpectedToBreak = 0,
                ExpectedFraudPatterns = new List<FraudPattern> { FraudPattern.Undervaluation, FraudPattern.Misclassification }
            };

            var result = DeclarationValidator.Evaluate(session, truth, _rules);

            Assert.AreEqual(2, result.MissedFraudPatterns.Count);
            Assert.Contains(FraudPattern.Undervaluation, (System.Collections.ICollection)result.MissedFraudPatterns);
        }

        [Test]
        public void NullInputs_ReturnInvalidResult()
        {
            var result = DeclarationValidator.Evaluate(null, null, null);
            Assert.IsFalse(result.IsValid);
            Assert.IsNotNull(result.Error);
        }
    }
}
