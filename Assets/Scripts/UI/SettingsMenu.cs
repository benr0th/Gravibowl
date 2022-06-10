using System.Collections;
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

    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    private void Update()
    {
        // Check if game is muted
        if (SPrefs.GetInt("Mute") == 1)
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
        SPrefs.SetInt("EquipOnBuy", setting);
    }

    public void MuteAudio(bool muteToggleOn)
    {
        int setting = muteToggleOn ? 1 : 0;
        SPrefs.SetInt("Mute", setting);
    }

    public void NoScreenShake(bool noScreenShake)
    {
        int setting = noScreenShake ? 1 : 0;
        SPrefs.SetInt("NoScreenShake", setting);
    }

    public void LeftHandOn(bool leftHandOn)
    {
        int setting = leftHandOn ? 1 : 0;
        SPrefs.SetInt("LeftHandOn", setting);
    }

    void CheckSettings()
    {
        doEquipOnBuy = SPrefs.GetInt("EquipOnBuy") == 1;
        muteAudio = SPrefs.GetInt("Mute") == 1;
        noScreenShake = SPrefs.GetInt("NoScreenShake") == 1;
        leftHandOn = SPrefs.GetInt("LeftHandOn") == 1;
        bgDropdown.value = SPrefs.GetInt("Background");
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
        switch (val)
        {
            case 0: SPrefs.SetInt("Background", 0);
                break;
            case 1: SPrefs.SetInt("Background", 1);
                break;
            case 2: SPrefs.SetInt("Background", 2);
                break;
        }
    }

    public void MainMenu() => SceneManager.LoadSceneAsync("Menu");
}
