using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class TimeRewindAbility : MonoBehaviour
{
    private AnimationController animController;

    [Header("Record Settings")]
    public float recordDuration = 5f;
    public float recordInterval = 0.1f;

    [Header("Rewind Settings")]
    public float rewindSpeed = 15f;
    public float abilityCooldown = 10f;
    public KeyCode rewindKey = KeyCode.R;

    [Header("Finish Lerp")]
    public float finishLerpDuration = 0.3f;

    [Header("Health Rewind")]
    public PlayerHealth playerHealth;
    public float healthLerpSpeed = 10f;

    [Header("Ghost Visuals")]
    public GameObject ghostPrefab;
    public float ghostFadeDuration = 0.2f;

    [Header("Optional Trace Line")]
    public LineRenderer traceLine;

    [Header("References")]
    public Transform playerModel;
    public Animator playerAnimator;

    // --- Snapshot holds position, rotation, anim & health ---
    struct Snapshot
    {
        public Vector3 pos;
        public Quaternion rot;
        public int animStateHash;
        public float animNormalizedTime;
        public float health;
    }

    private CharacterController cc;
    private List<Snapshot> history = new List<Snapshot>();
    public float recordTimer, cooldownTimer;

    // Rewind playback data
    private bool isRewinding;
    private Vector3[] pathPositions;
    private Quaternion[] pathRotations;
    private float[] cumDist;
    private float[] healthHistory;
    private float totalDist;
    private float traveledDist;

    // Ghost
    public GameObject ghostInst;
    private Transform ghostT;
    private Animator ghostAnimator;
    private List<Material> ghostMats = new List<Material>();
    private List<float> ghostAlphas = new List<float>();
    private float ghostFadeTimer;

    public bool canRewind;
    public GameObject spellBookObj;
    public OrthographicCharacterController characterController;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        animController = FindAnyObjectByType<AnimationController>();

        // if not assigned in inspector, fetch
        if (playerHealth == null)
            playerHealth = GetComponent<PlayerHealth>();

        if (ghostPrefab != null)
        {
            ghostInst = Instantiate(ghostPrefab, transform.position, Quaternion.identity);
            ghostT = ghostInst.transform;
            characterController.ghostItems = ghostT.GetComponentInChildren<ItemSelector>(true);
            ghostInst.SetActive(false);
            ghostAnimator = ghostInst.GetComponentInChildren<Animator>();

            foreach (var r in ghostInst.GetComponentsInChildren<Renderer>())
                foreach (var m in r.materials)
                {
                    ghostMats.Add(m);
                    ghostAlphas.Add(m.HasProperty("_Color") ? m.color.a : 1f);
                }
        }
    }

    void Update()
    {
        if(!canRewind)
            return;
        else if(canRewind && !spellBookObj.activeSelf)
        {
            spellBookObj.SetActive(true);
            FindAnyObjectByType<OrthographicCharacterController>().ghostItems.book.SetActive(true);
            traceLine.enabled = true;
        }


        cooldownTimer = Mathf.Max(0f, cooldownTimer - Time.deltaTime);

        if (!isRewinding)
        {
            // 1) Record everything
            recordTimer += Time.deltaTime;
            if (recordTimer >= recordInterval)
            {
                recordTimer -= recordInterval;
                var state = playerAnimator.GetCurrentAnimatorStateInfo(0);

                history.Add(new Snapshot
                {
                    pos = transform.position,
                    rot = playerModel.rotation,
                    animStateHash = state.fullPathHash,
                    animNormalizedTime = state.normalizedTime,
                    health = playerHealth.health
                });

                while (history.Count > Mathf.CeilToInt(recordDuration / recordInterval))
                    history.RemoveAt(0);
            }

            // 2) Ghost & trace
            if (history.Count > 1 && ghostInst != null)
            {
                var first = history[0];
                if (!ghostInst.activeSelf && cooldownTimer <= 0f)
                {
                    ghostInst.SetActive(true);
                    if (traceLine) traceLine.enabled = true;
                    ghostFadeTimer = 0f;
                    SetGhostAlpha(0f);
                    ghostT.position = first.pos;
                    ghostT.rotation = first.rot;
                }
                else
                {
                    ghostT.position = Vector3.Lerp(ghostT.position, first.pos, Time.deltaTime * 10f);
                    ghostT.rotation = Quaternion.Slerp(ghostT.rotation, first.rot, Time.deltaTime * 10f);
                }

                // replay anim
                ghostAnimator.Play(first.animStateHash, 0, first.animNormalizedTime);
                ghostAnimator.Update(0f);

                // fade
                ghostFadeTimer += Time.deltaTime;
                float p = Mathf.Clamp01(ghostFadeTimer / ghostFadeDuration);
                for (int i = 0; i < ghostMats.Count; i++)
                    if (ghostMats[i].HasProperty("_Color"))
                    {
                        var c = ghostMats[i].color;
                        c.a = ghostAlphas[i] * p;
                        ghostMats[i].color = c;
                    }

                // trace
                if (traceLine != null)
                {
                    traceLine.positionCount = history.Count;
                    for (int i = 0; i < history.Count; i++)
                        traceLine.SetPosition(i, history[i].pos);
                }
            }
            else
            {
                if (ghostInst != null && ghostInst.activeSelf) ghostInst.SetActive(false);
                if (traceLine) traceLine.positionCount = 0;
            }

            // 3) Trigger rewind
            if (Input.GetKeyDown(rewindKey) && cooldownTimer <= 0f && history.Count > 1)
                BeginRewind();
        }
    }

    void BeginRewind()
    {
        isRewinding = true;
        traveledDist = 0f;
        gameObject.layer = 9;
        animController.PlayAnimation("Casting");

        int n = history.Count;
        pathPositions = new Vector3[n];
        pathRotations = new Quaternion[n];
        cumDist = new float[n];
        healthHistory = new float[n];

        for (int i = 0; i < n; i++)
        {
            pathPositions[i] = history[i].pos;
            pathRotations[i] = history[i].rot;
            healthHistory[i] = history[i].health;
            cumDist[i] = (i == 0)
                ? 0f
                : cumDist[i - 1] + Vector3.Distance(pathPositions[i - 1], pathPositions[i]);
        }

        totalDist = cumDist[n - 1];
        if (traceLine != null)
            traceLine.positionCount = n;

        GetComponent<OrthographicCharacterController>().enabled = false;
    }

    void FixedUpdate()
    {
        if (!isRewinding) return;

        traveledDist += rewindSpeed * Time.fixedDeltaTime;
        float curDist = Mathf.Max(0f, totalDist - traveledDist);

        if (curDist <= 0f)
        {
            if (ghostInst) ghostInst.SetActive(false);
            if (traceLine) traceLine.positionCount = 0;
            StartCoroutine(FinishRewind());
            return;
        }

        int idx = System.Array.BinarySearch(cumDist, curDist);
        if (idx < 0) idx = ~idx;
        idx = Mathf.Clamp(idx, 1, cumDist.Length - 1);

        float segLen = cumDist[idx] - cumDist[idx - 1];
        float t = segLen > 0f
            ? (curDist - cumDist[idx - 1]) / segLen
            : 0f;

        Vector3 targetPos = Vector3.Lerp(pathPositions[idx - 1], pathPositions[idx], t);
        Quaternion targetRot = Quaternion.Slerp(pathRotations[idx - 1], pathRotations[idx], t);

        cc.Move(targetPos - transform.position);
        playerModel.rotation = targetRot;

        // **Health Lerp**
        float targetHealth = healthHistory[idx];
        playerHealth.health = Mathf.Lerp(playerHealth.health, targetHealth, healthLerpSpeed * Time.fixedDeltaTime);

        if (traceLine != null)
        {
            int count = idx;
            traceLine.positionCount = count + 1;
            for (int i = 0; i <= count; i++)
                traceLine.SetPosition(i, pathPositions[Mathf.Min(i, history.Count - 1)]);
        }
    }

    IEnumerator FinishRewind()
    {
        isRewinding = false;
        cooldownTimer = abilityCooldown;
        gameObject.layer = 8;
        animController.StopAnimation("Casting");
        if (traceLine) traceLine.enabled = false;

        Vector3 startPos = transform.position;
        Quaternion startRot = playerModel.rotation;
        Vector3 endPos = pathPositions[0];
        Quaternion endRot = pathRotations[0];
        float endHealth = healthHistory[0];

        cc.enabled = false;
        float timer = 0f;
        while (timer < finishLerpDuration)
        {
            timer += Time.deltaTime;
            float p = Mathf.Clamp01(timer / finishLerpDuration);
            transform.position = Vector3.Lerp(startPos, endPos, p);
            playerModel.rotation = Quaternion.Slerp(startRot, endRot, p);
            playerHealth.health = Mathf.Lerp(playerHealth.health, endHealth, p);
            yield return null;
        }

        transform.position = endPos;
        playerModel.rotation = endRot;
        playerHealth.health = endHealth;

        history.Clear();
        cc.enabled = true;
        GetComponent<OrthographicCharacterController>().enabled = true;
    }

    void SetGhostAlpha(float a)
    {
        for (int i = 0; i < ghostMats.Count; i++)
            if (ghostMats[i].HasProperty("_Color"))
            {
                var c = ghostMats[i].color;
                c.a = a;
                ghostMats[i].color = c;
            }
    }

    /// <summary>
    /// Fully resets the rewind ability:
    /// - Stops any ongoing rewind
    /// - Clears recorded history and trace
    /// - Resets cooldown timer
    /// - Hides and repositions the ghost
    /// - Re-enables movement/controllers
    /// </summary>
    public void ResetAbility()
    {
        // 1) Abort rewind
        isRewinding = false;

        // 2) Clear recording history and trace line
        history.Clear();
        if (traceLine != null)
        {
            traceLine.positionCount = 0;
            traceLine.enabled = false;
        }

        // 3) Reset cooldown so you can rewind immediately
        cooldownTimer = 0f;

        // 4) Hide and reposition ghost
        if (ghostInst != null)
        {
            ghostInst.SetActive(false);
            ghostT.position = transform.position;
            ghostT.rotation = playerModel.rotation;
        }

        // 5) Re-enable character movement & controller
        if (cc != null) cc.enabled = true;
        var ortho = GetComponent<OrthographicCharacterController>();
        if (ortho != null) ortho.enabled = true;

        // 6) Restore your layer if you changed it during rewind
        gameObject.layer = 8; // or whatever your default “player” layer is
    }
}
