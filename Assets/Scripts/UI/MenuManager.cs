#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject _mainView;
    [SerializeField] private GameObject _levelsView;

    private void Awake()
    {
        _mainView.SetActive(true);
        _levelsView.SetActive(false);
    }

    #region Main view
    public void LevelSelectClicked()
    {
        _mainView.SetActive(false);
        _levelsView.SetActive(true);
    }

    public void ExitClicked()
    {
        #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    #endregion
    
    #region Levels view
    public void LoadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void BackClicked()
    {
        _mainView.SetActive(true);
        _levelsView.SetActive(false);
    }
    #endregion
}
