using CustomsSim.Inspection;
using NUnit.Framework;

namespace CustomsSim.Tests.EditMode
{
    public sealed class InspectionChecklistTests
    {
        [Test]
        public void AddItem_ThenComplete_MarksAsDone()
        {
            var list = new InspectionChecklist();
            list.AddItem("seal-check");
            Assert.IsFalse(list.IsComplete("seal-check"));
            Assert.IsTrue(list.Complete("seal-check"));
            Assert.IsTrue(list.IsComplete("seal-check"));
        }

        [Test]
        public void AllComplete_RequiresEveryItem()
        {
            var list = new InspectionChecklist();
            list.AddItem("a");
            list.AddItem("b");
            list.Complete("a");
            Assert.IsFalse(list.AllComplete());
            list.Complete("b");
            Assert.IsTrue(list.AllComplete());
        }

        [Test]
        public void Progress_ReflectsCompletedFraction()
        {
            var list = new InspectionChecklist();
            list.AddItem("a");
            list.AddItem("b");
            list.AddItem("c");
            list.AddItem("d");
            list.Complete("a");
            list.Complete("b");
            Assert.AreEqual(0.5f, list.Progress, 0.001f);
        }

        [Test]
        public void Complete_UnknownKey_ReturnsFalse()
        {
            var list = new InspectionChecklist();
            list.AddItem("known");
            Assert.IsFalse(list.Complete("unknown"));
        }
    }
}
