using System.Text;
using CustomsSim.Core;
using CustomsSim.Inspection;
using TMPro;
using UnityEngine;

namespace CustomsSim.UI
{
    /// <summary>Popup that renders the declaration document when the player opens it.</summary>
    public sealed class DocumentPopup : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private TMP_Text body;

        private void OnEnable()
        {
            EventBus.Subscribe<DocumentOpened>(OnOpened);
            if (root != null) root.SetActive(false);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<DocumentOpened>(OnOpened);
        }

        public void Close()
        {
            if (root != null) root.SetActive(false);
        }

        private void OnOpened(DocumentOpened evt)
        {
            var doc = evt.Document;
            if (doc == null || body == null) return;

            var sb = new StringBuilder();
            sb.AppendLine($"Doc: {doc.DocumentNumber}");
            sb.AppendLine($"Consignee: {doc.Consignee}");
            sb.AppendLine($"Origin: {doc.OriginCountry} -> {doc.DestinationCountry}");
            sb.AppendLine();
            foreach (var line in doc.DeclaredLines)
            {
                sb.AppendLine($"{line.hsCode}  {line.description}  x{line.quantity}  @{line.unitValue:F2}");
            }
            sb.AppendLine();
            sb.AppendLine($"Total declared: {doc.TotalDeclaredValue():F2}");

            body.text = sb.ToString();
            if (root != null) root.SetActive(true);
        }
    }
}
