using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] ShipControl ship;
    [SerializeField] ScoreDisplay scoreDisplay;
    CPUPlayer cpu;
    GameManager GameManager;
    public PinManager[] pinManager = new PinManager[10];
    public GameObject[] pins;
    public Dictionary<int, int[]>[] gameScore = new Dictionary<int, int[]>[2];
    public Player[] playerClass = new Player[2];
    Vector3[] originalPinPos = new Vector3[10];
    public int pinScore, currentFrame, frameBall, frameScore, 
        frameBall1Score, frameBall2Score, frameBall3Score, player;
    public bool finalFrame, switchedPlayer, twoPlayer;

    [System.Serializable]
    public class Player
    {
        public Player(int id) => Id = id;
        public int Id { get; private set; }
        public int strikes, pinScore;
        public bool isSpare, isStrike;
    }

    private void Awake()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        pins = GameObject.FindGameObjectsWithTag("Pin");
        cpu = ship.GetComponent<CPUPlayer>();
        for (int i = 0; i < pins.Length; i++)
            pinManager[i] = pins[i].GetComponent<PinManager>();
    }

    private void Start()
    {
        Dictionary<int, int[]> gameScore1 = new();
        Dictionary<int, int[]> gameScore2 = new();
        gameScore[0] = gameScore1;
        gameScore[1] = gameScore2;
        // Store original pin pos for lane reset
        for (int i = 0; i < pins.Length; i++)
            originalPinPos[i] = pins[i].transform.position;
        // Set initial values for new game
        currentFrame = 1;
        frameBall = 0;
        // Initialize players
        playerClass[0] = new Player(0);
        playerClass[1] = new Player(1);
        // Check if playing two player mode
        twoPlayer = PlayerPrefs.GetInt("TwoPlayer") == 1;
        // Check for left-handed mode
        if (PlayerPrefs.GetInt("LeftHandOn") == 1)
            for (int i = 0; i < scoreDisplay.scoreMoveButton.Length; i++)
                scoreDisplay.scoreMoveButton[i].transform.position =
                    new Vector3(-scoreDisplay.scoreMoveButton[i].transform.position.x,
                    scoreDisplay.scoreMoveButton[i].transform.position.y);
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
        if (!playerClass[player].isSpare && !playerClass[player].isStrike && !finalFrame)
            ScoreNoBonus();
        else if (playerClass[player].isStrike && !finalFrame)
            ScoreStrikeBonus();
        else if (playerClass[player].isSpare && !finalFrame)
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
                        gameScore[player].Add(currentFrame, new int[] { frameBall1Score, frameBall2Score, playerClass[player].pinScore });
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
                        playerClass[player].pinScore += frameBall1Score + frameBall2Score;
                        gameScore[player][currentFrame][1] = frameBall2Score;
                        gameScore[player][currentFrame][2] = playerClass[player].pinScore;
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
                        if (playerClass[player].strikes == 3)
                            StrikeHandler();
                        break;
                    default:
                        frameBall1Score = frameScore;
                        if (playerClass[player].strikes == 2)
                            StrikeHandler();
                        gameScore[player].Add(currentFrame, new int[] { frameBall1Score, frameBall2Score, playerClass[player].pinScore });
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
                        playerClass[player].pinScore += frameBall1Score + frameBall2Score;
                        gameScore[player][currentFrame][1] = frameBall2Score;
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
                        if (playerClass[player].isSpare)
                            SpareScore();
                        GotStrike();
                        if (playerClass[player].strikes == 2)
                            StrikeHandler();
                        break;
                    default:
                        if (playerClass[player].isSpare)
                            SpareScore();
                        frameBall1Score = frameScore;
                        if (playerClass[player].strikes == 2)
                            StrikeHandler();
                        gameScore[player].Add(currentFrame, new int[] { frameBall1Score, frameBall2Score,
                            frameBall3Score, playerClass[player].pinScore });
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
                        if (gameScore[player][currentFrame - 1][0] != 10 || frameBall1Score == 10)
                        {
                            if (frameBall1Score == 10)
                                frameBall2Score = frameScore;
                            else
                            {
                                frameBall2Score = frameScore - frameBall1Score;
                                playerClass[player].pinScore += frameBall1Score + frameBall2Score;
                            }
                        }
                        StrikeHandler();
                        if (gameScore[player][currentFrame - 1][0] == 10 && frameBall1Score != 10)
                            playerClass[player].pinScore += frameBall1Score + frameBall2Score;
                        gameScore[player][currentFrame][1] = frameBall2Score;
                        gameScore[player][currentFrame][3] = playerClass[player].pinScore;
                        break;
                }
                break;
            case 3:
                if (frameBall2Score == 10 || playerClass[player].isSpare)
                    frameBall3Score = frameScore;
                else
                    frameBall3Score = frameScore - frameBall2Score;
                playerClass[player].pinScore += frameBall3Score + frameBall2Score + frameBall1Score;
                gameScore[player][currentFrame][2] = frameBall3Score;
                gameScore[player][currentFrame][3] = playerClass[player].pinScore;
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
        playerClass[player].isStrike = true;
        frameBall1Score = frameScore;
        if (finalFrame && frameBall == 1)
            gameScore[player].Add(currentFrame, new int[] { frameBall1Score, frameBall2Score,
                            frameBall3Score, playerClass[player].pinScore });
        else if (finalFrame && frameBall == 2)
        {
            frameBall2Score = frameScore;
            gameScore[player][currentFrame][1] = frameBall2Score;
        }
        if (finalFrame)
            scoreDisplay.StrikeScoreboard(frameBall - 1);
        else
        {
            if (currentFrame == 1 || gameScore[player][currentFrame - 1][0] + gameScore[player][currentFrame - 1][1] != 10
                || gameScore[player][currentFrame - 1][0] == 10)
                gameScore[player].Add(currentFrame, new int[] { frameBall1Score, frameBall2Score, playerClass[player].pinScore });
            scoreDisplay.StrikeScoreboard(0);
            playerClass[player].strikes++;
        }
        if (twoPlayer)
        {
            ship.enabled = false;
            Invoke(nameof(PinReset), 3.5f);
        }
        else
            PinReset();
    }

    void GotSpare()
    {
        playerClass[player].isSpare = true;
        frameBall2Score = frameScore - frameBall1Score;
        if (finalFrame && frameBall == 2)
        {
            //gameScore.Add(currentFrame, new int[] { frameBall1Score, frameBall2Score,
            //                frameBall3Score, pinScore });
            gameScore[player][currentFrame][1] = frameBall2Score;

        }
        if (playerClass[player].strikes < 1 && !finalFrame)
            gameScore[player][currentFrame][1] = frameBall2Score;
        
    }

    void SingleStrike()
    {
        if (finalFrame)
            if (frameBall == 1) frameBall2Score = 0;
            else
                if (frameBall1Score != 10 && !playerClass[player].isSpare)
                {
                    frameBall2Score = frameScore - frameBall1Score;
                    //pinScore += frameBall1Score + frameBall2Score;
                }
        if (!finalFrame)
            frameBall2Score = frameScore - frameBall1Score;
        playerClass[player].pinScore += 10 + frameBall1Score + frameBall2Score;
        gameScore[player][currentFrame - 1][2] = playerClass[player].pinScore;
        if (!playerClass[player].isSpare && !finalFrame)
            playerClass[player].pinScore += frameBall1Score + frameBall2Score;
        if (!finalFrame)
        {
            //gameScore.Add(currentFrame, new int[] { frameBall1Score, frameBall2Score, pinScore });
            gameScore[player][currentFrame][1] = frameBall2Score;
            gameScore[player][currentFrame][2] = playerClass[player].pinScore;
        }

        playerClass[player].isStrike = false;
        playerClass[player].strikes--;
        scoreDisplay.ScoreboardWrite(2, 2, 1, 2);
    }

    void DoubleStrike()
    {
        playerClass[player].pinScore += 10 + 10 + frameBall1Score;
        gameScore[player][currentFrame - 2][2] = playerClass[player].pinScore;
        if (frameBall == 2)
            SingleStrike();
        else
            playerClass[player].strikes--;
        scoreDisplay.ScoreboardWrite(3, 2, 2, 2);
    }

    void TripleStrike()
    {
        playerClass[player].pinScore += 30;
        gameScore[player][currentFrame - 3][2] = playerClass[player].pinScore;
        if (!playerClass[player].isStrike)
            DoubleStrike();
        else
            playerClass[player].strikes--;
        scoreDisplay.ScoreboardWrite(4, 2, 3, 2);
    }

    void StrikeHandler()
    {
        switch (playerClass[player].strikes)
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
        playerClass[player].isSpare = false;
        frameBall1Score = frameScore;
        playerClass[player].pinScore += 10 + frameBall1Score;
        gameScore[player][currentFrame - 1][2] = playerClass[player].pinScore;
        if (!finalFrame)
            gameScore[player].Add(currentFrame, new int[] { frameBall1Score, frameBall2Score, playerClass[player].pinScore });
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
                SwitchPlayer();
            else
                currentFrame++;
        } 
        frameScore = 0;
        ship.enabled = true;
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
            {
                ship.enabled = false;
                Invoke(nameof(PinReset), 3.5f);
            }
            else
                PinReset();
        }
        else if (finalFrame)
        {
            if (frameBall == 2)
            {
                if (!playerClass[player].isSpare && frameBall1Score != 10)
                {
                    if (twoPlayer)
                        TwoPlayerEnd();
                    else
                        GameManager.GameOver();
                }
                else if (playerClass[player].isSpare)
                    PinReset();
            }
            if (frameBall == 3)
            {
                if (twoPlayer)
                    TwoPlayerEnd();
                else
                    GameManager.GameOver();
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

    void SwitchPlayer()
    {
        switchedPlayer = !switchedPlayer;
        player = switchedPlayer ? 1 : 0;
        scoreDisplay.SwitchScoreboard();
        scoreDisplay.currentPlayerText[player].text = $"Player {player + 1}";
        cpu.fallenPins = 0;
        if (!switchedPlayer)
            currentFrame++;
    }

    void TwoPlayerEnd()
    {
        if (switchedPlayer)
            GameManager.GameOver();
        else
            SwitchPlayer();
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
