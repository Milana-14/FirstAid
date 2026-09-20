using TMPro;
using UnityEngine;

public sealed class PatientApplyHint : MonoBehaviour
{
    [SerializeField] private TMP_Text hintText;

    public void ShowHint(Transform target)
    {
        IInteractionHintProvider provider = target.GetComponent<IInteractionHintProvider>();

        if (provider == null)
        {
            provider = target.GetComponentInParent<IInteractionHintProvider>();
            
            if (provider == null)
            {
                hintText.text = string.Empty;
                return;
            }
        }

        LocalizationService.Instance.GetLocalizedString("UI_Table", provider.HintKey, localizedText => hintText.text = localizedText);
    }
}