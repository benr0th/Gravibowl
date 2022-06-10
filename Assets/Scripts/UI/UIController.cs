using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    GameManager GameManager;
    public TMPro.TextMeshProUGUI distanceText, highScore, gameOverText,
                                 powerUpText, coinsText, coinsTextGameOver;

    public Button retryButton, resetScore, quitGame,
                  pauseGame, coinAd, continueAd,
                  abilityButton;

    public Slider magnetGauge;

    public GameObject gameOverScreen, pauseScreen;

    private void Awake()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        SceneSetup();
    }

    private void Start()
    {
        highScore.text = "High Score\n" + SPrefs.GetInt("HighScore", 0).ToString();
        coinsText.text = "<sprite anim=0,5,12>" + SPrefs.GetInt("Coins", 0).ToString();
        coinsTextGameOver.text = "<sprite anim=0,5,12>" + SPrefs.GetInt("Coins", 0).ToString();
    }

    private void Update()
    {
        distanceText.text = GameManager.distanceTraveled + "m";
    }

    public void AdClick(Button buttonClicked)
    {
        if (buttonClicked == coinAd)
        {
            GameManager.coinAdClicked = true;
        } else if (buttonClicked == continueAd)
        {
            GameManager.continueAdClicked = true;
        }
    }
    
    void SceneSetup()
    {
        pauseScreen.gameObject.SetActive(false);
        powerUpText.enabled = false;
        coinsTextGameOver.enabled = false;
    }
}
