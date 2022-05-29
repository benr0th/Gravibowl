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
    int currentFrame, frameBall, pinScore, frameScore, frameBall1Score, frameBall2Score,
        strikes;

    bool isStrike, isSpare;

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
        currentFrame = 1;
        frameBall = 0;
        pinScore = 0;
        strikes = 0;
    }

    void ScoreHandler()
    {
        for (int i = 0; i < pins.Length; i++)
            if (pins[i].transform.position != originalPinPos[i] && !pinManager[i].pinFallen)
            {
                frameScore++;
                pinManager[i].pinFallen = true;
            }
        #region ugly score code
        /*
        if (frameBall == 1) 
            frameBall1Score = frameScore;
        if (frameScore < 10 && frameBall == 2)
        {
            frameBall2Score = frameScore - frameBall1Score;
            pinScore += frameBall1Score + frameBall2Score;
            gameScore.Add(currentFrame, new int[] {frameBall1Score, frameBall2Score});
        }
        if (frameScore == 10 && frameBall < 2)
            Strike();
        else if (frameScore == 10 && frameBall == 2)
            Spare();
        */
        #endregion
        if (!isSpare && !isStrike || currentFrame == 10)
            ScoreNoBonus();
        else if (isStrike && currentFrame < 10)
            ScoreStrikeBonus();
        else if (isSpare && currentFrame < 10)
            ScoreSpareBonus();

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

    void GotStrike()
    {
        strikes++;
        isStrike = true;
        frameBall1Score = frameScore;
        gameScore.Add(currentFrame, new int[] { frameBall1Score, frameBall2Score, pinScore });
        PinReset();
    }

    void GotSpare()
    {
        isSpare = true;
        frameBall2Score = frameScore - frameBall1Score;
        if (strikes < 1)
            gameScore.Add(currentFrame, new int[] { frameBall1Score, frameBall2Score, pinScore });
    }

    void SingleStrike()
    {
        frameBall2Score = frameScore - frameBall1Score;
        pinScore += 10 + frameBall1Score + frameBall2Score;
        gameScore[currentFrame - 1][2] = pinScore;
        if (!isSpare)
            pinScore += frameBall1Score + frameBall2Score;
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
        if (currentFrame < 10)
            currentFrame++;
        frameBall = 0;
        frameBall1Score = 0;
        frameBall2Score = 0;
        frameScore = 0;
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
        if (frameBall == 2)
        {
            PinReset();
        }
        foreach (KeyValuePair<int, int[]> kvp in gameScore)
        {
            Debug.Log($"Frame = {kvp.Key}, Score = {kvp.Value[0]},{kvp.Value[1]},{kvp.Value[2]}");
        }
    }

    public void DebugAutoStrike()
    {
        for (int i = 0; i < pins.Length; i++)
        {
            pins[i].GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 10), ForceMode2D.Impulse);
        }
    }
}
