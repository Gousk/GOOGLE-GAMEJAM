using UnityEngine;
using System;
using System.IO;
using TMPro;
using System.Collections;

// Serializable class to hold all player-related data
[Serializable]
public class PlayerData : MonoBehaviour
{
    // Total play time in seconds
    public double totalPlayTime;

    // Example fields - add or remove as needed
    public int currentLevel;

    public int currentCoins;
    public int totalCoins;

    public bool bookCollected;
    public bool staffCollected;
    public bool hatCollected;
    public bool capeCollected;

    public TMP_Text coinText;

    public PlayerDataManager playerDataManager;

    private void Start()
    {
        playerDataManager = GetComponent<PlayerDataManager>();
        coinText = playerDataManager.coinText;
    }

    public IEnumerator UpdateCoinText()
    {
        coinText = FindAnyObjectByType<CoinTextRef>().GetComponent<TMP_Text>();
        yield return new WaitUntil(()=> coinText!=null);
        coinText.text = currentCoins.ToString();
    }
    // Add more fields here (e.g., unlockedAchievements, bestScores, inventory, settings...)
}
