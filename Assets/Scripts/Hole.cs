using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class Hole : MonoBehaviour
{
    [SerializeField] float maxSpeedForHole;
    [SerializeField] public Vector2 launchPower;
    [SerializeField] Magnet magnet;
    Camera cam;
    GameManager GameManager;
    BallControl ball;

    float shrinkSpeed;
    public float originalLaunchPower;
    bool hasLaunched;

    private void Awake()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        ball = GameObject.Find("Ball").GetComponent<BallControl>();
        cam = GameObject.Find("MainCamera").GetComponent<Camera>();
        originalLaunchPower = launchPower.y;
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

    private void Update()
    {
        if (GameManager.superLaunchActive)
            StartCoroutine(LaunchPowerChanger());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ball") && other.GetComponent<Rigidbody2D>().velocity.y < maxSpeedForHole
            && magnet.isMagnetized)
        {
            GameManager.inHole = true;
            Debug.Log("In Hole");
            StartCoroutine(Launch());
            magnet.isMagnetized = false;
        }
    }

    IEnumerator Launch()
    {
        ball.rb.velocity = Vector2.zero;
        if (GameManager.superLaunchActive)
            StartCoroutine(cam.GetComponent<ScreenShake>().Shaking());
        yield return new WaitForSeconds(1f);
        ball.rb.AddForce(new Vector2(Random.Range(-1f, 1f), launchPower.y), ForceMode2D.Impulse);
        hasLaunched = true;
        GameManager.inHole = false;
        ball.isTouching = false;
    }

    //IEnumerator LaunchPowerChanger()
    //{
    //    float launchP = hole.GetComponentInChildren<Hole>().launchPower.y;
    //    bool launched = hole.GetComponentInChildren<Hole>().hasLaunched;
    //    launchP = 100;
    //    Debug.Log("launchP=" + launchP);
    //    yield return new WaitUntil(() => launched);
    //    launchP = hole.GetComponentInChildren<Hole>().originalLaunchPower;
    //    launched = false;
    //    Debug.Log("launchP=" + launchP);
    //}

    IEnumerator LaunchPowerChanger()
    {
        launchPower.y = 100;
        Debug.Log("Ability active");
        yield return new WaitUntil(() => hasLaunched);
        launchPower.y = originalLaunchPower;
        hasLaunched = false;
        GameManager.superLaunchActive = false;
        Debug.Log("No longer active");
    }
}