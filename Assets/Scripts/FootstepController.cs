// FootstepSoundSystem.cs
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FootstepSoundSystem : MonoBehaviour
{
    [System.Serializable]
    public class TagAudio
    {
        [Tooltip("Eşleşecek tag adı (örneğin \"Grass\", \"Wood\" vb.)")]
        public string tag;
        [Tooltip("Bu tag üzerinde yürüyünce rastgele çalınacak clip’ler")]
        public AudioClip[] clips;
    }

    [Header("Tag’e Göre Footstep Sesleri")]
    public List<TagAudio> tagFootstepSounds = new List<TagAudio>();

    [Header("Fallback (tag’e uyan yoksa)")]
    public AudioClip[] defaultFootstepSounds;

    [Header("Adım Mesafesi")]
    [Tooltip("Bu kadar dünya birimi kat edince bir adım sesi çalar.")]
    public float stepDistance = 2f;

    [Header("Zemin Algılama")]
    [Tooltip("Raycast’in hangi katmanlara çarpacağını ayarlar.")]
    public LayerMask groundLayers = ~0;

    [Header("Yürüme Durumu (dışarıdan set edilecek)")]
    [Tooltip("Sadece bu true iken footstep çalınır.")]
    public bool isWalking = false;

    private AudioSource audioSource;
    private Vector3 lastPosition;
    private float stepCycle;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        lastPosition = transform.position;
        stepCycle = 0f;
    }

    void Update()
    {
        // Eğer yürüme flag’i kapalıysa cycle sıfırla ve çık
        if (!isWalking)
        {
            stepCycle = 0f;
            lastPosition = transform.position;
            return;
        }

        // Yürüyor olunca, kat edilen mesafeyi say
        float dist = Vector3.Distance(transform.position, lastPosition);
        stepCycle += dist;

        if (stepCycle >= stepDistance)
        {
            PlayFootstep();
            stepCycle -= stepDistance;
        }

        lastPosition = transform.position;
    }

    private void PlayFootstep()
    {
        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * 0.1f;

        if (Physics.Raycast(origin, Vector3.down, out hit, 1.5f, groundLayers))
        {
            string groundTag = hit.collider.tag;
            AudioClip clipToPlay = null;

            // Önce tag listesinde ara
            foreach (var entry in tagFootstepSounds)
            {
                if (entry.tag == groundTag && entry.clips.Length > 0)
                {
                    clipToPlay = entry.clips[Random.Range(0, entry.clips.Length)];
                    break;
                }
            }

            // Bulunamazsa fallback kullan
            if (clipToPlay == null && defaultFootstepSounds.Length > 0)
            {
                clipToPlay = defaultFootstepSounds[Random.Range(0, defaultFootstepSounds.Length)];
            }

            if (clipToPlay != null)
                audioSource.PlayOneShot(clipToPlay);
        }
    }
}
