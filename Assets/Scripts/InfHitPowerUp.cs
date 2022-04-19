using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfHitPowerUp : MonoBehaviour
{
    public BallControl ball;
    GameManager GameManager;

    private void Awake()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ball"))
        {
            Destroy(gameObject);
            GameManager.grabbedPowerUp = true;
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
        Time.timeScale = 1;
    }
}
