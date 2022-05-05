using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using DG.Tweening;

public class Hole : MonoBehaviour
{
    [SerializeField] float maxSpeedForHole;
    [SerializeField] public Vector2 launchPower;
    [SerializeField] Magnet magnet;
    AudioSource launchAudio;
    Camera cam;
    GameManager GameManager;
    BallControl ball;
    UIController ui;

    float shrinkSpeed;
    public float originalLaunchPower;
    bool hasLaunched;
    WaitForSecondsRealtime launchTimer = new WaitForSecondsRealtime(1f);

    private void Awake()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        ball = GameObject.Find("Ball").GetComponent<BallControl>();
        cam = GameObject.Find("MainCamera").GetComponent<Camera>();
        ui = GameObject.Find("UI").GetComponent<UIController>();
        launchAudio = GetComponent<AudioSource>();
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
    // old gameplay code -  && other.GetComponent<Rigidbody2D>().velocity.y < maxSpeedForHole
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ball")
            && magnet.isMagnetized)
        {
            GameManager.inHole = true;
            ball.rb.velocity = Vector2.zero;
            Debug.Log("In Hole");
            magnet.canMagnetize = false;
            ball.hasTarget = false;
            StartCoroutine(Launch());
            magnet.isMagnetized = false;
        }
    }

    IEnumerator Launch()
    {
        if (GameManager.superLaunchActive)
        {
            if (PlayerPrefs.GetInt("NoScreenShake") == 0)
                StartCoroutine(cam.GetComponent<ScreenShake>().Shaking());
            StartCoroutine(LaunchAudio());
        }
        yield return launchTimer;
        ball.rb.AddForce(new Vector2(Random.Range(-1f, 1f), launchPower.y), ForceMode2D.Impulse);
        //ui.distanceText.transform.DOScale(new Vector3(3, 3), 2f).SetLoops(2,LoopType.Yoyo);
        hasLaunched = true;
        GameManager.inHole = false;
        ball.isTouching = false;
        magnet.canMagnetize = true;
    }

    IEnumerator LaunchPowerChanger()
    {
        launchPower.y = 100;
        //Debug.Log("Ability active");
        yield return new WaitUntil(() => hasLaunched);
        launchPower.y = originalLaunchPower;
        hasLaunched = false;
        GameManager.superLaunchActive = false;
        //Debug.Log("No longer active");
    }

    IEnumerator LaunchAudio()
    {
        yield return new WaitForSeconds(0.01f);
        launchAudio.Play();
    }
}