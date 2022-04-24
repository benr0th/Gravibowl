using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverScreen : MonoBehaviour
{
    GameManager GameManager;
    [SerializeField] UIController ui;
    [SerializeField] GameObject floatingCoinsPrefab;
    [SerializeField] AudioSource coinSound;


    private void Awake()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameObject.SetActive(false);
    }

    public void Setup()
    {
        gameObject.SetActive(true);
        ui.pauseGame.gameObject.SetActive(false);
        ui.coinsTextGameOver.enabled = true;
        ui.coinsText.gameObject.SetActive(false);
        GameManager.coins += GameManager.distanceTraveled / 2;
        PlayerPrefs.SetInt("Coins", GameManager.coins);

        if (GameManager.distanceTraveled > PlayerPrefs.GetInt("HighScore", 0))
        {
            PlayerPrefs.SetInt("HighScore", GameManager.distanceTraveled);
            ui.highScore.text = "High Score\n" + GameManager.distanceTraveled.ToString();
        }

        if (floatingCoinsPrefab)
        {
            Invoke(nameof(AddCoins), 0.5f);
        }
    }

    public void AddCoins()
    {
        coinSound.Play();
        ui.coinsTextGameOver.text = "<sprite anim=0,5,12>" + PlayerPrefs.GetInt("Coins", 0).ToString();
        GameObject prefab = Instantiate(floatingCoinsPrefab, 
            new Vector3(transform.position.x + 0.5f, transform.position.y),Quaternion.identity);
        prefab.GetComponentInChildren<TMP_Text>().text = "+" + (GameManager.distanceTraveled / 2).ToString();
        Destroy(prefab, 1f);
    }
}
