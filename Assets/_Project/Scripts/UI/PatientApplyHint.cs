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
            HideHint();
            return;
        }

        LocalizationService.Instance.GetLocalizedString("UI_Table", provider.HintKey, localizedText => hintText.text = localizedText);
    }

    public void HideHint()
    {
        hintText.text = string.Empty;
    }
}