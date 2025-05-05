using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public float MusicVolume;
    public float SFXVolume;

    public Slider musicSlider;
    public Slider soundSlider;

    public List<AudioSource> soundEffects;
    public AudioSource music;

    private void Start()
    {
        MusicVolume = MenuManager.Instance.MusicVolume;
        SFXVolume = MenuManager.Instance.SFXVolume;

        MenuManager.Instance.musicSlider = musicSlider;
        MenuManager.Instance.sfxSlider = soundSlider;

        MenuManager.Instance.musicSource = music;
        foreach (var item in soundEffects)
        {
            MenuManager.Instance.sfxSources.Add(item);
        }

        MenuManager.Instance.StartFunction();
    }
}