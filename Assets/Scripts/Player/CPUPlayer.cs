using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPUPlayer : MonoBehaviour
{
    [SerializeField] ScoreManager scoreManager;
    [SerializeField] ShipControl ship;
    public int fallenPins;
    public bool hasLetGo;

    private void Awake()
    {
        
    }

    private void FixedUpdate()
    {
        if (!scoreManager.switchedPlayer) return;
        if (!hasLetGo)
        {
            ship.isTouching = true;
            ship.stoppedTouching = false;
            ship.thrustPrefab.SetActive(true);
            ship.thrustAudio.enabled = true;
        }
        else
        {
            ship.thrustPrefab.SetActive(false);
            ship.thrustAudio.enabled = false;
        }
    }

    public IEnumerator LetGoFirst()
    {
        //yield return new WaitUntil(() => ship.enabled = true);
        switch (SPrefs.GetInt("Difficulty"))
        {
            case 1:
                yield return new WaitForSeconds(Random.Range(1.9f, 2f));
                break;
            case 2:
                yield return new WaitForSeconds(Random.Range(1.4f, 1.53f));
                break;
            case 3:
                yield return new WaitForSeconds(Random.Range(1.15f, 1.2f));
                break;
        }
        ship.isTouching = false;
        ship.stoppedTouching = true;
        hasLetGo = true;   
    }

    public IEnumerator LetGoSecond()
    {
        //yield return new WaitUntil(() => ship.enabled = true);
        yield return new WaitForSeconds(0.5f);
        ship.GetComponentInChildren<BoxCollider2D>().enabled = true;
    }

    //IEnumerator CPUTouch()
    //{
    //    //yield return new WaitUntil(() => ship.enabled = true);
    //    ship.isTouching = true;
    //    ship.stoppedTouching = false;
    //    ship.thrustPrefab.SetActive(true);
    //    ship.thrustAudio.enabled = true;
    //}
}
