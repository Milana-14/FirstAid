using UnityEngine;

public static class GameSettings
{
    private const string DifficultyKey = "GameDifficulty";

    public static GameDifficulty Difficulty { get; private set; } = (GameDifficulty)PlayerPrefs.GetInt(DifficultyKey, 0);

    public static void SetDifficulty(GameDifficulty difficulty)
    {
        Difficulty = difficulty;
        PlayerPrefs.SetInt(DifficultyKey, (int)difficulty);
        PlayerPrefs.Save();
    }
}