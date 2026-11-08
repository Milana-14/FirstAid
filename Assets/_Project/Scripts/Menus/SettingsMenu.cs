using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;

    public TMP_Dropdown qualityDropdown;
    public TMP_Dropdown resolutionDropdown;

    private const string QualityKey = "QualityLevel";
    private const string ResolutionKey = "ResolutionIndex";

    // Matches "1920x1080", "1920 x 1080", "1920 X 1080", "1920 × 1080"
    private static readonly Regex ResolutionPattern = new Regex(@"(\d+)\s*[xX×]\s*(\d+)");

    private void Start()
    {
        SetupQuality();
        SetupResolution();
    }

    private void OnDestroy()
    {
        if (resolutionDropdown != null)
            resolutionDropdown.onValueChanged.RemoveListener(SetResolution);
    }

    // ---------- Quality ----------

    private void SetupQuality()
    {
        // Fill the dropdown from Project Settings > Quality
        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(QualitySettings.names.ToList());

        // Load the saved level (or fall back to the current one)
        int saved = PlayerPrefs.GetInt(QualityKey, QualitySettings.GetQualityLevel());
        saved = Mathf.Clamp(saved, 0, QualitySettings.names.Length - 1);

        QualitySettings.SetQualityLevel(saved);
        qualityDropdown.SetValueWithoutNotify(saved);
        qualityDropdown.RefreshShownValue();
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt(QualityKey, qualityIndex);
        PlayerPrefs.Save();
    }

    // ---------- Resolution ----------

    private void SetupResolution()
    {
        var options = Screen.resolutions
            .Select(r => $"{r.width} x {r.height}")
            .Distinct()
            .ToList();

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(options);

        string current = $"{Screen.width} x {Screen.height}";
        int defaultIndex = Mathf.Max(0, options.IndexOf(current));

        int saved = PlayerPrefs.GetInt(ResolutionKey, defaultIndex);
        saved = Mathf.Clamp(saved, 0, options.Count - 1);

        resolutionDropdown.SetValueWithoutNotify(saved);
        resolutionDropdown.RefreshShownValue();

        resolutionDropdown.onValueChanged.AddListener(SetResolution);
        ApplyResolution(saved);
}

    public void SetResolution(int index)
    {
        if (ApplyResolution(index))
        {
            PlayerPrefs.SetInt(ResolutionKey, index);
            PlayerPrefs.Save();
        }
    }

    private bool ApplyResolution(int index)
    {
        if (index < 0 || index >= resolutionDropdown.options.Count) return false;

        string text = resolutionDropdown.options[index].text;
        Match match = ResolutionPattern.Match(text);

        if (!match.Success)
        {
            Debug.LogWarning($"SettingsMenu: couldn't read a resolution from option \"{text}\". Use a format like 1920 x 1080.");
            return false;
        }

        int width = int.Parse(match.Groups[1].Value);
        int height = int.Parse(match.Groups[2].Value);

        Screen.SetResolution(width, height, Screen.fullScreenMode);
        return true;
    }

    // ---------- Other settings ----------

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("MainVolume", volume);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
}