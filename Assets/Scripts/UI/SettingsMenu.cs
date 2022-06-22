using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] Toggle equipToggle, muteToggle, shakeToggle, leftHandToggle;
    [SerializeField] Button menu;
    [SerializeField] TMP_Dropdown bgDropdown;
    AudioManager audioManager;
    bool doEquipOnBuy, muteAudio, noScreenShake, leftHandOn;

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern string GetData(string key);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void SetData(string key, string value);

    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    private void Update()
    {
        // Check if game is muted
#if UNITY_WEBGL && !UNITY_EDITOR
        int.TryParse(GetData("Mute"), out int muted);
        if (muted == 1)
#else
        if (SPrefs.GetInt("Mute") == 1)
#endif
            AudioListener.volume = 0;
        else
            AudioListener.volume = 1;
    }

    private void Start()
    {
        audioManager.AudioOnPress(menu, 0);
        equipToggle.onValueChanged.AddListener(delegate { audioManager.PlaySound(2); });
        muteToggle.onValueChanged.AddListener(delegate { audioManager.PlaySound(2); });
        leftHandToggle.onValueChanged.AddListener(delegate { audioManager.PlaySound(2); });
        CheckSettings();
        CheckToggleStatus();
        audioManager.audioSources[2].enabled = true;
    }

    public void EquipOnBuy(bool doEquipToggle)
    {
        int setting = doEquipToggle ? 1 : 0;
#if UNITY_WEBGL && !UNITY_EDITOR
        SetData("EquipOnBuy", setting.ToString());
#else
        SPrefs.SetInt("EquipOnBuy", setting);
#endif

    }

    public void MuteAudio(bool muteToggleOn)
    {
        int setting = muteToggleOn ? 1 : 0;
#if UNITY_WEBGL && !UNITY_EDITOR
        SetData("Mute", setting.ToString());
#else
        SPrefs.SetInt("Mute", setting);
#endif
    }

    public void NoScreenShake(bool noScreenShake)
    {
        int setting = noScreenShake ? 1 : 0;
#if UNITY_WEBGL && !UNITY_EDITOR
        SetData("NoScreenShake", setting.ToString());
#else
        SPrefs.SetInt("NoScreenShake", setting);
#endif
    }

    public void LeftHandOn(bool leftHandOn)
    {
        int setting = leftHandOn ? 1 : 0;
#if UNITY_WEBGL && !UNITY_EDITOR
        SetData("LeftHandOn", setting.ToString());
#else
        SPrefs.SetInt("LeftHandOn", setting);
#endif
    }

    void CheckSettings()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        int.TryParse(GetData("EquipOnBuy"), out int eob);
        doEquipOnBuy = eob == 1;
        int.TryParse(GetData("Mute"), out int muted);
        muteAudio = muted == 1;
        int.TryParse(GetData("NoScreenShake"), out int noscreenshake);
        noScreenShake = noscreenshake == 1;
        int.TryParse(GetData("LeftHandOn"), out int lefty);
        leftHandOn = lefty == 1;
        int.TryParse(GetData("Background"), out int background);
        bgDropdown.value = background;
#else
        doEquipOnBuy = SPrefs.GetInt("EquipOnBuy") == 1;
        muteAudio = SPrefs.GetInt("Mute") == 1;
        noScreenShake = SPrefs.GetInt("NoScreenShake") == 1;
        leftHandOn = SPrefs.GetInt("LeftHandOn") == 1;
        bgDropdown.value = SPrefs.GetInt("Background");
#endif
    }

    void CheckToggleStatus()
    {
        audioManager.audioSources[2].enabled = false;
        equipToggle.isOn = doEquipOnBuy;
        muteToggle.isOn = muteAudio;
        shakeToggle.isOn = noScreenShake;
        leftHandToggle.isOn = leftHandOn;
    }

    public void Dropdown(int val)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        switch (val)
        {
            case 0:
                SetData("Background", "0");
                break;
            case 1:
                SetData("Background", "1");
                break;
            case 2:
                SetData("Background", "2");
                break;
        }
#else
        switch (val)
        {
            case 0: SPrefs.SetInt("Background", 0);
                break;
            case 1: SPrefs.SetInt("Background", 1);
                break;
            case 2: SPrefs.SetInt("Background", 2);
                break;
        }
#endif
    }

    public void MainMenu() => SceneManager.LoadSceneAsync("Menu");
}
