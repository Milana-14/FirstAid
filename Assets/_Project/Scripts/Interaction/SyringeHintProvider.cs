using _Project.Scripts.Interaction;
using UnityEngine;

public sealed class SyringeHintProvider : MonoBehaviour, IInteractionHintProvider
{
    [SerializeField] private string prepareHintKey;
    [SerializeField] private string injectHintKey;
    [SerializeField] private Syringe syringe;

    public string HintKey => syringe.IsReady ? injectHintKey : prepareHintKey;
}