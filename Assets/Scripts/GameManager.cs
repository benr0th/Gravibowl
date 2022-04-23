using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public BallControl ball;
    public Camera cam;
    public InfHitPowerUp hitPowerUp;
    public GameOverScreen gameOverScreen;
    UIController ui;

    public GameObject hole;
    public Vector3 spawnHolePos;
    public Vector3 spawnPowerUpPos;

    public bool gameOver;

    [SerializeField]
    private float yMin;
    [SerializeField]
    private float yMax;
    private bool upDiff;
    public bool isPaused;
    public bool slowMo;

    public int distanceTraveled;
    public int coins;
    private float ballStartPos;
    private float ballCurrentPos;

    [HideInInspector]
    public float grabbedPowerUpTime = 4f;
    public bool grabbedPowerUp;
    private bool spawnPowerUp;
    private int powerUpRoll;
    [SerializeField]
    private float maxPSpeed;

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
        coins = PlayerPrefs.GetInt("Coins");
    }

    private void Update()
    {
        yMin = Mathf.Clamp(yMin, 0.2f, 100f);
        yMax = Mathf.Clamp(yMax, 0.2f, 100f);
        
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

        if (ball != null && hitPowerUp != null)
        {
            float ballSpeed = ball.rb.velocity.y;
            ballCurrentPos = ball.transform.position.y;
            distanceTraveled = Mathf.FloorToInt(ballCurrentPos - ballStartPos);

            // Increases hole spawn as distance increases
            if (distanceTraveled % 100 == 0 && distanceTraveled > 0 && upDiff)
            {
                upDiff = false;
                yMin -= 0.2f;
                yMax -= 0.2f;
            } else if (distanceTraveled % 100 != 0 && !upDiff) { upDiff = true; }

            // Spawns holes
            if (ballCurrentPos > spawnHolePos.y - 4f)
            {
                spawnHolePos.y += Random.Range(yMin, yMax);
                spawnHolePos.x = Random.Range(-2.05f, 2.05f);
                Instantiate(hole, spawnHolePos, Quaternion.identity);
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
            ui.retryButton.gameObject.SetActive(true);
            ui.quitGame.gameObject.SetActive(true);
        } else
        {
            if (slowMo)
            {
                Time.timeScale = 0.05f;
                Time.fixedDeltaTime = Time.timeScale * .02f;
            } else { Time.timeScale = 1; }
            ui.retryButton.gameObject.SetActive(false);
            ui.quitGame.gameObject.SetActive(false);

        }
    }

    public void EndPowerUp()
    {
        grabbedPowerUp = false;
        grabbedPowerUpTime = 4f;
        ui.powerUpText.enabled = false;
    }

    public void Retry()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("HoleInOneHundred");
    }

    public void ResetScore()
    {
        PlayerPrefs.DeleteKey("HighScore");
        ui.highScore.text = "High Score\n0";
    }

    public void GameOver()
    {
        gameOver = true;
        gameOverScreen.Setup();
    }

    public void Quit()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu");
    }
}
