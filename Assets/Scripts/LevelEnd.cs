using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class LevelEnd : MonoBehaviour
{
    [Header("Po dotknięciu")]
    public float delay = 0.6f;
    public bool disablePlayerControl = true;
    public string menuSceneName = "MainMenu";

    bool done;

    void OnValidate()
    {
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (done || !other.CompareTag("Player")) return;
        done = true;

        var rb = other.attachedRigidbody;
        if (rb) rb.linearVelocity = Vector2.zero;

        if (disablePlayerControl)
        {
            var root = other.transform.root;
            var ctrl = root.GetComponent<PlayerController2D>();
            if (ctrl) ctrl.enabled = false;
            var combat = root.GetComponent<PlayerCombat2D>();
            if (combat) combat.enabled = false;
        }

        StartCoroutine(FinishSequence());
    }

    IEnumerator FinishSequence()
    {
        if (delay > 0f) yield return new WaitForSeconds(delay);

        // 🔥 ZAPISZ PROGRES – TO JEST KLUCZ
        GameProgress.MarkCompletedCurrentLevel();

        if (!string.IsNullOrEmpty(menuSceneName))
            SceneManager.LoadScene(menuSceneName);
        else
            Debug.LogError("LevelEnd: Ustaw 'menuSceneName' na nazwę sceny menu.");
    }
}
