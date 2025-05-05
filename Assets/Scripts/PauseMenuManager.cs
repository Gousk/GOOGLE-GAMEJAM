using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("Assign the pause menu panel here (set inactive by default)")]
    public GameObject pauseMenuUI;

    [Header("Scene Settings")]
    [Tooltip("Name of your main menu scene")]
    public string mainMenuSceneName = "MainMenu";

    private bool isPaused = false;
    private ShootingController shootingController;

    //public OrthographicCharacterController orthographicCharacterController;
    public PlayerHealth playerHealth;
    private float drainRef;

    private void Start()
    {
        ResumeGame();
        //orthographicCharacterController = PlayerDataManager.Instance.GetComponent<OrthographicCharacterController>();
        playerHealth = PlayerDataManager.Instance.GetComponent<PlayerHealth>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);

        Time.timeScale = 0f;
        isPaused = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;

        if(shootingController != null && !shootingController.canShoot)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void ReturnToMainMenu()
    {
        // Ensure time is running in main menu
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void PauseForDialogue()
    {
        //Debug.Log(PlayerDataManager.Instance.GetComponent<OrthographicCharacterController>());
        //PlayerDataManager.Instance.GetComponent<OrthographicCharacterController>().enabled = false;
        //PlayerDataManager.Instance.GetComponent<CharacterController>().enabled = false;
        drainRef = PlayerDataManager.Instance.GetComponent<PlayerHealth>().healthDrainRate;
        PlayerDataManager.Instance.GetComponent<PlayerHealth>().healthDrainRate = 0f;
    }

    public void ResumeForDialogue()
    {
        //PlayerDataManager.Instance.GetComponent<OrthographicCharacterController>().enabled = true;
        //PlayerDataManager.Instance.GetComponent<OrthographicCharacterController>().GetComponent<CharacterController>().enabled = true;
        PlayerDataManager.Instance.GetComponent<PlayerHealth>().healthDrainRate = drainRef;

    }
}
