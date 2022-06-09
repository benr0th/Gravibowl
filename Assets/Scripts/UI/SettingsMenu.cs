using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] Toggle equipToggle, muteToggle, shakeToggle, leftHandToggle;
    [SerializeField] Button menu;
    AudioManager audioManager;
    bool doEquipOnBuy, muteAudio, noScreenShake, leftHandOn;

    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    private void Start()
    {
        CheckSettings();
        CheckToggleStatus();
        audioManager.AudioOnPress(menu, 0);
    }

    public void EquipOnBuy(bool doEquipToggle)
    {
        int setting = doEquipToggle ? 1 : 0;
        PlayerPrefs.SetInt("EquipOnBuy", setting);
        equipToggle.onValueChanged.AddListener((setting) => audioManager.PlaySound(2));
    }

    public void MuteAudio(bool muteToggleOn)
    {
        int setting = muteToggleOn ? 1 : 0;
        PlayerPrefs.SetInt("Mute", setting);
        muteToggle.onValueChanged.AddListener((setting) => audioManager.PlaySound(2));
    }

    public void NoScreenShake(bool noScreenShake)
    {
        int setting = noScreenShake ? 1 : 0;
        PlayerPrefs.SetInt("NoScreenShake", setting);
        shakeToggle.onValueChanged.AddListener((setting) => audioManager.PlaySound(2));
    }

    public void LeftHandOn(bool leftHandOn)
    {
        int setting = leftHandOn ? 1 : 0;
        PlayerPrefs.SetInt("LeftHandOn", setting);
        leftHandToggle.onValueChanged.AddListener((setting) => audioManager.PlaySound(2));
    }

    void CheckSettings()
    {
        doEquipOnBuy = PlayerPrefs.GetInt("EquipOnBuy") == 1;
        muteAudio = PlayerPrefs.GetInt("Mute") == 1;
        noScreenShake = PlayerPrefs.GetInt("NoScreenShake") == 1;
        leftHandOn = PlayerPrefs.GetInt("LeftHandOn") == 1;
    }

    void CheckToggleStatus()
    {
        equipToggle.isOn = doEquipOnBuy ? true : false;
        muteToggle.isOn = muteAudio ? true : false;
        shakeToggle.isOn = noScreenShake ? true : false;
        leftHandToggle.isOn = leftHandOn ? true : false;
    }

    public void MainMenu() => SceneManager.LoadSceneAsync("Menu");
}
