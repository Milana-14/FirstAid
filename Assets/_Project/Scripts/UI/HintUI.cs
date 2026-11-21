using TMPro;
using UnityEngine;

public sealed class HintUI : MonoBehaviour
{
    [SerializeField] private GameObject thoughtPanel;
    [SerializeField] private TMP_Text thoughtText;

    public void ShowThought(string text)
    {
        thoughtPanel.SetActive(true);
        thoughtText.text = text;
    }

    public void Hide()
    {
        thoughtPanel.SetActive(false);
    }
}