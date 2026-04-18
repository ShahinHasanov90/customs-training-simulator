using System.Collections.Generic;
using UnityEngine;

namespace CustomsSim.Inspection
{
    /// <summary>A customs declaration attached to a container.</summary>
    public sealed class Document : MonoBehaviour, IInteractable
    {
        [SerializeField] private string documentNumber = "DOC-0000";
        [SerializeField] private string consignee = "Unknown";
        [SerializeField] private string originCountry = "XX";
        [SerializeField] private string destinationCountry = "XX";
        [SerializeField] private List<DeclaredLine> declaredLines = new();

        public string DocumentNumber => documentNumber;
        public string Consignee => consignee;
        public string OriginCountry => originCountry;
        public string DestinationCountry => destinationCountry;
        public IReadOnlyList<DeclaredLine> DeclaredLines => declaredLines;

        public string DisplayName => $"Declaration {documentNumber}";
        public bool CanInteract => true;

        public void Interact()
        {
            Core.EventBus.Raise(new DocumentOpened(this));
        }

        public float TotalDeclaredValue()
        {
            var total = 0f;
            foreach (var line in declaredLines)
            {
                total += line.unitValue * line.quantity;
            }
            return total;
        }
    }

    [System.Serializable]
    public struct DeclaredLine
    {
        public string description;
        public string hsCode;
        public float unitValue;
        public int quantity;
    }

    public readonly struct DocumentOpened
    {
        public readonly Document Document;
        public DocumentOpened(Document document) => Document = document;
    }
}
