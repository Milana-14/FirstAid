using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Dropdown))]
public class ResolutionDropdown : MonoBehaviour
{
    private const string PrefKey = "ResolutionIndex";

    // Matches "1920x1080", "1920 x 1080", "1920 X 1080", "1920 × 1080"
    private static readonly Regex ResolutionPattern = new Regex(@"(\d+)\s*[xX×]\s*(\d+)");

    private TMP_Dropdown dropdown;

    private void Awake()
    {
        dropdown = GetComponent<TMP_Dropdown>();
    }

    private void Start()
    {
        // Restore the saved choice if it's still a valid option index.
        int saved = PlayerPrefs.GetInt(PrefKey, dropdown.value);
        if (saved >= 0 && saved < dropdown.options.Count)
        {
            dropdown.SetValueWithoutNotify(saved);
            dropdown.RefreshShownValue();
        }

        dropdown.onValueChanged.AddListener(SetResolution);
        ApplyResolution(dropdown.value);
    }

    private void OnDestroy()
    {
        if (dropdown != null)
            dropdown.onValueChanged.RemoveListener(SetResolution);
    }

    public void SetResolution(int index)
    {
        if (ApplyResolution(index))
        {
            PlayerPrefs.SetInt(PrefKey, index);
            PlayerPrefs.Save();
        }
    }

    private bool ApplyResolution(int index)
    {
        if (index < 0 || index >= dropdown.options.Count) return false;

        string text = dropdown.options[index].text;
        Match match = ResolutionPattern.Match(text);

        if (!match.Success)
        {
            Debug.LogWarning($"ResolutionDropdown: couldn't read a resolution from option \"{text}\". Use a format like 1920 x 1080.");
            return false;
        }

        int width = int.Parse(match.Groups[1].Value);
        int height = int.Parse(match.Groups[2].Value);

        Screen.SetResolution(width, height, Screen.fullScreenMode);
        return true;
    }
}