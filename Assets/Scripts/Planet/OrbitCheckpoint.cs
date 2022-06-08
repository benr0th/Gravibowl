using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitCheckpoint : MonoBehaviour
{
    GameManager GameManager;
    ShipControl ship;

    private void Awake()
    {
        ship = GameObject.Find("Ship").GetComponent<ShipControl>();
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ship"))
        {
            GameManager.canStopTouching = true;
            GameManager.checkpointHits = 1;
            StartCoroutine(ExitOrbit());
        }
    }

    IEnumerator ExitOrbit()
    {
        if (ship.stoppedTouching)
            GameManager.exitOrbit = true;
        yield return new WaitForSeconds(0.3f);
        GameManager.exitOrbit = false;
        ship.isTouching = false;
    }
}
