using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class Hole : MonoBehaviour
{
    [SerializeField] float maxSpeedForHole;
    [SerializeField] Vector2 launchPower;
    [SerializeField] Magnet magnet;
    GameManager GameManager;
    BallControl ball;

    float shrinkSpeed;

    private void Awake()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        ball = GameObject.Find("Ball").GetComponent<BallControl>();
    }
    /* old gameplay style
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ball") && other.GetComponent<Rigidbody2D>().velocity.y < maxSpeedForHole)
        {
            other.gameObject.SetActive(false);
            GameManager.EndPowerUp();
            GameManager.GameOver();
        }
    }
    */
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ball") && other.GetComponent<Rigidbody2D>().velocity.y < maxSpeedForHole
            && magnet.isMagnetized)
        {
            GameManager.inHole = true;
            StartCoroutine(Launch());
            magnet.isMagnetized = false;
        }
    }

    IEnumerator Launch()
    {
        ball.rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(1f);
        ball.rb.AddForce(new Vector2(Random.Range(-1f, 1f), launchPower.y), ForceMode2D.Impulse);
        GameManager.inHole = false;
    }
}