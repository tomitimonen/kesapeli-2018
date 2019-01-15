using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public bool Mute
    {
        get
        {
            if (!PlayerPrefs.HasKey("mute"))
            {
                PlayerPrefs.SetInt("mute", 0);
                PlayerPrefs.Save();
            }
            return PlayerPrefs.GetInt("mute") != 0;
        }
        set
        {
            PlayerPrefs.SetInt("mute", value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
    public float MusicVolume
    {
        get
        {
            if (!PlayerPrefs.HasKey("musicVolume"))
            {
                PlayerPrefs.SetFloat("musicVolume", 1f);
                PlayerPrefs.Save();
            }
            return PlayerPrefs.GetFloat("musicVolume");
        }
        set
        {
            PlayerPrefs.SetFloat("musicVolume", value);
            PlayerPrefs.Save();
        }
    }
    public float SFXVolume
    {
        get
        {
            if (!PlayerPrefs.HasKey("sfxVolume"))
            {
                PlayerPrefs.SetFloat("sfxVolume", 1f);
                PlayerPrefs.Save();
            }
            return PlayerPrefs.GetFloat("sfxVolume");
        }
        set
        {
            PlayerPrefs.SetFloat("sfxVolume", value);
            PlayerPrefs.Save();
        }
    }

    static SettingsManager instance;
    public static SettingsManager Instance
    {
        get
        {
            if (instance != null) return instance;

            GameObject singletons = GameObject.Find("Singletons");
            if (singletons == null)
            {
                singletons = new GameObject("Singletons");
            }
            instance = singletons.AddComponent<SettingsManager>();

            return instance;
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

}

