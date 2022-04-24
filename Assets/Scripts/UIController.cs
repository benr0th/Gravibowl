using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    GameManager GameManager;
    public TMPro.TextMeshProUGUI distanceText;
    public TMPro.TextMeshProUGUI highScore;
    public TMPro.TextMeshProUGUI gameOverText;
    public TMPro.TextMeshProUGUI powerUpText;
    public TMPro.TextMeshProUGUI coinsTextGameOver;
    public TMPro.TextMeshProUGUI coinsText;

    public Button retryButton;
    public Button resetScore;
    public Button quitGame;
    public Button pauseGame;
    public Button coinAd;
    public Button continueAd;

    private void Awake()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        pauseGame.gameObject.SetActive(true);
        retryButton.gameObject.SetActive(false);
        quitGame.gameObject.SetActive(false);
        powerUpText.enabled = false;
        coinsTextGameOver.enabled = false;
    }

    private void Start()
    {
        highScore.text = "High Score\n" + PlayerPrefs.GetInt("HighScore", 0).ToString();
        coinsText.text = "<sprite anim=0,5,12>" + PlayerPrefs.GetInt("Coins", 0).ToString();
        coinsTextGameOver.text = "<sprite anim=0,5,12>" + PlayerPrefs.GetInt("Coins", 0).ToString();
        coinAd.onClick.AddListener(CoinClick);
        continueAd.onClick.AddListener(ContinueClick);

    }

    private void Update()
    {
        distanceText.text = GameManager.distanceTraveled + "m";
    }

    void CoinClick()
    {
        GameManager.coinAdClicked = true;
    }

    void ContinueClick()
    {
        GameManager.continueAdClicked = true;
    }
    
}
