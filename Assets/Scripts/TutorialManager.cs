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

#if UNITY_WEBGL
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern string GetData(string key);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void SetData(string key, string value);
#endif

    private void Awake()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void Start()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        int.TryParse(GetData("LeftHandOn"), out int lefty);
        if (lefty == 1)
#else
        if (SPrefs.GetInt("LeftHandOn") == 1)
#endif
            helpButton.transform.position = new Vector3(-helpButton.transform.position.x, 
                helpButton.transform.position.y);
    }

    private void Update()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        int.TryParse(GetData("TimesPlayed"), out int timesplayed);
        if ((timesplayed == 1
#else
        if ((SPrefs.GetInt("TimesPlayed") == 1 
#endif
            | tutStarted) & !tutEnded)
            Tutorial();

        if ((SPrefs.GetInt("CPU") == 1 & scoreManager.switchedPlayer) 
            | GameManager.gameOver | GameManager.isPaused | ship.notAtStart)
            helpButton.enabled = false;
        else
            helpButton.enabled = true;
    }

    void Tutorial()
    {
        tutStarted = true;
        for (int i = 0; i < tutMessages.Length; i++)
        {
            if (i == tutIndex)
                tutMessages[i].SetActive(true);
            else
                tutMessages[i].SetActive(false);
        }

        
        if (tutIndex == 0)
        {
#if UNITY_IOS || UNITY_ANDROID
            if ((ship.touch.phase == TouchPhase.Moved | ship.touch.phase == TouchPhase.Stationary) 
                & !EventSystem.current.IsPointerOverGameObject(ship.touch.fingerId))
#else
            if (Input.GetMouseButton(0) &
                !EventSystem.current.IsPointerOverGameObject())
#endif
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
#if UNITY_IOS || UNITY_ANDROID
            if (ship.touch.phase == TouchPhase.Ended
                & !EventSystem.current.IsPointerOverGameObject(ship.touch.fingerId))
#else
            if (Input.GetMouseButtonUp(0) &
                !EventSystem.current.IsPointerOverGameObject())
#endif
            {
                Time.timeScale = 1;
                tutMessages[tutIndex].SetActive(false);
                tutEnded = true;
                GameManager.tutRelease = false;
            }
        }
    }

    public void TutButton()
    {
        ToggleHelpScreen();
        tutIndex = 0;
        tutEnded = false;
        tutStarted = true;
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
