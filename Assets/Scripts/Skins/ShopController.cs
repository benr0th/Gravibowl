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
        coinsText.text = "Coins: " + SPrefs.GetInt("Coins");
        selectedSkin.sprite = skinManager.GetSelectedSkin().sprite;
    }

    public void MainMenu() => SceneManager.LoadSceneAsync("Menu");
    public void BackToGame() => SceneManager.LoadSceneAsync("Game");
}
