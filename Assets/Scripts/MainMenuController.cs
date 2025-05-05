using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    [Header("Fade & Scene Load")]
    [Tooltip("Full-screen black UI Image")]
    public Image fadeImage;
    [Tooltip("Fade duration in seconds")]
    public float fadeDuration = 1f;
    [Tooltip("Name of the game scene to load")]
    public string gameSceneName;

    void Start()
    {
        if (fadeImage != null)
        {
            var c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
            fadeImage.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Hook this to your Play button.
    /// </summary>
    public void PlayGame()
    {
        StartCoroutine(FadeToBlackAndLoad());
    }

    private IEnumerator FadeToBlackAndLoad()
    {
        float timer = 0f;
        Color c = fadeImage.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            c.a = Mathf.Clamp01(timer / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        c.a = 1f;
        fadeImage.color = c;

        SceneManager.LoadScene(gameSceneName);
    }

    /// <summary>
    /// Hook this to your Quit button.
    /// </summary>
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}