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

    void Update()
    {
        coinsText.text = "Coins: " + PlayerPrefs.GetInt("Coins");
        selectedSkin.sprite = skinManager.GetSelectedSkin().sprite;
    }

    public void MainMenu() => SceneManager.LoadScene("Menu");
    public void BackToGame() => SceneManager.LoadScene("HoleInOneHundred");
}
