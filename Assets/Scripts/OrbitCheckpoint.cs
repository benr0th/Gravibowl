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
            GameManager.checkpointHits++;
    }
}
