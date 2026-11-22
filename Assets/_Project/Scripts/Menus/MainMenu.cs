using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingMenu : MonoBehaviour
{
    [Header("Difficulty Buttons")]
    [SerializeField] private Button easyButton;
    [SerializeField] private Button hardButton;

    [Header("Visual Colors")]
    [SerializeField] private Color selectedColor = Color.white;
    [SerializeField] private Color unselectedColor = new Color(0.6f, 0.6f, 0.6f, 1f);

    private void Start()
    {
        RefreshDifficultyUI();
    }

    public void SelectEasy()
    {
        GameSettings.SetDifficulty(GameDifficulty.Easy);
        RefreshDifficultyUI();
    }

    public void SelectHard()
    {
        GameSettings.SetDifficulty(GameDifficulty.Hard);
        RefreshDifficultyUI();
    }

    private void RefreshDifficultyUI()
    {
        bool isEasy = GameSettings.Difficulty == GameDifficulty.Easy;

        ApplyButtonState(easyButton, isEasy);
        ApplyButtonState(hardButton, !isEasy);
    }

    private void ApplyButtonState(Button button, bool isSelected)
    {
        if (button == null) return;

        button.gameObject.SetActive(true);
        button.interactable = !isSelected;

        if (button.targetGraphic != null)
        {
            button.targetGraphic.color = isSelected ? selectedColor : unselectedColor;
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}