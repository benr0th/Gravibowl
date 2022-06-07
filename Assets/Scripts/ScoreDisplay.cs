using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] ScoreManager scoreManager;
    [SerializeField] GameObject[] scoreBoard;
    [SerializeField] public Button[] scoreMoveButton;
    [SerializeField] Sprite upButton, downButton;
    public FrameTextArray[] frameArray;
    public TextMeshProUGUI[] currentPlayerText;
    public bool moved, flipped;
    int alpha1 = 0;
    int alpha2 = 1;

    [System.Serializable]
    public class FrameTextArray
    {
        public GameObject[] frameText;
    }

    private void Start()
    {

    }

    public void ScoreboardUpdate()
    {
        if (scoreManager.finalFrame)
        {
            if (scoreManager.frameBall == 1 & scoreManager.frameBall1Score != 10)
                FrameBall1Score();
            if (scoreManager.frameBall == 2 & 
                (scoreManager.frameBall2Score != 10 | scoreManager.frameBall1Score == 0))
            {
                FrameBall2Score();
                if (scoreManager.frameBall1Score + scoreManager.frameBall2Score != 10 &
                    scoreManager.frameBall1Score != 10)
                    ScoreboardWrite(1, 3, 0, 3);
            }
            if (scoreManager.frameBall == 3)
            {
                if (scoreManager.frameBall3Score != 10 | scoreManager.frameBall2Score == 0)
                    FrameBall3Score();
                ScoreboardWrite(1, 3, 0, 3);
            }
        }
        else
        {
            if (scoreManager.frameBall == 1 & scoreManager.frameBall1Score != 10)
                FrameBall1Score();
            if (scoreManager.frameBall == 2)
            {
                FrameBall2Score();
                if (scoreManager.frameBall1Score + scoreManager.frameBall2Score != 10)
                    ScoreboardWrite(1, 2, 0, 2);
            }
        }
    }

    public void StrikeScoreboard(int childIndex)
    {
        StartCoroutine(ScoreboardAnim());
        TextWrite(1, childIndex, "X");
        if (!scoreManager.finalFrame)
            TextWrite(1, 1, "");
    }

    public void SpareScoreboard(int childIndex)
    {
        StartCoroutine(ScoreboardAnim());
        TextWrite(1, childIndex, "/");
    }

    public void ZeroScore(int childIndex)
    {
        StartCoroutine(ScoreboardAnim());
        TextWrite(1, childIndex, "-");
    }

    public void ScoreboardWrite(int offsetFrame, int childIndex, int offsetScore, int scoreIndex)
    {
        StartCoroutine(ScoreboardAnim());
        TextWrite(offsetFrame, childIndex, 
            scoreManager.gameScore[scoreManager.player][scoreManager.currentFrame - offsetScore][scoreIndex].ToString());
    }

    void FrameBall1Score()
    {
        if (scoreManager.frameBall1Score == 0)
            ZeroScore(0);
        else
            ScoreboardWrite(1, 0, 0, 0);
    }

    void FrameBall2Score()
    {
        if (scoreManager.frameBall1Score + scoreManager.frameBall2Score == 10
            & scoreManager.frameBall1Score != 10)
            SpareScoreboard(1);
        else if (scoreManager.frameBall2Score == 0)
            ZeroScore(1);
        else
            ScoreboardWrite(1, 1, 0, 1);
    }

    void FrameBall3Score()
    {
        if (scoreManager.frameBall2Score + scoreManager.frameBall3Score == 10
            & scoreManager.frameBall2Score != 10
            & scoreManager.frameBall1Score + scoreManager.frameBall2Score != 10)
            SpareScoreboard(2);
        else if (scoreManager.frameBall3Score == 0)
            ZeroScore(2);
        else
            ScoreboardWrite(1, 2, 0, 2);
    }

    private void TextWrite(int offset, int childIndex, string text)
    {
        frameArray[scoreManager.player].frameText[scoreManager.currentFrame - offset].transform.GetChild(childIndex)
                        .GetComponent<TextMeshProUGUI>().text = text;

    }

    IEnumerator ScoreboardAnim()
    {
        ScoreboardUp();
        yield return new WaitForSeconds(2.3f);
        if (scoreManager.finalFrame & scoreManager.frameBall >= 2)
        {
            if (scoreManager.frameBall == 2)
            {
                if (!scoreManager.playerClass[scoreManager.player].isSpare & scoreManager.frameBall1Score != 10)
                {
                    if (scoreManager.twoPlayer & !scoreManager.switchedPlayer)
                        ScoreboardDown();
                    else
                        yield break;
                }
                else if (scoreManager.playerClass[scoreManager.player].isSpare)
                    ScoreboardDown();
                else
                    ScoreboardDown();
            }
            if (scoreManager.frameBall == 3)
            {
                if (scoreManager.twoPlayer & !scoreManager.switchedPlayer)
                    ScoreboardDown();
                else
                    yield break;
            }
            
        }
        else
            ScoreboardDown();
    }

    void ScoreboardUp()
    {
        if (!moved)
            moved = !moved;
        scoreBoard[scoreManager.player].transform.DOMoveY(-0.2f, 1f);
        scoreMoveButton[scoreManager.player].GetComponent<Image>().sprite = downButton;
    }
    void ScoreboardDown()
    {
        if (moved)
            moved = !moved;
        scoreBoard[scoreManager.player].transform.DOMoveY(-5.7f, 1f);
        scoreMoveButton[scoreManager.player].GetComponent<Image>().sprite = upButton;
    }

    public void ScoreboardButton()
    {
        moved = !moved;
        if (moved)
            ScoreboardUp();
        if (!moved)
            ScoreboardDown();
    }

    public void SwitchScoreboard()
    {
        scoreBoard[0].GetComponent<CanvasGroup>().alpha = alpha1;
        scoreMoveButton[0].gameObject.SetActive(!scoreManager.switchedPlayer);
        scoreBoard[1].GetComponent<CanvasGroup>().alpha = alpha2;
        scoreMoveButton[1].gameObject.SetActive(scoreManager.switchedPlayer);
        (alpha1, alpha2) = (alpha2, alpha1);
    }
}
