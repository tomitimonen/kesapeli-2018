using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Audio;

public class SettingsGUIManager : MonoBehaviour {

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private GameObject muteButton;
    [SerializeField] private GameObject unMuteButton;
	// Use this for initialization
	void Start () {
        if (musicVolumeSlider != null) musicVolumeSlider.value = SettingsManager.Instance.MusicVolume;
        if (sfxVolumeSlider != null) sfxVolumeSlider.value = SettingsManager.Instance.SFXVolume;
        if (SettingsManager.Instance.Mute) MuteAudio();
        else UnMuteAudio();
    }

    public void OnMute()
    {
        SettingsManager.Instance.Mute = true;
        MuteAudio();
    }
    public void OnUnMute()
    {
        SettingsManager.Instance.Mute = false;
        UnMuteAudio();
    }
    void MuteAudio()
    {
        setMuteButtonsVisibility(true);
        audioMixer.SetFloat("MasterVolume", -80f);
    }
    void UnMuteAudio()
    {
        setMuteButtonsVisibility(false);
        SetVolumeParameter("MasterVolume", 1f);
    }
    void setMuteButtonsVisibility(bool mute)
    {
        if (muteButton != null) muteButton.SetActive(!mute);
        if (unMuteButton != null) unMuteButton.SetActive(mute);
    }
    public void OnMusicVolumeChanged(float value)
    {
        SettingsManager.Instance.MusicVolume = value;
        SetVolumeParameter("MusicVolume", value);
    }
    public void OnSFXVolumeChanged(float value)
    {
        SettingsManager.Instance.SFXVolume = value;
        SetVolumeParameter("SFXVolume", value);
    }
    void SetVolumeParameter(string param, float value)
    {
        value = Mathf.Max(value, 0.001f);
        audioMixer.SetFloat(param, Mathf.Log10(value) * 20f);
    }
	
}
