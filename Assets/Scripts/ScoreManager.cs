using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] ShipControl ship;
    GameManager GameManager;
    GameObject[] pins;
    PinManager[] pinManager = new PinManager[10];
    Dictionary<int, int[]> gameScore = new();
    Vector3[] originalPinPos = new Vector3[10];
    int currentFrame, frameBall, pinScore, frameScore, 
        frameBall1Score, frameBall2Score, frameBall3Score,
        strikes;

    bool isStrike, isSpare;
    public bool finalFrame;

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
        // Store original pin pos for lane reset
        for (int i = 0; i < pins.Length; i++)
            originalPinPos[i] = pins[i].transform.position;
        // Set initial values for new game
        currentFrame = 1;
        frameBall = 0;
        pinScore = 0;
        strikes = 0;
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

        if (finalFrame)
            Debug.Log($"frame: {currentFrame}, ball1: {frameBall1Score}, ball2: {frameBall2Score}, ball3: {frameBall3Score}\n" +
            $"frameScore: {frameScore}, total: {pinScore}");
        else
            Debug.Log($"frame: {currentFrame}, ball1: {frameBall1Score}, ball2: {frameBall2Score}\n" +
                $"frameScore: {frameScore}, total: {pinScore}");
        
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
                        gameScore.Add(currentFrame, new int[] { frameBall1Score, frameBall2Score, pinScore });
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
                        break;
                    default:
                        frameBall1Score = frameScore;
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
        isSpare = false;
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
                        gameScore.Add(currentFrame, new int[] { frameBall1Score, frameBall2Score, pinScore });
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
                        GotStrike();
                        break;
                    default:
                        frameBall1Score = frameScore;
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
                        gameScore.Add(currentFrame, new int[] { frameBall1Score, frameBall2Score,
                            frameBall3Score, pinScore });
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
                
                break;
               
        }
    }

    void GotStrike()
    {
        isStrike = true;
        frameBall1Score = frameScore;
        if (finalFrame && frameBall == 2)
        {
            frameBall2Score = frameScore;
            gameScore.Add(currentFrame, new int[] { frameBall1Score, frameBall2Score,
                            frameBall3Score, pinScore });
        }
        if (!finalFrame)
        {
            gameScore.Add(currentFrame, new int[] { frameBall1Score, frameBall2Score, pinScore });
            strikes++;
        }
        PinReset();
    }

    void GotSpare()
    {
        isSpare = true;
        frameBall2Score = frameScore - frameBall1Score;
        if (finalFrame && frameBall == 2)
            gameScore.Add(currentFrame, new int[] { frameBall1Score, frameBall2Score,
                            frameBall3Score, pinScore });
        if (strikes < 1 && !finalFrame)
            gameScore.Add(currentFrame, new int[] { frameBall1Score, frameBall2Score, pinScore });
        
    }

    void SingleStrike()
    {
        if (finalFrame)
            if (frameBall == 1) frameBall2Score = 0;
            else
                if (frameBall1Score != 10 && !isSpare)
                {
                    frameBall2Score = frameScore - frameBall1Score;
                    pinScore += frameBall1Score + frameBall2Score;
                }
        else if (!finalFrame)
            frameBall2Score = frameScore - frameBall1Score;
        pinScore += 10 + frameBall1Score + frameBall2Score;
        gameScore[currentFrame - 1][2] = pinScore;
        if (!isSpare && !finalFrame)
            pinScore += frameBall1Score + frameBall2Score;
        if (!finalFrame)
            gameScore.Add(currentFrame, new int[] { frameBall1Score, frameBall2Score, pinScore });
        isStrike = false;
        strikes = 0;
    }

    void DoubleStrike()
    {
        pinScore += 10 + 10 + frameBall1Score;
        gameScore[currentFrame - 2][2] = pinScore;
        SingleStrike();
    }

    void TripleStrike()
    {
        pinScore += 30;
        gameScore[currentFrame - 3][2] = pinScore;
        DoubleStrike();
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
            case > 3:
                for (int i = 0; i < strikes - 3; i++)
                {
                    pinScore += 30;
                    gameScore[currentFrame - (strikes - i)][2] = pinScore;
                }
                TripleStrike();
                break;
            default:
                break;
        }
    }

    void SpareScore()
    {
        frameBall1Score = frameScore;
        pinScore += 10 + frameBall1Score;
        gameScore[currentFrame - 1][2] = pinScore;
    }

    void PinReset()
    {
        for (int i = 0; i < pins.Length; i++)
        {
            pins[i].transform.position = originalPinPos[i];
            pins[i].GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            pins[i].GetComponent<Rigidbody2D>().angularVelocity = 0;
            pins[i].transform.up = Vector2.up;
            pinManager[i].pinFallen = false;
        }
        if (!finalFrame)
        {
            currentFrame++;
            frameBall = 0;
            frameBall1Score = 0;
            frameBall2Score = 0;
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
        yield return new WaitForSeconds(2f);
        ShipReset();
        frameBall++;
        ScoreHandler();
        if (frameBall == 2 && !finalFrame)
            PinReset();
        else if (finalFrame)
        {
            if (frameBall == 2)
            {
                if (!isSpare && frameBall1Score != 10)
                    //GameManager.GameOver();
                    Debug.Log("game over");
                else if (isSpare)
                    PinReset();
            }
            if (frameBall == 3)
                //GameManager.GameOver();
                Debug.Log("game over");
        }


        foreach (KeyValuePair<int, int[]> kvp in gameScore)
        {
            Debug.Log($"Frame = {kvp.Key}, Score = {kvp.Value[0]},{kvp.Value[1]},{kvp.Value[2]}");
            if (kvp.Key == 10)
                Debug.Log($"Frame = {kvp.Key}, Score = {kvp.Value[0]},{kvp.Value[1]},{kvp.Value[2]},{kvp.Value[3]}");
        }
       
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
