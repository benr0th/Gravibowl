using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Linq;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] ShipControl ship;
    [SerializeField] GameObject[] tutMessages;
    GameManager GameManager;
    bool tutStarted, tutEnded;
    public int tutIndex;
    float waitTime = 1.1f;

    private void Awake()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void Update()
    {
        if ((PlayerPrefs.GetInt("TimesPlayed") == 1 | tutStarted) & !tutEnded)
            Tutorial();
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
            if (waitTime <= 0)
                tutIndex++;
            else
                waitTime -= Time.deltaTime;
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
                tutStarted = false;
            }
        }
    }

    public void TutButton()
    {
        tutIndex = 0;
        waitTime = 1.1f;
        tutEnded = false;
        tutStarted = true;
    }

    /* tutorial text page
 
    Press and Hold the screen to attract to the nearby planet!

    Keep holding to slingshot around the planet!

    Let go when you get a good shot!

     */
}
