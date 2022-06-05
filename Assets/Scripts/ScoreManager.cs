using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] ShipControl ship;
    [SerializeField] ScoreDisplay scoreDisplay;
    GameManager GameManager;
    PinManager[] pinManager = new PinManager[10];
    public GameObject[] pins;
    public Dictionary<int, int[]> gameScore = new();
    public Dictionary<int, int[]> gameScore2 = new();
    public Dictionary<int, int[]>[] gameScores = new Dictionary<int, int[]>[2];
    Vector3[] originalPinPos = new Vector3[10];
    int strikes;
    public int pinScore, currentFrame, frameBall, frameScore, 
        frameBall1Score, frameBall2Score, frameBall3Score, player;
    bool isStrike;
    public bool finalFrame, isSpare, switchPlayer, twoPlayer;

    private void Awake()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        pins = GameObject.FindGameObjectsWithTag("Pin");
        for (int i = 0; i < pins.Length; i++)
            pinManager[i] = pins[i].GetComponent<PinManager>();
    }

    private void Start()
    {
        gameScore = new();
        gameScore2 = new();
        gameScores[0] = gameScore;
        gameScores[1] = gameScore2;
        // Store original pin pos for lane reset
        for (int i = 0; i < pins.Length; i++)
            originalPinPos[i] = pins[i].transform.position;
        // Set initial values for new game
        currentFrame = 1;
        frameBall = 0;
        pinScore = 0;
        strikes = 0;
        // Check if playing two player mode
        twoPlayer = PlayerPrefs.GetInt("TwoPlayer") == 1;
    }

    private void Update()
    {
        finalFrame = currentFrame == 10;
    }

    void ScoreHandler()
    {
        // If any pin has moved, increase score - ignore pins that already moved
        for (int i = 0; i < pins.Length; i++)
            if (pins[i].transform.position != originalPinPos[i] && !pinManager[i].pinFallen)
            {
                frameScore++;
                pinManager[i].pinFallen = true;
            }
        if (!isSpare && !isStrike && !finalFrame)
            ScoreNoBonus();
        else if (isStrike && !finalFrame)
            ScoreStrikeBonus();
        else if (isSpare && !finalFrame)
            ScoreSpareBonus();
        else if (finalFrame)
            ScoreFinalFrame();

        /* Debug score display
        if (finalFrame)
            Debug.Log($"frame: {currentFrame}, ball1: {frameBall1Score}, ball2: {frameBall2Score}, ball3: {frameBall3Score}\n" +
            $"frameScore: {frameScore}, total: {pinScore}");
        else
            Debug.Log($"frame: {currentFrame}, ball1: {frameBall1Score}, ball2: {frameBall2Score}\n" +
                $"frameScore: {frameScore}, total: {pinScore}");
        */
    }

    void ScoreNoBonus()
    {
        switch (frameBall)
        {
            case 1:
                switch (frameScore)
                {
                    case 10:
                        GotStrike();
                        break;
                    default:
                        frameBall1Score = frameScore;
                        gameScore.Add(currentFrame, new int[] { frameBall1Score, frameBall2Score, pinScore });
                        break;
                }
                break;
            case 2:
                switch (frameScore)
                {
                    case 10:
                        GotSpare();
                        break;
                    default:
                        frameBall2Score = frameScore - frameBall1Score;
                        pinScore += frameBall1Score + frameBall2Score;
                        gameScore[currentFrame][1] = frameBall2Score;
                        gameScore[currentFrame][2] = pinScore;
                        break;
                }
                break;
        }
    }

    void ScoreStrikeBonus()
    {
        switch (frameBall)
        {
            case 1:
                switch (frameScore)
                {
                    case 10:
                        GotStrike();
                        if (strikes == 3)
                            StrikeHandler();
                        break;
                    default:
                        frameBall1Score = frameScore;
                        if (strikes == 2)
                            StrikeHandler();
                        gameScore.Add(currentFrame, new int[] { frameBall1Score, frameBall2Score, pinScore });
                        break;
                }
                break;
            case 2:
                switch (frameScore)
                {
                    case 10:
                        GotSpare();
                        StrikeHandler();
                        break;
                    default:
                        StrikeHandler();
                        break;
                }
                break;
        }
    }

    void ScoreSpareBonus()
    {
        switch (frameBall)
        {
            case 1:
                switch (frameScore)
                {
                    case 10:
                        SpareScore();
                        GotStrike();
                        break;
                    default:
                        SpareScore();
                        break;
                }
                break;
            case 2:
                switch (frameScore)
                {
                    case 10:
                        GotSpare();
                        break;
                    default:
                        frameBall2Score = frameScore - frameBall1Score;
                        pinScore += frameBall1Score + frameBall2Score;
                        gameScore[currentFrame][1] = frameBall2Score;
                        break;
                }
                break;
        }
    }

    void ScoreFinalFrame()
    {
        switch (frameBall)
        {
            case 1:
                switch (frameScore)
                {
                    case 10:
                        if (isSpare)
                            SpareScore();
                        GotStrike();
                        if (strikes == 2)
                            StrikeHandler();
                        break;
                    default:
                        if (isSpare)
                            SpareScore();
                        frameBall1Score = frameScore;
                        if (strikes == 2)
                            StrikeHandler();
                        gameScore.Add(currentFrame, new int[] { frameBall1Score, frameBall2Score,
                            frameBall3Score, pinScore });
                        break;
                }
                break;
            case 2:
                switch (frameScore)
                {
                    case 10:
                        if (frameBall1Score == 10) GotStrike();
                        else GotSpare();
                        StrikeHandler();
                        break;
                    default:
                        if (gameScore[currentFrame - 1][0] != 10 || frameBall1Score == 10)
                        {
                            if (frameBall1Score == 10)
                                frameBall2Score = frameScore;
                            else
                            {
                                frameBall2Score = frameScore - frameBall1Score;
                                pinScore += frameBall1Score + frameBall2Score;
                            }
                        }
                        StrikeHandler();
                        if (gameScore[currentFrame - 1][0] == 10 && frameBall1Score != 10)
                            pinScore += frameBall1Score + frameBall2Score;
                        gameScore[currentFrame][1] = frameBall2Score;
                        gameScore[currentFrame][3] = pinScore;
                        break;
                }
                break;
            case 3:
                if (frameBall2Score == 10 || isSpare)
                    frameBall3Score = frameScore;
                else
                    frameBall3Score = frameScore - frameBall2Score;
                pinScore += frameBall3Score + frameBall2Score + frameBall1Score;
                gameScore[currentFrame][2] = frameBall3Score;
                gameScore[currentFrame][3] = pinScore;
                if (frameBall3Score == 10 && (frameBall2Score == 10 || frameBall1Score + frameBall2Score == 10))
                {
                    scoreDisplay.StrikeScoreboard(2);
                    scoreDisplay.ScoreboardWrite(1, 3, 0, 3);
                }
                break;
               
        }
    }

    void GotStrike()
    {
        isStrike = true;
        frameBall1Score = frameScore;
        if (finalFrame && frameBall == 1)
            gameScore.Add(currentFrame, new int[] { frameBall1Score, frameBall2Score,
                            frameBall3Score, pinScore });
        else if (finalFrame && frameBall == 2)
        {
            frameBall2Score = frameScore;
            gameScore[currentFrame][1] = frameBall2Score;
        }
        if (finalFrame)
            scoreDisplay.StrikeScoreboard(frameBall - 1);
        else
        {
            if (currentFrame == 1 || gameScore[currentFrame - 1][0] + gameScore[currentFrame - 1][1] != 10
                || gameScore[currentFrame - 1][0] == 10)
                gameScore.Add(currentFrame, new int[] { frameBall1Score, frameBall2Score, pinScore });
            scoreDisplay.StrikeScoreboard(0);
            strikes++;
        }
        PinReset();
    }

    void GotSpare()
    {
        isSpare = true;
        frameBall2Score = frameScore - frameBall1Score;
        if (finalFrame && frameBall == 2)
        {
            //gameScore.Add(currentFrame, new int[] { frameBall1Score, frameBall2Score,
            //                frameBall3Score, pinScore });
            gameScore[currentFrame][1] = frameBall2Score;

        }
        if (strikes < 1 && !finalFrame)
            gameScore[currentFrame][1] = frameBall2Score;
        
    }

    void SingleStrike()
    {
        if (finalFrame)
            if (frameBall == 1) frameBall2Score = 0;
            else
                if (frameBall1Score != 10 && !isSpare)
                {
                    frameBall2Score = frameScore - frameBall1Score;
                    //pinScore += frameBall1Score + frameBall2Score;
                }
        if (!finalFrame)
            frameBall2Score = frameScore - frameBall1Score;
        pinScore += 10 + frameBall1Score + frameBall2Score;
        gameScore[currentFrame - 1][2] = pinScore;
        if (!isSpare && !finalFrame)
            pinScore += frameBall1Score + frameBall2Score;
        if (!finalFrame)
        {
            //gameScore.Add(currentFrame, new int[] { frameBall1Score, frameBall2Score, pinScore });
            gameScore[currentFrame][1] = frameBall2Score;
            gameScore[currentFrame][2] = pinScore;
        }

        isStrike = false;
        strikes--;
        scoreDisplay.ScoreboardWrite(2, 2, 1, 2);
    }

    void DoubleStrike()
    {
        pinScore += 10 + 10 + frameBall1Score;
        gameScore[currentFrame - 2][2] = pinScore;
        if (frameBall == 2)
            SingleStrike();
        else
            strikes--;
        scoreDisplay.ScoreboardWrite(3, 2, 2, 2);
    }

    void TripleStrike()
    {
        pinScore += 30;
        gameScore[currentFrame - 3][2] = pinScore;
        if (!isStrike)
            DoubleStrike();
        else
            strikes--;
        scoreDisplay.ScoreboardWrite(4, 2, 3, 2);
    }

    void StrikeHandler()
    {
        switch (strikes)
        {
            case 1:
                SingleStrike();
                break;
            case 2:
                DoubleStrike();
                break;
            case 3:
                TripleStrike();
                break;
            default:
                break;
        }
    }

    void SpareScore()
    {
        isSpare = false;
        frameBall1Score = frameScore;
        pinScore += 10 + frameBall1Score;
        gameScore[currentFrame - 1][2] = pinScore;
        if (!finalFrame)
            gameScore.Add(currentFrame, new int[] { frameBall1Score, frameBall2Score, pinScore });
        scoreDisplay.ScoreboardWrite(2, 2, 1, 2);
    }

    void PinReset()
    {
        for (int i = 0; i < pins.Length; i++)
        {
            pins[i].SetActive(true);
            pins[i].transform.position = originalPinPos[i];
            pins[i].GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            pins[i].GetComponent<Rigidbody2D>().angularVelocity = 0;
            pins[i].transform.up = Vector2.up;
            pinManager[i].pinFallen = false;
        }
        if (!finalFrame)
        {
            frameBall = 0;
            frameBall1Score = 0;
            frameBall2Score = 0;
            if (twoPlayer)
            {
                switchPlayer = !switchPlayer;
                player = switchPlayer ? 1 : 0;
                scoreDisplay.currentPlayerText.text = $"Player {player + 1}";
                if (!switchPlayer)
                    currentFrame++;
            }
            else
                currentFrame++;
        } 
        frameScore = 0;
        
        // Planet spawns on random side each frame, for more interesting gameplay
        Destroy(GameObject.FindGameObjectWithTag("Planet"));
        GameManager.InitPlanet();
    }

    void ShipReset()
    {
        ship.transform.position = GameManager.shipStartPos;
        ship.rb.velocity = Vector2.zero;
        ship.transform.up = Vector2.up;
    }

    public IEnumerator LaneReset()
    {
        yield return new WaitForSeconds(1.7f);
        GameManager.canStopTouching = false;
        ShipReset();
        frameBall++;
        ScoreHandler();
        for (int i = 0; i < pins.Length; i++)
            if (pinManager[i].pinFallen)
                pins[i].SetActive(false);
        scoreDisplay.ScoreboardUpdate();
        if (frameBall == 2 && !finalFrame)
        {
            if (twoPlayer)
                Invoke(nameof(PinReset), 3);
            else
                PinReset();
        }
        else if (finalFrame)
        {
            if (frameBall == 2)
            {
                if (!isSpare && frameBall1Score != 10)
                {
                    //scoreDisplay.ScoreboardUpdate();
                    GameManager.GameOver();
                }
                else if (isSpare)
                    PinReset();
            }
            if (frameBall == 3)
            {
                GameManager.GameOver();
                //scoreDisplay.ScoreboardUpdate();
            }
        }

        /* Debug score display
        foreach (KeyValuePair<int, int[]> kvp in gameScore)
        {
            Debug.Log($"Frame = {kvp.Key}, Score = {kvp.Value[0]},{kvp.Value[1]},{kvp.Value[2]}");
            if (kvp.Key == 10)
                Debug.Log($"Frame = {kvp.Key}, Score = {kvp.Value[0]},{kvp.Value[1]},{kvp.Value[2]},{kvp.Value[3]}");
        }
        */
    }

    // Debug function - please remove when done!
    public void DebugAutoStrike()
    {
        for (int i = 0; i < pins.Length; i++)
        {
            pins[i].GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 30), ForceMode2D.Impulse);
        }
        ship.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 30), ForceMode2D.Impulse);
    }
}
