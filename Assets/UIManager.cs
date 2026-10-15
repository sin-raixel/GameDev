using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManagerScript : MonoBehaviour
{
    public static UIManagerScript instance;

    [SerializeField] private GameObject pause;
    [SerializeField] private GameObject death;
    [SerializeField] private GameObject win;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip gameOverSound;

    private bool isActive = false;

    private void Start()
    {
        instance = this;
        pause.SetActive(isActive);
        death.SetActive(false);
        win.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void OnResume()
    {
        TogglePause();
        Time.timeScale = 1;
    }

    public void OnRestart()
    {
        TogglePause();
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void ShowDeathUI()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        death.SetActive(true);
        audioSource.PlayOneShot(gameOverSound);
    }

    public void ShowWinUI()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        win.SetActive(true);
        audioSource.PlayOneShot(winSound);
    }

    public void OnRespawn()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void TogglePause()
    {
        isActive = !isActive;
        pause.SetActive(isActive);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = isActive;

        if (Time.timeScale == 1)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }
}