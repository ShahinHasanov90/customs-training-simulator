using CustomsSim.Core;
using CustomsSim.Inspection;
using CustomsSim.Player;
using CustomsSim.Scenario;
using TMPro;
using UnityEngine;

namespace CustomsSim.UI
{
    /// <summary>Top-level HUD: shows the current container ID and hint text.</summary>
    public sealed class HudView : MonoBehaviour
    {
        [SerializeField] private TMP_Text containerLabel;
        [SerializeField] private TMP_Text hintLabel;
        [SerializeField] private TMP_Text scenarioLabel;

        private void OnEnable()
        {
            EventBus.Subscribe<ContainerOpened>(OnContainerOpened);
            EventBus.Subscribe<InteractableHoverChanged>(OnHover);
            EventBus.Subscribe<ScenarioBegan>(OnScenarioBegan);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<ContainerOpened>(OnContainerOpened);
            EventBus.Unsubscribe<InteractableHoverChanged>(OnHover);
            EventBus.Unsubscribe<ScenarioBegan>(OnScenarioBegan);
        }

        private void OnContainerOpened(ContainerOpened evt)
        {
            if (containerLabel != null)
            {
                containerLabel.text = evt.Container != null ? evt.Container.ContainerId : string.Empty;
            }
        }

        private void OnHover(InteractableHoverChanged evt)
        {
            if (hintLabel == null) return;
            hintLabel.text = evt.Target == null ? string.Empty : $"[E] {evt.Target.DisplayName}";
        }

        private void OnScenarioBegan(ScenarioBegan evt)
        {
            if (scenarioLabel != null && evt.Scenario != null)
            {
                scenarioLabel.text = evt.Scenario.DisplayName;
            }
        }
    }
}
