using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    [Tooltip("Starting health")]
    public float health;

    [Header("Health Drain Settings")]
    [Tooltip("How much health is lost per second")]
    public float healthDrainRate = 0f;

    [Header("UI")]
    public Slider healthBar;      // assign your UI Slider here

    void Start()
    {
        StartCoroutine(HealthStar());
    }

    private IEnumerator HealthStar()
    {
        // find the slider
        healthBar = FindAnyObjectByType<HealthTag>().GetComponent<Slider>();
        yield return new WaitUntil(() => healthBar != null);

        // initialize health
        health = maxHealth;

        // set slider to 0–1
        healthBar.minValue = 0f;
        healthBar.maxValue = 1f;
        healthBar.value = 1f; // full at start
    }

    void Update()
    {
        // 1) Continuous drain
        if (healthDrainRate > 0f)
            health = Mathf.Clamp(health - healthDrainRate * Time.deltaTime, 0f, maxHealth);

        // 2) Clamp (safety)
        health = Mathf.Clamp(health, 0f, maxHealth);

        // 3) Update the UI slider in normalized 0–1 space
        if (healthBar != null)
            healthBar.value = health / maxHealth;

        // 4) Debug/testing keys
        if (Input.GetKeyDown(KeyCode.H))
            ModifyHealth(-10f);
        if (Input.GetKeyDown(KeyCode.J))
            ModifyHealth(+15f);

        // 5) Death check
        if (health <= 0f)
            Die();
    }

    /// <summary>
    /// Call this to change health (negative = damage, positive = heal).
    /// </summary>
    public void ModifyHealth(float delta)
    {
        health = Mathf.Clamp(health + delta, 0f, maxHealth);
        if (healthBar != null)
            healthBar.value = health / maxHealth;
    }

    private void Die()
    {
        // Call respawn
        RespawnManager.Instance?.RespawnPlayer();

        // Reset health
        health = maxHealth;
    }

    /// <summary>
    /// Exposed reset for other systems (e.g. rewind) to restore full health.
    /// </summary>
    public void ResetToFull()
    {
        health = maxHealth;
        if (healthBar != null)
            healthBar.value = 1f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("OutOfBounds"))
            Die();
    }
}
