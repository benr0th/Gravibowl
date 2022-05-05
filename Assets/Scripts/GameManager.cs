using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] BallControl ball;
    [SerializeField] Camera cam;
    [SerializeField] InfHitPowerUp hitPowerUp;
    [SerializeField] GameOverScreen gameOverScreen;
    UIController ui;

    public GameObject holePrefab;
    public Vector3 spawnHolePos, spawnPowerUpPos, ballLastPos;

    public bool gameOver, hasRespawned, inHole, canHitAgain,
                coinAdClicked, continueAdClicked, superLaunchActive,
                isPaused, slowMo, grabbedPowerUp, doEquipOnBuy;

    [SerializeField] float yMin, yMax, maxPSpeed;
    bool upDiff, spawnPowerUp, switchSide;

    public int distanceTraveled, distanceTraveledLast, coins;
    public float ballStartPos;
    float ballCurrentPos, grabbedPowerUpTime = 4f;
    int powerUpRoll;

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
    }

    private void Start()
    {
        spawnHolePos = new Vector3();
        ballStartPos = ball.transform.position.y;
        coins = PlayerPrefs.GetInt("Coins", 0);
        if (PlayerPrefs.GetInt("Mute") == 1)
            AudioListener.volume = 0;
        else
            AudioListener.volume = 1;
        StartCoroutine(GameOverTrigger());
    }

    private void Update()
    {   
        // Power up stuff
        if (grabbedPowerUp && !isPaused)
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

        if (ball != null)
        {
            float ballSpeed = ball.rb.velocity.y;
            ballCurrentPos = ball.transform.position.y;
            distanceTraveled = Mathf.FloorToInt(ballCurrentPos - ballStartPos);

            // Decreases hole spawn as distance increases
            if (distanceTraveled % 100 == 0 && distanceTraveled > 0 && upDiff)
            {
                upDiff = false;
                yMin = Mathf.Clamp(yMin, 0.5f, 10f);
                yMax = Mathf.Clamp(yMax, 0.5f, 10f);
                yMin += 0.2f;
                yMax += 0.2f;
            } else if (distanceTraveled % 100 != 0 && !upDiff) { upDiff = true; }
            // hold onto spawnHolePos.x = Random.Range(-2.05f, 2.05f);
            // Spawns holes
            if (ballCurrentPos > spawnHolePos.y - 4f)
            {
                switchSide = !switchSide;
                spawnHolePos.y += Random.Range(yMin, yMax);
                if (switchSide)
                    spawnHolePos.x = Random.Range(0, 2.05f);
                else
                    spawnHolePos.x = Random.Range(-2.05f, 0);
                Instantiate(holePrefab, spawnHolePos, Quaternion.identity);
            }

            // Spawn power up
            /*
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
            */

        }     
    }

    public void Pause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0;
            ui.pauseScreen.SetActive(true);
            ball.enabled = false;
            ui.abilityButton.enabled = false;
            ball.isTouching = false;
        } else
        {
            if (slowMo)
            {
                Time.timeScale = 0.05f;
                Time.fixedDeltaTime = Time.timeScale * .02f;
            } else { Time.timeScale = 1; }
            ui.pauseScreen.SetActive(false);
            ball.enabled = true;
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
        ball.stoppedMoving = 0;
        ball.transform.position = ballLastPos;
        StartCoroutine(GameOverTrigger());
        ui.coinsText.text = "<sprite anim=0,5,12>" + PlayerPrefs.GetInt("Coins", 0).ToString();
        canHitAgain = true;
        hasRespawned = true;
        gameOver = false;
        ball.ready = false;
        ui.gameOverScreen.SetActive(false);
        ui.pauseGame.gameObject.SetActive(true);
        ui.coinsTextGameOver.enabled = false;
        ui.coinsText.gameObject.SetActive(true);
        ui.abilityButton.enabled = true;
        //Physics.IgnoreLayerCollision(10, 3);
        ball.gameObject.SetActive(true);
    }

    public void Retry()
    {
        Time.timeScale = 1;
        SceneManager.LoadSceneAsync("Game");
    }

    public void ResetScore()
    {
        PlayerPrefs.DeleteKey("HighScore");
        ui.highScore.text = "High Score\n0";
    }

    public void GameOver()
    {
        gameOver = true;
        ballLastPos = ball.transform.position;
        ui.gameOverScreen.SetActive(true);
        gameOverScreen.Setup();
    }

    IEnumerator GameOverTrigger()
    {
        yield return new WaitUntil(() => ball.stoppedMoving > 3);
        GameOver();
    }

    public void Quit()
    {
        Time.timeScale = 1;
        SceneManager.LoadSceneAsync("Menu");
    }
}
