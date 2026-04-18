using CustomsSim.Scenario;
using NUnit.Framework;
using UnityEngine;

namespace CustomsSim.Tests.EditMode
{
    public sealed class ScoringRulesTests
    {
        private ScoringRules _rules;

        [SetUp]
        public void SetUp()
        {
            _rules = ScriptableObject.CreateInstance<ScoringRules>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_rules);
        }

        [Test]
        public void Defaults_SumToOne()
        {
            Assert.IsTrue(_rules.WeightsSumIsSane(), "Default weights should sum to ~1.0");
        }

        [Test]
        public void UnbalancedWeights_FailSanityCheck()
        {
            _rules.CorrectnessWeight = 0.9f;
            _rules.ProceduralWeight = 0.9f;
            _rules.SpeedWeight = 0.9f;
            Assert.IsFalse(_rules.WeightsSumIsSane());
        }

        [Test]
        public void FailedCorrectnessCap_IsApplied()
        {
            _rules.FailedCorrectnessCap = 42f;
            Assert.AreEqual(42f, _rules.FailedCorrectnessCap);
        }
    }
}
