using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    GameManager GameManager;
    [SerializeField] UIController ui;
    [SerializeField] GameObject floatingCoinsPrefab;
    [SerializeField] ScoreManager scoreManager;
    [SerializeField] ScoreDisplay scoreDisplay;
    [SerializeField] Button shop;
    AudioSource coinSound;
    public int coinsGained;

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern string GetData(string key);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void SetData(string key, string value);

    private void Awake()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        coinSound = GetComponent<AudioSource>();
    }

    public void Setup()
    {
        ui.pauseGame.gameObject.SetActive(false);
        ui.muteButton.gameObject.SetActive(false);
        ui.coinsTextGameOver.enabled = true;
        ui.coinsText.gameObject.SetActive(false);

#if !(UNITY_IOS || UNITY_ANDROID)
        ui.coinAd.gameObject.SetActive(false);
        shop.transform.position = new Vector3(0, shop.transform.position.y);
#endif

        //ui.abilityButton.enabled = false;
        for (int i = 0; i < scoreManager.pins.Length; i++)
            scoreManager.pins[i].SetActive(false);
        if (scoreManager.twoPlayer)
            TwoPlayerEndScreen();
        /*
        if (scoreManager.playerClass[scoreManager.player].pinScore > SPrefs.GetInt("HighScore", 0))
        {
            SPrefs.SetInt("HighScore", scoreManager.playerClass[scoreManager.player].pinScore);
            ui.highScore.text = "High Score\n" + scoreManager.playerClass[scoreManager.player].pinScore.ToString();
        }
        */

        if (floatingCoinsPrefab) Invoke(nameof(AddCoins), 0.5f);

        if (GameManager.hasRespawned)
        {
            coinsGained = (GameManager.distanceTraveled - GameManager.distanceTraveledLast) / 2;
            GameManager.coins += coinsGained;
        } else
        {
            coinsGained = Mathf.Max(scoreManager.playerClass[0].pinScore, scoreManager.playerClass[1].pinScore) / 2;
            GameManager.distanceTraveledLast = GameManager.distanceTraveled;
            GameManager.coins += coinsGained;
        }
#if UNITY_WEBGL && !UNITY_EDITOR
        SetData("Coins", GameManager.coins.ToString());
#else
        SPrefs.SetInt("Coins", GameManager.coins);
#endif
    }

    public void AddCoins()
    {
        coinSound.Play();
        ui.coinsTextGameOver.text = "<sprite anim=0,5,12>" +
#if UNITY_WEBGL && !UNITY_EDITOR
           GetData("Coins");
#else
            SPrefs.GetInt("Coins", 0).ToString();
#endif
        GameObject prefab = Instantiate(floatingCoinsPrefab,
            new Vector3(ui.coinsTextGameOver.transform.position.x + 0.8f,
            ui.coinsTextGameOver.transform.position.y - 1f), Quaternion.identity);
        prefab.GetComponentInChildren<TMP_Text>().text = "+" + coinsGained.ToString();
        Destroy(prefab, 1f);
    }

    void TwoPlayerEndScreen()
    {
        // Displays both scoreboards for two player mode
        for (int i = 0; i < scoreDisplay.scoreBoard.Length; i++)
        {
            scoreDisplay.scoreBoard[i].transform.localScale = new Vector3(0.8f, 0.8f);
            scoreDisplay.scoreBoard[i].GetComponent<Button>().enabled = false;
            scoreDisplay.scoreMoveButton[i].gameObject.SetActive(false);
        }
        scoreDisplay.scoreBoard[0].GetComponent<CanvasGroup>().alpha = 1;
        scoreDisplay.scoreBoard[0].transform.position = new Vector3(0, 1.04f);
        scoreDisplay.scoreBoard[1].transform.position = new Vector3(0, -0.97f);
        
    }
}
