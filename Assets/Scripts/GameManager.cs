using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] ShipControl ship;
    [SerializeField] Camera cam;
    [SerializeField] InfHitPowerUp hitPowerUp;
    [SerializeField] GameOverScreen gameOverScreen;
    [SerializeField] Sprite[] planets;
    [SerializeField] Sprite muteOn, muteOff;
    [SerializeField] GameObject loadingScreen;
    [SerializeField] Slider loadBar;
    [SerializeField] TextMeshProUGUI loadText;
    [SerializeField] Button help, tutStart, pause, retryPause, quitPause, retryOver, quitOver, mute; 
    AudioManager audioManager;
    UIController ui;

    public GameObject planetPrefab;
    public Vector3 spawnPlanetPos, spawnPowerUpPos, shipLastPos, 
                   shipCurrentPos, shipStartPos;

    public bool gameOver, hasRespawned, inHole, canHitAgain, exitOrbit, tutRelease,
                coinAdClicked, continueAdClicked, superLaunchActive, isOrbiting,
                isPaused, slowMo, grabbedPowerUp, doEquipOnBuy, lockedOn, canStopTouching;

    public int distanceTraveled, distanceTraveledLast, coins, checkpointHits;

    [SerializeField] float yMin, yMax, maxPSpeed;
    bool upDiff, spawnPowerUp, switchSide, isMuted;
    float grabbedPowerUpTime = 4f;
    int powerUpRoll, timesPlayed;

#if UNITY_WEBGL
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern string GetData(string key);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void SetData(string key, string value);
#endif

    //public bool IsPaused
    //{
    //    get { return isPaused; }
    //    set
    //    {
    //        if (isPaused == false && value == true)
    //        {
    //            Debug.Log("pause");
    //        }
    //        isPaused = value;
    //    }
    //}

    private void Awake()
    {
        ui = GameObject.Find("UI").GetComponent<UIController>();
        audioManager = FindObjectOfType<AudioManager>();
#if !(UNITY_IOS || UNITY_ANDROID)
        GetComponent<AdsManager>().enabled = false;
#endif
    }

    private void Start()
    {
        shipStartPos = ship.transform.position;
#if UNITY_WEBGL && !UNITY_EDITOR
        int.TryParse(GetData("TimesPlayed"), out int timesplayed);
        timesPlayed = timesplayed;
        int.TryParse(GetData("Coins"), out int parseCoins);
        coins = parseCoins;
#else
        timesPlayed = SPrefs.GetInt("TimesPlayed");
        coins = SPrefs.GetInt("Coins", 0);
#endif
        InitPlanet();
        audioManager.AudioOnPress(pause, 5);
        audioManager.AudioOnPress(retryPause, 0);
        audioManager.AudioOnPress(quitPause, 6);
        audioManager.AudioOnPress(retryOver, 1);
        audioManager.AudioOnPress(quitOver, 6);
        audioManager.AudioOnPress(help, 5);
        audioManager.AudioOnPress(tutStart, 0);
        audioManager.AudioOnPress(mute, 0);
    }

    private void Update()
    {
        // Check if game is muted
#if UNITY_WEBGL && !UNITY_EDITOR
        int.TryParse(GetData("Mute"), out int muted);
        if (muted == 1)
#else
        if (SPrefs.GetInt("Mute") == 1)
#endif
        {
            isMuted = true;
            mute.GetComponent<Image>().sprite = muteOn;
        }
        else
        {
            isMuted = false;
            mute.GetComponent<Image>().sprite = muteOff;
        }

        // Power up stuff
        if (grabbedPowerUp & !isPaused)
        {
            grabbedPowerUp = true;
            // Power up timer
            grabbedPowerUpTime -= Time.deltaTime;
            // Bring speed back to normal after grabbing
            slowMo = false;
            Time.timeScale += (1f / 0.05f) * Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);

            ui.powerUpText.enabled = true;
            ui.powerUpText.text = "Keep Swiping Down!\n" + grabbedPowerUpTime.ToString("f2");
        }
        if (grabbedPowerUpTime <= 0f)
        {
            // When power up runs out
            EndPowerUp();
        }
#region Legacy powerup/difficulty code
        /*
        if (ship != null)
        {
            float shipSpeed = ship.rb.velocity.y;
            shipCurrentPos = ship.transform.position;
            distanceTraveled = Mathf.FloorToInt(shipCurrentPos.y - shipStartPos.y);
        
            // Decreases planet spawn as distance increases
            if (distanceTraveled % 100 == 0 & distanceTraveled > 0 & upDiff)
            {
                upDiff = false;
                yMin = Mathf.Clamp(yMin, 0.5f, 10f);
                yMax = Mathf.Clamp(yMax, 0.5f, 10f);
                yMin += 0.2f;
                yMax += 0.2f;
            } else if (distanceTraveled % 100 != 0 & !upDiff) { upDiff = true; }


            // Spawn power up

            if (distanceTraveled % 50 == 0 && distanceTraveled > 0 && spawnPowerUp && !grabbedPowerUp
                && ballSpeed > maxPSpeed)
            {
                spawnPowerUp = false;
                spawnPowerUpPos.y = ballCurrentPos + 3f;
                spawnPowerUpPos.x = Random.Range(-2.05f, 2.05f);
                powerUpRoll = Random.Range(1, 11);
                if (powerUpRoll <= 10)
                {
                    // Slo-mo because moving so fast
                    slowMo = true;
                    Time.timeScale = 0.05f;
                    Time.fixedDeltaTime = Time.timeScale * .02f;
                    Instantiate(hitPowerUp, spawnPowerUpPos, Quaternion.identity);
                }
            } else if (distanceTraveled % 50 != 0 && !spawnPowerUp) { spawnPowerUp = true; }
        }
        */
#endregion
    }

    public void InitPlanet()
    {
        var p = Random.Range(0, 14);
        planetPrefab.GetComponent<SpriteRenderer>().sprite = planets[p];
        spawnPlanetPos.y = Random.Range(-2.7f, -2.55f);
        var coinFlip = Random.Range(0, 2);
        if (coinFlip == 0)
        {
            spawnPlanetPos.x = -1.15f;
            Instantiate(planetPrefab, spawnPlanetPos, Quaternion.Euler(0, 180, 0));
        }
        else
        {
            spawnPlanetPos.x = 1.15f;
            Instantiate(planetPrefab, spawnPlanetPos, Quaternion.identity);
        }
    }

    public void Pause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0;
            ui.pauseScreen.SetActive(true);
            ship.enabled = false;
            ui.abilityButton.enabled = false;
            ship.isTouching = false;
        } else
        {
            if (slowMo)
            {
                Time.timeScale = 0.05f;
                Time.fixedDeltaTime = Time.timeScale * .02f;
            } else { Time.timeScale = 1; }
            ui.pauseScreen.SetActive(false);
            ship.enabled = true;
            ui.abilityButton.enabled = true;
        }
    }

    public void EndPowerUp()
    {
        grabbedPowerUp = false;
        grabbedPowerUpTime = 4f;
        ui.powerUpText.enabled = false;
    }

    public void Respawn()
    {
        ship.stoppedMoving = 0;
        ship.transform.position = shipLastPos;
        //StartCoroutine(GameOverTrigger());
        ui.coinsText.text = "<sprite anim=0,5,12>" + SPrefs.GetInt("Coins", 0).ToString();
        canHitAgain = true;
        hasRespawned = true;
        gameOver = false;
        ship.ready = false;
        ui.gameOverScreen.SetActive(false);
        ui.pauseGame.gameObject.SetActive(true);
        ui.coinsTextGameOver.enabled = false;
        ui.coinsText.gameObject.SetActive(true);
        ui.abilityButton.enabled = true;
        //Physics.IgnoreLayerCollision(10, 3);
        ship.gameObject.SetActive(true);
    }

    public void Retry()
    {
        Time.timeScale = 1;
        timesPlayed++;
#if UNITY_WEBGL && !UNITY_EDITOR
        SetData("TimesPlayed", timesPlayed.ToString());
#else
        SPrefs.SetInt("TimesPlayed", timesPlayed);
#endif
        SceneManager.LoadSceneAsync("Game");
    }

    public void ResetScore()
    {
        SPrefs.DeleteKey("HighScore");
        ui.highScore.text = "High Score\n0";
    }

    public void GameOver()
    {
        gameOver = true;
        shipLastPos = ship.transform.position;
        ui.gameOverScreen.SetActive(true);
        gameOverScreen.Setup();
    }

    public void Shop()
    {
        SceneManager.LoadSceneAsync("Shop");
    }

    public void Mute()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if (isMuted)
            SetData("Mute", "0");
        else
            SetData("Mute", "1");
#else
        if (isMuted)
            SPrefs.SetInt("Mute", 0);
        else
            SPrefs.SetInt("Mute", 1);
#endif
    }

    public void Quit()
    {
        Time.timeScale = 1;
        SceneManager.LoadSceneAsync("Menu");
    }
}
