using System.Collections.Generic;
using CustomsSim.Core;
using CustomsSim.Inspection;
using UnityEngine;

namespace CustomsSim.Player
{
    /// <summary>Tracks documents and tools the trainee has picked up during a shift.</summary>
    public sealed class InventoryComponent : MonoBehaviour
    {
        private readonly List<Document> _documents = new();
        private readonly HashSet<string> _tools = new();

        public IReadOnlyList<Document> Documents => _documents;
        public IReadOnlyCollection<string> Tools => _tools;

        public void AddDocument(Document doc)
        {
            if (doc == null) return;
            _documents.Add(doc);
            EventBus.Raise(new DocumentPickedUp(doc));
        }

        public bool AddTool(string toolId)
        {
            if (string.IsNullOrWhiteSpace(toolId)) return false;
            return _tools.Add(toolId);
        }

        public bool HasTool(string toolId) => !string.IsNullOrEmpty(toolId) && _tools.Contains(toolId);

        public void Clear()
        {
            _documents.Clear();
            _tools.Clear();
        }
    }

    public readonly struct DocumentPickedUp
    {
        public readonly Document Document;
        public DocumentPickedUp(Document document) => Document = document;
    }
}
