using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfHitPowerUp : MonoBehaviour
{
    BallControl ball;
    GameManager GameManager;

    private void Awake()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        ball = GameObject.Find("Ball").GetComponent<BallControl>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ball"))
        {
            Destroy(gameObject);
            GameManager.grabbedPowerUp = true;
            ball.inputEnabled = false;
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
        GameManager.slowMo = false;
        Time.timeScale = 1;
    }
}
