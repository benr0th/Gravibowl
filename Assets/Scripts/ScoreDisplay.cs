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
    [SerializeField] Button scoreMoveButton;
    [SerializeField] Sprite upButton, downButton;
    public TextMeshProUGUI currentPlayerText;
    bool strike, spare, zeroFirst, zeroSecond, moved;
    int finalIndex;

    private void Start()
    {
        if (PlayerPrefs.GetInt("LeftHandOn") == 1)
            scoreMoveButton.transform.position = 
                new Vector3(-scoreMoveButton.transform.position.x, scoreMoveButton.transform.position.y);
    }

    public void ScoreboardUpdate()
    {
        if (scoreManager.finalFrame)
        {
            if (scoreManager.frameBall == 1 && scoreManager.frameBall1Score != 10)
                FrameBall1Score();
            if (scoreManager.frameBall == 2 && 
                (scoreManager.frameBall2Score != 10 || scoreManager.frameBall1Score == 0))
            {
                FrameBall2Score();
                if (scoreManager.frameBall1Score + scoreManager.frameBall2Score != 10 &&
                    scoreManager.frameBall1Score != 10)
                    ScoreboardWrite(1, 3, 0, 3);
            }
            if (scoreManager.frameBall == 3)
            {
                if (scoreManager.frameBall3Score != 10 || scoreManager.frameBall2Score == 0)
                    FrameBall3Score();
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

    IEnumerator ScoreboardAnim()
    {
        ScoreboardUp();
        yield return new WaitForSeconds(2.3f);
        if (scoreManager.finalFrame && scoreManager.frameBall >= 2)
        {
            if (scoreManager.frameBall == 2)
            {
                if (!scoreManager.isSpare && scoreManager.frameBall1Score != 10)
                    yield break;
                else if (scoreManager.isSpare)
                    ScoreboardDown();
            }
            if (scoreManager.frameBall == 3)
                yield break;
        }
        else
            ScoreboardDown();
    }

    void ScoreboardUp()
    {
        if (!moved)
            moved = !moved;
        scoreTransform.DOMoveY(-0.2f, 1f);
        scoreMoveButton.GetComponent<Image>().sprite = downButton;
    }
    void ScoreboardDown()
    {
        if (moved)
            moved = !moved;
        scoreTransform.DOMoveY(-5.7f, 1f);
        scoreMoveButton.GetComponent<Image>().sprite = upButton;
    }

    public void ScoreboardButton()
    {
        moved = !moved;
        if (moved)
            ScoreboardUp();
        if (!moved)
            ScoreboardDown();
    }
}
