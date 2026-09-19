using UnityEngine;

public sealed class Applicable : MonoBehaviour, IInteractionHintProvider
{
    [SerializeField] private string applyHintKey;

    public string HintKey => applyHintKey;
}