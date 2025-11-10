using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameProgress
{
    private const string HighestKey = "HighestUnlocked"; // np. 1 = Level1, 2 = Level2

    public static void MarkCompletedCurrentLevel()
    {
        int current = SceneManager.GetActiveScene().buildIndex;
        int highest = PlayerPrefs.GetInt(HighestKey, 1); // zakładamy, że 1 = Level1 domyślnie dostępny
        int next = current + 1;

        if (next > highest)
        {
            PlayerPrefs.SetInt(HighestKey, next);
            PlayerPrefs.Save();
        }

        Debug.Log($"[GameProgress] Completed buildIndex={current}, HighestUnlocked={PlayerPrefs.GetInt(HighestKey, 1)}");
    }

    public static int GetHighestUnlocked()
    {
        return PlayerPrefs.GetInt(HighestKey, 1);
    }

    public static bool IsUnlockedByBuildIndex(int buildIndex)
    {
        return buildIndex <= GetHighestUnlocked();
    }

    // (opcjonalnie do testów)
    public static void ResetAll()
    {
        PlayerPrefs.DeleteKey(HighestKey);
        PlayerPrefs.Save();
    }
}
