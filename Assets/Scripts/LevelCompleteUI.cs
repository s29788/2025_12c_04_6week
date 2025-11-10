using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelCompleteUI : MonoBehaviour
{
    [Header("Referencje UI")]
    public CanvasGroup panel;   // CanvasGroup na Twoim panelu
    public Button btnMenu;
    public Button btnNext;

    [Header("Sceny")]
    [Tooltip("Nazwa sceny z menu głównym (dokładnie jak w Build Settings).")]
    public string menuSceneName = "MainMenu";
    [Tooltip("Jeśli puste, Next załaduje kolejną scenę po indeksie Build. Jeśli chcesz konkretną, wpisz nazwę.")]
    public string nextSceneName = "";

    bool isShown = false;

    void Awake()
    {
        if (btnMenu) btnMenu.onClick.AddListener(GoMenu);
        if (btnNext) btnNext.onClick.AddListener(GoNext);
        HideInstant();
    }

    public void Show()
    {
        isShown = true;
        gameObject.SetActive(true);
        panel.alpha = 1f;
        panel.blocksRaycasts = true;
        panel.interactable = true;
        Time.timeScale = 0f; // pauza gry
    }

    public void HideInstant()
    {
        isShown = false;
        gameObject.SetActive(true); // żeby można było znaleźć przez FindObjectOfType
        panel.alpha = 0f;
        panel.blocksRaycasts = false;
        panel.interactable = false;
    }

    public void GoMenu()
    {
        if (!isShown) return;
        Time.timeScale = 1f;
        if (!string.IsNullOrEmpty(menuSceneName))
            SceneManager.LoadScene(menuSceneName);
        else
            Debug.LogError("LevelCompleteUI: menuSceneName jest puste.");
    }

    public void GoNext()
    {
        if (!isShown) return;
        Time.timeScale = 1f;

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
            return;
        }

        // domyślnie: kolejna scena wg Build Index
        int i = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(i + 1);
    }
}
