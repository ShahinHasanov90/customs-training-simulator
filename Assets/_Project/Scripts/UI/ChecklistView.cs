using System.Collections.Generic;
using System.Text;
using CustomsSim.Core;
using CustomsSim.Scenario;
using TMPro;
using UnityEngine;

namespace CustomsSim.UI
{
    /// <summary>Shows the procedural checklist next to the HUD.</summary>
    public sealed class ChecklistView : MonoBehaviour
    {
        [SerializeField] private TMP_Text body;

        private ScenarioRunner _runner;
        private readonly List<string> _items = new();

        private void OnEnable()
        {
            EventBus.Subscribe<ScenarioBegan>(OnScenarioBegan);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<ScenarioBegan>(OnScenarioBegan);
        }

        private void OnScenarioBegan(ScenarioBegan evt)
        {
            _runner = FindObjectOfType<ScenarioRunner>();
            _items.Clear();
            if (evt.Scenario != null)
            {
                _items.AddRange(evt.Scenario.ChecklistItems);
            }
            Refresh();
        }

        private void Update()
        {
            if (_runner != null && _runner.CurrentSession != null)
            {
                Refresh();
            }
        }

        private void Refresh()
        {
            if (body == null) return;
            var sb = new StringBuilder();
            var session = _runner?.CurrentSession;
            foreach (var key in _items)
            {
                var done = session != null && session.Checklist.IsComplete(key);
                sb.AppendLine(done ? $"[x] {key}" : $"[ ] {key}");
            }
            body.text = sb.ToString();
        }
    }
}
