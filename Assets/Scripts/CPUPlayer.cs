using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPUPlayer : MonoBehaviour
{
    [SerializeField] ScoreManager scoreManager;
    [SerializeField] ShipControl ship;
    public int fallenPins;
    bool hasLetGo;

    private void Awake()
    {
        
    }

    private void Update()
    {
        if (scoreManager.switchedPlayer)
        {
            for (int i = 0; i < scoreManager.pins.Length; i++)
                if (scoreManager.pinManager[i].pinFallen)
                    fallenPins++;

            if (!hasLetGo)
            {
                ship.isTouching = true;
                ship.stoppedTouching = false;
            }

            if (fallenPins == 0)
                StartCoroutine(LetGoFirstFrame());
        }
    }

    IEnumerator LetGoFirstFrame()
    {
        yield return new WaitForSeconds(Random.Range(1.06f, 1.15f));
        ship.isTouching = false;
        ship.stoppedTouching = true;
        hasLetGo = true;
    }
}
