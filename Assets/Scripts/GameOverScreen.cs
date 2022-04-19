using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    GameManager GameManager;
    UIController ui;

    private void Awake()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        ui = GameObject.Find("UI").GetComponent<UIController>();
        gameObject.SetActive(false);
    }

    public void Setup()
    {
        gameObject.SetActive(true);
        ui.pauseGame.gameObject.SetActive(false);
        if (GameManager.distanceTraveled > PlayerPrefs.GetInt("HighScore", 0))
        {
            PlayerPrefs.SetInt("HighScore", GameManager.distanceTraveled);
            ui.highScore.text = "High Score\n" + GameManager.distanceTraveled.ToString();
        }
    }
}
