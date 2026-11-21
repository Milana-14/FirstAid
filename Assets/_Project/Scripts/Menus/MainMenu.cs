using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingMenu : MonoBehaviour
{
    [Header("Difficulty Selection")]
    [SerializeField] private GameObject easySelected;
    [SerializeField] private GameObject hardSelected;
    
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
        easySelected.SetActive(GameSettings.Difficulty == GameDifficulty.Easy);
        hardSelected.SetActive(GameSettings.Difficulty == GameDifficulty.Hard);
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