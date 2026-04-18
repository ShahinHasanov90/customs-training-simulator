using System.Collections.Generic;
using System.Linq;

namespace CustomsSim.Inspection
{
    /// <summary>A procedural checklist that the trainee is expected to tick off.</summary>
    public sealed class InspectionChecklist
    {
        private readonly Dictionary<string, bool> _items = new();
        private readonly List<string> _ordered = new();

        public IReadOnlyList<string> Items => _ordered;

        public void AddItem(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return;
            if (_items.ContainsKey(key)) return;
            _items[key] = false;
            _ordered.Add(key);
        }

        public bool Complete(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return false;
            if (!_items.ContainsKey(key)) return false;
            _items[key] = true;
            return true;
        }

        public bool IsComplete(string key) => _items.TryGetValue(key, out var done) && done;

        public bool AllComplete() => _items.Count > 0 && _items.Values.All(v => v);

        public float Progress => _items.Count == 0 ? 0f : (float)_items.Values.Count(v => v) / _items.Count;

        public void Reset()
        {
            foreach (var key in _items.Keys.ToList())
            {
                _items[key] = false;
            }
        }
    }
}
