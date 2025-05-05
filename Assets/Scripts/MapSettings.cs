using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.RayTracingAccelerationStructure;

public class MapSettings : MonoBehaviour
{
    public string NextLevel;

    public float gravity;
    public float airInfluence;

    public int keyPiecesNeeded;
    public int keyPiecesCollected;
    public int neededBombCount;
    public int destroyedBombCount;

    public bool isLevel1;
    public bool isLevel2;
    public bool isLevel3;
    public GameObject bigKey;
    public GameObject staff;
    public GameObject hat;
    public GameObject door;

    public GameObject topFloor;
    public int totalCoins;
    public TMP_Text coinText;


    public List<Collectible> coins = new List<Collectible>();

    public GameObject NextLevelUI;
    public TMP_Text NextLevelUICoin;
    public GameObject GameEndUI;

    bool isPaused = false;

    public OrthographicCharacterController OrthographicCharacterController;

    public List<Transform> checkpoints = new List<Transform>();
    private RespawnManager respawnManager;

    private void Start()
    {
        StartCoroutine(SetMapSettings());
        StartCoroutine(SetCheckPoints());
        respawnManager.mapSettings = this;
        totalCoins = PlayerDataManager.Instance.GetComponent<PlayerData>().currentCoins;
        coinText.text = totalCoins.ToString();
    }

    private void Update()
    {
        Debug.Log(OrthographicCharacterController.gravity + " " + gravity + "anlık");
        Debug.Log(OrthographicCharacterController.airDirectionInfluence + " " + airInfluence + "anlık");

        if (isLevel1)
        {
            if(keyPiecesCollected == 3)
            {
                bigKey.SetActive(true);   
            }
        }
        if(isLevel2)
        {
            if (keyPiecesCollected == 3)
            {
                staff.SetActive(true);
                door.SetActive(false);
            }
        }
        if (isLevel3)
        {
            if (keyPiecesCollected == 3)
            {
                if(destroyedBombCount == neededBombCount)
                {
                    hat.SetActive(true);
                }
            }
        }

        if (keyPiecesCollected == keyPiecesNeeded && !isLevel3)
        {
            NextLevelSequence();
        }
        else if(keyPiecesCollected == keyPiecesNeeded && isLevel3)
        {
            EndGameSequence();
        }
    }

    private IEnumerator SetMapSettings()
    {
        OrthographicCharacterController = PlayerDataManager.Instance.GetComponent<OrthographicCharacterController>();
        yield return new WaitUntil(() => OrthographicCharacterController != null);
        OrthographicCharacterController.gravity = gravity;
        OrthographicCharacterController.airDirectionInfluence = airInfluence;
        Debug.Log(OrthographicCharacterController.gravity + " " + gravity + "önce");
        Debug.Log(OrthographicCharacterController.airDirectionInfluence + " " + airInfluence + "önce");
        yield return new WaitUntil(() => OrthographicCharacterController.gravity == gravity && OrthographicCharacterController.airDirectionInfluence == airInfluence);
        Debug.Log(OrthographicCharacterController.gravity + " " + gravity + "sonra");
        Debug.Log(OrthographicCharacterController.airDirectionInfluence + " " + airInfluence + "sonra");
        Debug.Log("mapsettingsapplied");

        if(isLevel2)
            OrthographicCharacterController.GetComponent<MapVisibilityManager>().TopFloor = topFloor;
    }

    private IEnumerator SetCheckPoints()
    {
        respawnManager = RespawnManager.Instance;
        yield return new WaitUntil(() => respawnManager != null);
        for (int i = 0; i < respawnManager.checkpointList.Count; i++)
        {
            respawnManager.checkpointList[i].transform.position = checkpoints[i].transform.position;
        }

        StartCoroutine(respawnManager.FirstSpawn());
    }

    public void NextLevelSequence()
    {
        NextLevelPanel();
    }

    public void EndGameSequence()
    {
        if (GameEndUI != null)
            GameEndUI.SetActive(true);

        Time.timeScale = 0f;
        isPaused = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void NextLevelPanel()
    {
        if (NextLevelUI != null)
            NextLevelUI.SetActive(true);
        NextLevelUICoin.text = "x"+totalCoins.ToString();

        Time.timeScale = 0f;
        isPaused = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(NextLevel);
    }
}
