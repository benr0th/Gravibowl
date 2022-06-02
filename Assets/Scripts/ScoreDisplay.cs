using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] ScoreManager scoreManager;
    [SerializeField] GameObject scoreBoard;
    [SerializeField] public GameObject[] frameText;
    //public TextMeshProUGUI[] frameBall1Text, frameBall2Text, frameScoreText;
    TextMeshProUGUI frameBall3Text;
    bool strike, spare, zeroFirst, zeroSecond;

    private void Awake()
    {
        
    }

    private void Start()
    {
        
    }

    private void Update()
    {

    }

    public void ScoreboardUpdate()
    {
        strike = scoreManager.gameScore[scoreManager.currentFrame - 1][0] == 10;
        spare = scoreManager.gameScore[scoreManager.currentFrame - 1][0] != 10 &&
            scoreManager.gameScore[scoreManager.currentFrame - 1][0] +
            scoreManager.gameScore[scoreManager.currentFrame - 1][1] == 10;
        zeroFirst = scoreManager.gameScore[scoreManager.currentFrame - 1][0] == 0;
        zeroSecond = scoreManager.gameScore[scoreManager.currentFrame - 1][1] == 0;

        if (strike)
            frameText[scoreManager.currentFrame - 2].transform.GetChild(0)
                .GetComponent<TextMeshProUGUI>().text = "X";
        else if (zeroFirst)
            frameText[scoreManager.currentFrame - 2].transform.GetChild(0)
                .GetComponent<TextMeshProUGUI>().text = "-";
        else
            frameText[scoreManager.currentFrame-2].transform.GetChild(0).GetComponent<TextMeshProUGUI>()
                .text = scoreManager.gameScore[scoreManager.currentFrame - 1][0].ToString();

        if (spare)
            frameText[scoreManager.currentFrame - 2].transform.GetChild(1)
                .GetComponent<TextMeshProUGUI>().text = "/";
        else if (strike)
            frameText[scoreManager.currentFrame - 2].transform.GetChild(1)
                .GetComponent<TextMeshProUGUI>().text = "";
        else if (zeroSecond)
            frameText[scoreManager.currentFrame - 2].transform.GetChild(1)
                .GetComponent<TextMeshProUGUI>().text = "-";
        else
            frameText[scoreManager.currentFrame-2].transform.GetChild(1).GetComponent<TextMeshProUGUI>()
                .text = scoreManager.gameScore[scoreManager.currentFrame - 1][1].ToString();
        
        // final frame? 

        if (!strike && !spare)
            frameText[scoreManager.currentFrame-2].transform.GetChild(2).GetComponent<TextMeshProUGUI>()
                .text = scoreManager.gameScore[scoreManager.currentFrame - 1][2].ToString();
    }
}
