using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    GameManager GameManager;
    public Text distanceText;
    public Text highScore;
    public Text gameOverText;
    public TMPro.TextMeshProUGUI powerUpText;
    public Button retryButton;
    public Button resetScore;
    public Button quitGame;
    public Button pauseGame;

    private void Awake()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        pauseGame.gameObject.SetActive(true);
        retryButton.gameObject.SetActive(false);
        quitGame.gameObject.SetActive(false);
        powerUpText.enabled = false;
    }

    private void Start()
    {
        highScore.text = "High Score\n" + PlayerPrefs.GetInt("HighScore", 0).ToString();
    }

    private void Update()
    {
        distanceText.text = GameManager.distanceTraveled + "m";
    }

}
