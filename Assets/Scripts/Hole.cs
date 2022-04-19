using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Hole : MonoBehaviour
{
    public float maxSpeedForHole = 0.2f;

    UIController ui;
    GameManager GameManager;

    private void Awake()
    {
        ui = GameObject.Find("UI").GetComponent<UIController>();
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ball") && other.GetComponent<Rigidbody2D>().velocity.y < maxSpeedForHole)
        {
            Destroy(other.gameObject);
            GameManager.GameOver();
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}