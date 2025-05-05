using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    [Header("Audio Settings (Menu)")]
    public Slider musicSlider;
    public Slider sfxSlider;
    public AudioSource musicSource;
    [Tooltip("All existing AudioSources you use for SFX")]
    public List<AudioSource> sfxSources = new List<AudioSource>();

    // live values persisted between scenes
    public float MusicVolume = 1f;
    public float SFXVolume = 1f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        StartFunction();
    }

    public void StartFunction()
    {
        // initialize sliders & audio
        if (musicSlider != null)
        {
            musicSlider.value = MusicVolume;
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }
        if (sfxSlider != null)
        {
            sfxSlider.value = SFXVolume;
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }

        if (musicSource != null) musicSource.volume = MusicVolume;
        ApplySFXVolume(SFXVolume);
    }

    public void SetMusicVolume(float volume)
    {
        MusicVolume = volume;
        if (musicSource != null)
            musicSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        SFXVolume = volume;
        ApplySFXVolume(volume);
    }

    private void ApplySFXVolume(float volume)
    {
        foreach (var src in sfxSources)
        {
            if (src != null)
                src.volume = volume;
        }
    }
}