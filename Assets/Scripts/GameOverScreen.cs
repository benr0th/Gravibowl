using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverScreen : MonoBehaviour
{
    GameManager GameManager;
    UIController ui;
    public GameObject floatingCoinsPrefab;

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
        ui.coinsTextGameOver.enabled = true;
        ui.coinsText.gameObject.SetActive(false);
        GameManager.coins += GameManager.distanceTraveled;

        if (GameManager.distanceTraveled > PlayerPrefs.GetInt("HighScore", 0))
        {
            PlayerPrefs.SetInt("HighScore", GameManager.distanceTraveled);
            ui.highScore.text = "High Score\n" + GameManager.distanceTraveled.ToString();
        }
        if (floatingCoinsPrefab)
        {
            Invoke(nameof(CoinFloat), 0.7f);
            
        }
    }

    void CoinFloat()
    {
        GameObject prefab = Instantiate(floatingCoinsPrefab, 
            new Vector3(transform.position.x + 0.5f, transform.position.y),Quaternion.identity);
        prefab.GetComponentInChildren<TMP_Text>().text = "+" + GameManager.distanceTraveled.ToString();
        Destroy(prefab, 1f);
    }
}
