using UnityEngine;

namespace _Project.Scripts.Interaction
{
    public sealed class Syringe : MonoBehaviour
    {
        public bool IsNeedleExposed { get; private set; }
        public bool HasDose { get; private set; }
        public bool HasSelectedInjectionSite { get; private set; }

        public bool IsReady => IsNeedleExposed && HasDose && HasSelectedInjectionSite;
        
        public void ExposeNeedle()
        {
            IsNeedleExposed = true;
        }

        public void LoadDose()
        {
            HasDose = true;
        }

        public void SelectInjectionSite()
        {
            HasSelectedInjectionSite = true;
        }
    }
}