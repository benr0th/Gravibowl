using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ShopController : MonoBehaviour
{
    [SerializeField] Image selectedSkin;
    [SerializeField] TextMeshProUGUI coinsText;
    [SerializeField] SkinManager skinManager;
    [SerializeField] Button menu;
    AudioManager audioManager;

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern string GetData(string key);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void SetData(string key, string value);

    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();   
    }

    private void Start()
    {
        audioManager.AudioOnPress(menu, 0);
    }

    void Update()
    {
        coinsText.text = "<sprite anim=0,5,12>" +
#if UNITY_WEBGL && !UNITY_EDITOR
            GetData("Coins");
#else
            SPrefs.GetInt("Coins");
#endif
        selectedSkin.sprite = skinManager.GetSelectedSkin().sprite;
    }

    public void MainMenu() => SceneManager.LoadSceneAsync("Menu");
    public void BackToGame() => SceneManager.LoadSceneAsync("Game");
}
