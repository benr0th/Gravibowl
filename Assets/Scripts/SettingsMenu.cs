using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    bool doEquipOnBuy, muteAudio, noScreenShake, leftHandOn;
    [SerializeField] Toggle equipToggle, muteToggle, shakeToggle, leftHandToggle;

    private void Start()
    {
        CheckSettings();
        CheckToggleStatus();
    }

    public void EquipOnBuy(bool doEquipToggle)
    {
        int setting = doEquipToggle ? 1 : 0;
        PlayerPrefs.SetInt("EquipOnBuy", setting);
    }

    public void MuteAudio(bool muteToggle)
    {
        int setting = muteToggle ? 1 : 0;
        PlayerPrefs.SetInt("Mute", setting);
    }

    public void NoScreenShake(bool noScreenShake)
    {
        int setting = noScreenShake ? 1 : 0;
        PlayerPrefs.SetInt("NoScreenShake", setting);
    }

    public void LeftHandOn(bool leftHandOn)
    {
        int setting = leftHandOn ? 1 : 0;
        PlayerPrefs.SetInt("LeftHandOn", setting);
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
