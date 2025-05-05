using System.Collections;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public enum CollectibleType
    {
        Currency,
        Staff,
        Spellbook,
        MageHat,
        LevelPiece,
        TimePotion
    }

    [Header("Collectible Settings")]
    public CollectibleType type;
    public float bobHeight = 0.5f;
    public float bobSpeed = 2f;
    public int Amount = 1;

    [Header("Rotation Settings")]
    [Tooltip("Maximum yaw angle (±) to lerp between.")]
    public float rotationAngle = 45f;
    [Tooltip("Speed of the back-and-forth rotation.")]
    public float rotationSpeed = 1f;

    [Header("References")]
    public GameObject model;    // the visible child model

    [Header("Audio")]
    [Tooltip("Sound to play when this item is collected")]
    public AudioClip collectSfx;
    [Tooltip("Volume for the collect SFX; set this from another script")]
    public float sfxVolume = 1f;

    private Vector3 initialModelPosition;
    private Quaternion initialModelRotation;
    private bool isCollected = false;
    [Space]
    public ShootingController shootController;
    public TimeRewindAbility rewindController;
    public OrthographicCharacterController characterController;
    public PlayerData playerData;
    public MapSettings mapSettings;

    void Start()
    {
        if (model == null)
            model = this.gameObject;

        initialModelPosition = model.transform.localPosition;
        initialModelRotation = model.transform.localRotation;

        shootController = PlayerDataManager.Instance.GetComponent<ShootingController>();
        rewindController = PlayerDataManager.Instance.GetComponent<TimeRewindAbility>();
        characterController = PlayerDataManager.Instance.GetComponent<OrthographicCharacterController>();
        playerData = PlayerDataManager.Instance.GetComponent<PlayerData>();
        mapSettings = FindAnyObjectByType<MapSettings>();   
    }

    void Update()
    {
        if (isCollected) return;

        if(MenuManager.Instance != null)
        {
            sfxVolume = MenuManager.Instance.SFXVolume;
        }
        else
        {
            sfxVolume = 1f;
        }
        // up-and-down bobbing
        float newY = initialModelPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        Vector3 pos = model.transform.localPosition;
        pos.y = newY;
        model.transform.localPosition = pos;

        // back-and-forth rotation around Y
        float yaw = Mathf.Sin(Time.time * rotationSpeed) * rotationAngle;
        model.transform.localRotation = initialModelRotation * Quaternion.Euler(0f, yaw, 0f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (isCollected) return;
        if (!other.CompareTag("Player")) return;

        isCollected = true;
        HandleCollection();
    }

    private void HandleCollection()
    {
        //// 1) Play SFX with correct volume
        //if (collectSfx != null)
        //{
        //    AudioSource sfxSource = gameObject.AddComponent<AudioSource>();
        //    sfxSource.clip = collectSfx;
        //    sfxSource.volume = sfxVolume;
        //    sfxSource.Play();
        //    Destroy(sfxSource, collectSfx.length);


        //}

        // 1) Play SFX
        if (collectSfx != null)
            AudioSource.PlayClipAtPoint(collectSfx, transform.position, sfxVolume);

        // 2) Apply game logic
        switch (type)
        {
            case CollectibleType.Currency:
                PlayerDataManager.Instance.AddCoins(Amount);
                StartCoroutine(playerData.UpdateCoinText());
                PlayerDataManager.Instance.GetComponent<PlayerHealth>().ModifyHealth(Amount*5);
                break;

            case CollectibleType.Staff:
                shootController.canShoot = true;
                playerData.staffCollected = true;
                mapSettings.keyPiecesCollected++;
                break;

            case CollectibleType.Spellbook:
                rewindController.canRewind = true;
                playerData.bookCollected = true;
                StartCoroutine(ActivateRewind());
                break;

            case CollectibleType.MageHat:
                characterController.items.hat.SetActive(true);
                characterController.ghostItems.hat.SetActive(true);
                characterController.items.cape.SetActive(true);
                characterController.ghostItems.cape.SetActive(true);
                playerData.hatCollected = true;
                playerData.capeCollected = true;
                mapSettings.keyPiecesCollected++;
                break;

            case CollectibleType.LevelPiece:
                // handle level piece logic here
                mapSettings.keyPiecesCollected++;
                PlayerDataManager.Instance.GetComponent<PlayerHealth>().ModifyHealth(Amount);
                break;

            case CollectibleType.TimePotion:
                // handle level piece logic here
                PlayerDataManager.Instance.GetComponent<PlayerHealth>().ModifyHealth(Amount);
                break;
        }

        // 3) Destroy after a frame so the SFX can play
        if(type != CollectibleType.Spellbook)
            Destroy(gameObject);
    }

    private IEnumerator ActivateRewind()
    {
        Debug.Log("hey1");
        yield return new WaitUntil(()=> rewindController != null);
        rewindController.enabled = true;
        yield return new WaitUntil(() => rewindController.enabled == true);
        rewindController.canRewind = true;
        yield return new WaitUntil(() => rewindController.canRewind == true);
        Destroy(gameObject);
        Debug.Log("hey2");
    }
}
