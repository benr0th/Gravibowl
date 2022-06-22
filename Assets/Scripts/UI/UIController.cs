using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    GameManager GameManager;
    public TextMeshProUGUI distanceText, highScore, gameOverText,
                                 powerUpText, coinsText, coinsTextGameOver;

    public Button retryButton, resetScore, quitGame,
                  pauseGame, coinAd, continueAd,
                  abilityButton, muteButton;

    public Slider magnetGauge;

    public GameObject gameOverScreen, pauseScreen;

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern string GetData(string key);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void SetData(string key, string value);

    private void Awake()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        SceneSetup();
    }

    private void Start()
    {
        highScore.text = "High Score\n" + SPrefs.GetInt("HighScore", 0).ToString();
#if UNITY_WEBGL && !UNITY_EDITOR
        coinsText.text = "<sprite anim=0,5,12>" + GetData("Coins");
        coinsTextGameOver.text = "<sprite anim=0,5,12>" + GetData("Coins");
#else
        coinsText.text = "<sprite anim=0,5,12>" + SPrefs.GetInt("Coins", 0).ToString();
        coinsTextGameOver.text = "<sprite anim=0,5,12>" + SPrefs.GetInt("Coins", 0).ToString();
#endif
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
        powerUpText.enabled = false;
        coinsTextGameOver.enabled = false;
    }
}
