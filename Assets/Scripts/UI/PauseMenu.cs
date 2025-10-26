using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject _pauseView;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _pauseView.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void ContinueClicked()
    {
        _pauseView.SetActive(false);
        Time.timeScale = 1;
    }

    public void MainMenuClicked()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
