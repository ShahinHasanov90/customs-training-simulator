using CustomsSim.Core;
using UnityEngine;

namespace CustomsSim.Inspection
{
    /// <summary>A tamper-evident seal. Can be scanned non-destructively or broken.</summary>
    public sealed class Seal : MonoBehaviour, IInteractable
    {
        [SerializeField] private string sealId = "SEAL-0000";
        [SerializeField] private bool tamperedOnArrival;
        [SerializeField] private bool isBroken;

        public string SealId => sealId;
        public bool IsBroken => isBroken;
        public bool TamperedOnArrival => tamperedOnArrival;

        public string DisplayName => $"Seal {sealId}";
        public bool CanInteract => !isBroken;

        public void Interact()
        {
            Scan();
        }

        public ScanResult Scan()
        {
            var result = new ScanResult(sealId, isBroken || tamperedOnArrival);
            EventBus.Raise(new SealScanned(this, result));
            return result;
        }

        public void Break(string reason)
        {
            if (isBroken) return;
            isBroken = true;
            EventBus.Raise(new SealBroken(this, reason));
        }
    }

    public readonly struct ScanResult
    {
        public readonly string SealId;
        public readonly bool TamperDetected;

        public ScanResult(string sealId, bool tamperDetected)
        {
            SealId = sealId;
            TamperDetected = tamperDetected;
        }
    }

    public readonly struct SealScanned
    {
        public readonly Seal Seal;
        public readonly ScanResult Result;

        public SealScanned(Seal seal, ScanResult result)
        {
            Seal = seal;
            Result = result;
        }
    }

    public readonly struct SealBroken
    {
        public readonly Seal Seal;
        public readonly string Reason;

        public SealBroken(Seal seal, string reason)
        {
            Seal = seal;
            Reason = reason;
        }
    }
}
