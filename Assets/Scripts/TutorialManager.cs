using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] ShipControl ship;
    [SerializeField] GameObject[] tutMessages;
    [SerializeField] GameObject helpScreen, tutButton;
    [SerializeField] public Button helpButton;
    [SerializeField] ScoreManager scoreManager;
    GameManager GameManager;
    public bool tutStarted, tutEnded, screenToggled, debugTut;
    public int tutIndex;

    private void Awake()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void Start()
    {
        if (PlayerPrefs.GetInt("LeftHandOn") == 1)
            helpButton.transform.position = new Vector3(-helpButton.transform.position.x, 
                helpButton.transform.position.y);
    }

    private void Update()
    {
        if ((PlayerPrefs.GetInt("TimesPlayed") == 1 | tutStarted | debugTut) & !tutEnded)
            Tutorial();

        if ((PlayerPrefs.GetInt("CPU") == 1 & scoreManager.switchedPlayer) 
            | GameManager.gameOver | GameManager.isPaused | ship.notAtStart)
            helpButton.enabled = false;
        else
            helpButton.enabled = true;
    }

    void Tutorial()
    {
        for (int i = 0; i < tutMessages.Length; i++)
        {
            if (i == tutIndex)
                tutMessages[i].SetActive(true);
            else
                tutMessages[i].SetActive(false);
        }

        
        if (tutIndex == 0)
        {
            if ((ship.touch.phase == TouchPhase.Moved | ship.touch.phase == TouchPhase.Stationary) 
                & !EventSystem.current.IsPointerOverGameObject(ship.touch.fingerId))
            {
                tutIndex++;
            }
        }
        else if (tutIndex == 1)
        {
            if (GameManager.tutRelease)
                tutIndex++;
        }
        else if (tutIndex == 2)
        {
            Time.timeScale = 0;
            if (ship.touch.phase == TouchPhase.Ended
                & !EventSystem.current.IsPointerOverGameObject(ship.touch.fingerId))
            {
                Time.timeScale = 1;
                tutMessages[tutIndex].SetActive(false);
                tutEnded = true;
                debugTut = false; // TEMPORARY!
                GameManager.tutRelease = false;
            }
        }
    }

    public void TutButton()
    {
        ToggleHelpScreen();
        tutIndex = 0;
        tutEnded = false;
        //tutStarted = true;
        debugTut = true; // TEMPORARY!
    }

    public void HelpScreen() => ToggleHelpScreen();

    void ToggleHelpScreen()
    {
        screenToggled = !screenToggled;
        helpScreen.SetActive(screenToggled);

        if (screenToggled)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;

        if (scoreManager.frameBall != 0)
            tutButton.SetActive(false);
        else
            tutButton.SetActive(true);
    }
}
