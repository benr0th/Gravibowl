using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;

public class Hole : MonoBehaviour
{
    [SerializeField] float maxSpeedForHole;
    public bool isDead;

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
            isDead = true;
            other.gameObject.SetActive(false);
            GameManager.EndPowerUp();
            GameManager.GameOver();
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}