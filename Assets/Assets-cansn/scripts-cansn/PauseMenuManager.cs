using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenuManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject pauseMenu;

    private bool isPaused = false;

    void Start()
    {
        // G�venlik: oyun asla donuk ba�lamas�n
        Time.timeScale = 1f;

        if (pauseMenu != null)
            pauseMenu.SetActive(false);

        isPaused = false;
    }

    void Update()
    {
        // P tu�u ile pause toggle
        if (Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame)
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    void Pause()
    {
        if (pauseMenu != null)
            pauseMenu.SetActive(true);

        Time.timeScale = 0f;
        isPaused = true;
    }

    void Resume()
    {
        if (pauseMenu != null)
            pauseMenu.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;
    }

    // UI BUTTON
    public void ToggleSound()
    {
        AudioListener.volume = AudioListener.volume == 0f ? 1f : 0f;
    }

    // UI BUTTON
    public void GoToMainMenu()
    {
        Debug.Log("pressed");
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
