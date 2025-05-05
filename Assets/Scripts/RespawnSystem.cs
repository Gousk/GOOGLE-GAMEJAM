using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// Singleton manager that handles player respawn and checkpoint notifications.
/// </summary>
public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance { get; private set; }

    [Header("Spawn Points")]
    [Tooltip("Default spawn point if no checkpoint reached yet.")]
    public Transform defaultSpawnPoint;
    private Transform currentSpawnPoint;

    [Header("UI Notifier")]
    [Tooltip("TextMeshProUGUI element for checkpoint messages.")]
    public TextMeshProUGUI notificationText;
    public float fadeInDuration = 0.5f;
    public float displayDuration = 1.5f;
    public float fadeOutDuration = 0.5f;

    private Coroutine notifRoutine;
    private Color notifOriginalColor;

    public GameObject player;
    private CharacterController charController;

    public List<Checkpoint> checkpointList;
    public MapSettings mapSettings;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        currentSpawnPoint = defaultSpawnPoint;
        if (notificationText != null)
            notifOriginalColor = notificationText.color;
    }

    void Start()
    {
        if(player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
            charController = player.GetComponent<CharacterController>();

        StartCoroutine(FirstSpawn());
    }

    public IEnumerator FirstSpawn()
    {
        yield return new WaitUntil(()=> player != null || currentSpawnPoint != null);
        currentSpawnPoint = defaultSpawnPoint;
        yield return new WaitUntil(() => currentSpawnPoint == defaultSpawnPoint);
        Debug.Log("ALOLOL");
        RespawnPlayer();
    }

    /// <summary>
    /// Updates the current checkpoint. Called by Checkpoint.
    /// </summary>
    public void SetCheckpoint(Transform checkpoint)
    {
        currentSpawnPoint = checkpoint;
    }

    /// <summary>
    /// Teleports player to the current spawn point and resets health.
    /// </summary>
    public void RespawnPlayer()
    {
        if (player == null || currentSpawnPoint == null)
            return;

        if (charController != null)
            charController.enabled = false;

        player.transform.position = currentSpawnPoint.position;
        if(player.GetComponent<TimeRewindAbility>().canRewind)
        {
            player.GetComponent<TimeRewindAbility>().ResetAbility();
        }
        //player.transform.rotation = currentSpawnPoint.rotation;

        if (charController != null)
            charController.enabled = true;

        var health = player.GetComponent<PlayerHealth>();
        if (health != null)
            health.ResetToFull();
    }

    public void RespawnPlayerAtDefualt()
    {
        if (player == null || defaultSpawnPoint == null)
            return;

        if (charController != null)
            charController.enabled = false;

        player.transform.position = defaultSpawnPoint.position;
        if (player.GetComponent<TimeRewindAbility>().canRewind)
        {
            player.GetComponent<TimeRewindAbility>().ResetAbility();
        }
        //player.transform.rotation = currentSpawnPoint.rotation;

        if (charController != null)
            charController.enabled = true;

        var health = player.GetComponent<PlayerHealth>();
        if (health != null)
            health.ResetToFull();
    }

    /// <summary>
    /// Shows a checkpoint reached message with fade-in/out.
    /// </summary>
    public void ShowCheckpoint(string message)
    {
        if (notificationText == null) return;
        notificationText.text = message;

        if (notifRoutine != null)
            StopCoroutine(notifRoutine);
        notifRoutine = StartCoroutine(ShowRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        // Fade in
        float t = 0f;
        while (t < fadeInDuration)
        {
            t += Time.deltaTime;
            SetAlpha(Mathf.Clamp01(t / fadeInDuration));
            yield return null;
        }
        SetAlpha(1f);

        // Display
        yield return new WaitForSeconds(displayDuration);

        // Fade out
        t = 0f;
        while (t < fadeOutDuration)
        {
            t += Time.deltaTime;
            SetAlpha(1f - Mathf.Clamp01(t / fadeOutDuration));
            yield return null;
        }
        SetAlpha(0f);
    }

    private void SetAlpha(float alpha)
    {
        if (notificationText == null) return;
        Color c = notifOriginalColor;
        c.a = alpha;
        notificationText.color = c;
    }
}