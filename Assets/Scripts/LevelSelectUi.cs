using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectUI : MonoBehaviour
{
    [Header("Przyciski")]
    public Button level1Button;
    public Button level2Button;

    [Header("Kłódki (opcjonalnie)")]
    public GameObject level2Lock;

    [Header("Build Indexy (z Build Settings)")]
    public int level1BuildIndex = 1; // wpisz swój index z Build Settings
    public int level2BuildIndex = 2; // wpisz swój index z Build Settings

    void Start()
    {
        Refresh();

        if (level1Button) level1Button.onClick.AddListener(() => Load(level1BuildIndex));
        if (level2Button) level2Button.onClick.AddListener(() =>
        {
            if (GameProgress.IsUnlockedByBuildIndex(level2BuildIndex))
                Load(level2BuildIndex);
            else
                Debug.Log("Level 2 locked.");
        });
    }

    void Refresh()
    {
        int highest = GameProgress.GetHighestUnlocked();

        if (level1Button) level1Button.interactable = highest >= level1BuildIndex;

        bool l2Unlocked = GameProgress.IsUnlockedByBuildIndex(level2BuildIndex);
        if (level2Button) level2Button.interactable = l2Unlocked;
        if (level2Lock)   level2Lock.SetActive(!l2Unlocked);

        Debug.Log($"[Menu] HighestUnlocked={highest} (L1 idx={level1BuildIndex}, L2 idx={level2BuildIndex})");
    }

    void Load(int buildIndex)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(buildIndex);
    }

    // (opcjonalnie) przycisk w menu do resetu
    public void ResetProgressAndRefresh()
    {
        GameProgress.ResetAll();
        Refresh();
    }
}
