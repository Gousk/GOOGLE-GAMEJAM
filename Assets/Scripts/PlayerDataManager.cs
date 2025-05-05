using UnityEngine;
using TMPro;

/// <summary>
/// Singleton MonoBehaviour that updates the PlayerData component locally in-memory.
/// Save this in PlayerDataManager.cs.
/// </summary>
public class PlayerDataManager : MonoBehaviour
{
    private static PlayerDataManager _instance;
    public static PlayerDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("[Singleton] PlayerDataManager");
                _instance = go.AddComponent<PlayerDataManager>();
            }
            return _instance;
        }
    }

    private PlayerData _playerData;
    private float _accumulator;
    public TMP_Text coinText;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Find the PlayerData component on the Player GameObject
        _playerData = FindObjectOfType<PlayerData>();
        if (_playerData == null)
        {
            Debug.LogError("PlayerData component not found! Attach PlayerData.cs to your Player.");
            _playerData = gameObject.AddComponent<PlayerData>();
        }

        // Start with default values; disable persistent loading
        _accumulator = 0f;
    }

    private void Update()
    {
        // Update play time on the component once per second
        _accumulator += Time.deltaTime;
        if (_accumulator >= 1f)
        {
            _playerData.totalPlayTime += _accumulator;
            _accumulator = 0f;
        }
    }

    /// <summary>
    /// Stub: In-memory save only. No file operations.
    /// </summary>
    public void SaveData()
    {
        // Intentionally left blank to keep data in memory only
    }

    /// <summary>
    /// Stub: Persistent loading disabled.
    /// </summary>
    public void LoadData()
    {
        // Intentionally left blank; no data loaded from disk
    }

    /// <summary>
    /// Reset the PlayerData component’s fields to default.
    /// </summary>
    public void ResetData()
    {
        if (_playerData == null) return;
        _playerData.totalPlayTime = 0;
        _playerData.currentLevel = 0;
        _playerData.totalCoins = 0;
        _playerData.bookCollected = false;
        _playerData.staffCollected = false;
        _playerData.hatCollected = false;
        _playerData.capeCollected = false;
        // No persistent save
    }

    #region Helper Methods
    public void AddCoins(int amount)
    {
        if (_playerData == null || amount <= 0) return;
        _playerData.currentCoins += amount;
        _playerData.totalCoins += amount;
        SaveData();
        if (coinText != null)
            coinText.text = _playerData.currentCoins.ToString();
    }

    public bool SpendCoins(int amount)
    {
        if (_playerData == null || amount <= 0 || _playerData.currentCoins < amount) return false;
        _playerData.currentCoins -= amount;
        SaveData();
        if (coinText != null)
            coinText.text = _playerData.currentCoins.ToString();
        return true;
    }

    public void LevelUp()
    {
        if (_playerData == null) return;
        _playerData.currentLevel++;
        Debug.Log($"Player leveled up to {_playerData.currentLevel}!");
        SaveData();
    }

    public bool IsItemCollected(string itemName)
    {
        if (_playerData == null) return false;
        switch (itemName.ToLower())
        {
            case "book": return _playerData.bookCollected;
            case "staff": return _playerData.staffCollected;
            case "hat": return _playerData.hatCollected;
            case "cape": return _playerData.capeCollected;
            default: return false;
        }
    }
    #endregion
}