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
    [SerializeField] Button equipButton, muteButton;
    [SerializeField] Sprite checkedBox, uncheckedBox;

    private void Start()
    {
        CheckEquipSetting();
        CheckMuteSetting();
        equipButton.image.sprite = doEquipOnBuy ? checkedBox : uncheckedBox;
        muteButton.image.sprite = muteAudio ? checkedBox : uncheckedBox;
    }

    private void Update()
    {
        CheckEquipSetting();
        CheckMuteSetting();
    }

    public void EquipOnBuy()
    {
        if (!doEquipOnBuy)
        {
            CheckboxToggleOn(equipButton);
            PlayerPrefs.SetInt("EquipOnBuy", 1);
        }
        else
        {
            CheckboxToggleOff(equipButton);
            PlayerPrefs.SetInt("EquipOnBuy", 0);
        }
    }

    public void MuteAudio()
    {
        if (!muteAudio)
        {
            CheckboxToggleOn(muteButton);
            PlayerPrefs.SetInt("Mute", 1);
        }
        else
        {
            CheckboxToggleOff(muteButton);
            PlayerPrefs.SetInt("Mute", 0);
        }
    }

    void CheckboxToggleOn(Button button)
    {
        button.image.sprite = checkedBox;
    }

    void CheckboxToggleOff(Button button)
    {
        button.image.sprite = uncheckedBox;
    }

    void CheckEquipSetting() => doEquipOnBuy = PlayerPrefs.GetInt("EquipOnBuy") == 1;
    void CheckMuteSetting() => muteAudio = PlayerPrefs.GetInt("Mute") == 1;
    public void MainMenu() => SceneManager.LoadScene("Menu");
}
