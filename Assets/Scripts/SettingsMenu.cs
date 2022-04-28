using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    bool doEquipOnBuy;
    bool muteAudio;
    [SerializeField] Toggle equipToggle, muteToggle;

    private void Start()
    {
        CheckEquipSetting();
        CheckMuteSetting();
        equipToggle.isOn = doEquipOnBuy ? true : false;
        muteToggle.isOn = muteAudio ? true : false;

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

    void CheckEquipSetting() => doEquipOnBuy = PlayerPrefs.GetInt("EquipOnBuy") == 1;
    void CheckMuteSetting() => muteAudio = PlayerPrefs.GetInt("Mute") == 1;
    public void MainMenu() => SceneManager.LoadScene("Menu");
}
