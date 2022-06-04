using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] ScoreManager scoreManager;
    [SerializeField] GameObject scoreBoard;
    [SerializeField] public GameObject[] frameText;
    [SerializeField] Transform scoreTransform;
    TextMeshProUGUI frameBall3Text;
    bool strike, spare, zeroFirst, zeroSecond;
    int finalIndex;

    public void ScoreboardUpdate()
    {
        if (scoreManager.finalFrame)
        {
            if (scoreManager.frameBall == 1 && scoreManager.frameBall1Score != 10)
                FrameBall1Score();
            if (scoreManager.frameBall == 2 && scoreManager.frameBall2Score != 10)
            {
                FrameBall2Score();
                if (scoreManager.frameBall1Score + scoreManager.frameBall2Score != 10 &&
                    scoreManager.frameBall1Score != 10)
                    ScoreboardWrite(1, 3, 0, 3);
            }
            if (scoreManager.frameBall == 3 && scoreManager.frameBall3Score != 10)
            {
                FrameBall3Score();
                if ((scoreManager.frameBall2Score + scoreManager.frameBall3Score != 10 ||
                    scoreManager.frameBall2Score + scoreManager.frameBall3Score == 10) &&
                    scoreManager.frameBall2Score != 10)
                    ScoreboardWrite(1, 3, 0, 3);
            }
        }
        else
        {
            if (scoreManager.frameBall == 1)
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
        scoreTransform.DOMove(new Vector3(0, -3.3f), 0.5f);
        TextWrite(1, childIndex, "X");
        if (!scoreManager.finalFrame)
            TextWrite(1, 1, "");
    }

    public void SpareScoreboard(int childIndex)
    {
        TextWrite(1, childIndex, "/");
    }

    public void ZeroScore(int childIndex)
    {
        TextWrite(1, childIndex, "-");
    }

    public void ScoreboardWrite(int offsetFrame, int childIndex, int offsetScore, int scoreIndex)
    {
        TextWrite(offsetFrame, childIndex, 
            scoreManager.gameScore[scoreManager.currentFrame - offsetScore][scoreIndex].ToString());
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
            && scoreManager.frameBall1Score != 10)
            SpareScoreboard(1);
        else if (scoreManager.frameBall2Score == 0)
            ZeroScore(1);
        else
            ScoreboardWrite(1, 1, 0, 1);
    }

    void FrameBall3Score()
    {
        if (scoreManager.frameBall2Score + scoreManager.frameBall3Score == 10
            && scoreManager.frameBall2Score != 10)
            SpareScoreboard(2);
        else if (scoreManager.frameBall3Score == 0)
            ZeroScore(2);
        else
            ScoreboardWrite(1, 2, 0, 2);
    }

    private void TextWrite(int offset, int childIndex, string text)
    {
        frameText[scoreManager.currentFrame - offset].transform.GetChild(childIndex)
                        .GetComponent<TextMeshProUGUI>().text = text;
    }

}
