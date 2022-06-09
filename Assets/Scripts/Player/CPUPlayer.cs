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

    private void Update()
    {
        if (scoreManager.switchedPlayer)
        {
            
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

            //else
            //    StartCoroutine(LetGoSecond());
        }
    }

    public IEnumerator LetGoFirst()
    {
        yield return new WaitForSeconds(Random.Range(1.17f, 1.22f));
        ship.isTouching = false;
        ship.stoppedTouching = true;
        hasLetGo = true;   
    }

    public IEnumerator LetGoSecond()
    {
        yield return new WaitForSeconds(0.5f);
        ship.GetComponentInChildren<BoxCollider2D>().enabled = true;
    }
}
