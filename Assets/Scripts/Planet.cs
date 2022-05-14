using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using DG.Tweening;

public class Planet : MonoBehaviour
{
    [SerializeField] float maxSpeedForHole, gravForce;
    [SerializeField] public Vector2 launchPower;
    [SerializeField] Magnet magnet;
    AudioSource launchAudio;
    Camera cam;
    GameManager GameManager;
    ShipControl ship;
    UIController ui;
    public Rigidbody2D planetRb;

    float shrinkSpeed;
    public float originalLaunchPower, G;
    bool hasLaunched, doOrbit;
    public float soiRadius;
    WaitForSecondsRealtime launchTimer = new WaitForSecondsRealtime(1f);

    private void Awake()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        ship = GameObject.Find("Ship").GetComponent<ShipControl>();
        cam = GameObject.Find("MainCamera").GetComponent<Camera>();
        ui = GameObject.Find("UI").GetComponent<UIController>();
        launchAudio = GetComponent<AudioSource>();
        originalLaunchPower = launchPower.y;
    }

    // Creates a visual representation of the sphere of influence in the editor
    public void OnDrawGizmos() => Gizmos.DrawWireSphere(transform.position, soiRadius);


    private void Update()
    {
        //if (GameManager.superLaunchActive)
        //    StartCoroutine(LaunchPowerChanger());
    }

    private void FixedUpdate()
    {
        //Debug.Log($"vel={ship.rb.velocity.magnitude}");
        /*
        if (GameManager.checkpointHits > 2)
            Debug.Log($"orbit achieved!\nvel={ship.rb.velocity}\nvelMag={ship.rb.velocity.magnitude}" +
                $"\nPosVector={transform.position - ship.transform.position}" +
                $"\nPosFloat={(transform.position - ship.transform.position).magnitude}" +
                $"\nnormalized={(transform.position - ship.transform.position).normalized}");
        */
        float orbitalDistance = Vector3.Distance(transform.position, ship.transform.position);
        if (ship.isTouching && orbitalDistance < soiRadius && !GameManager.exitOrbit)
        {

            //Debug.Log($"orbiting {transform.position}");
            //Orbit();
            if (!GameManager.isOrbiting)
            {
                StartCoroutine(OrbitRoutine());
                StartCoroutine(OrbitSpeedChange());
            }


        }
        if (doOrbit)
        {
            Orbit();
            // Side faces planet, so force will be in correct direction
            if (transform.position.x >= 0)
                TurnTowardsRight();
            //ship.transform.right = transform.position - ship.transform.position;
            else
                TurnTowardsLeft();
            //ship.transform.right = (transform.position - ship.transform.position) * -1;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ship")
            && ship.isTouching)
        {
            //transform.position = Vector2.Lerp(ship.transform.position,
            //    (transform.position - ship.transform.position).normalized * 0.5f, 0.5f);
            ship.orbitVel = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        ship.orbitVel = false;
    }

    void Orbit()
    {
        // Gravity
        Vector2 direction = planetRb.position - ship.rb.position;
        float distanceSqr = direction.sqrMagnitude;
        float forceMagnitude = G * (planetRb.mass * ship.rb.mass) / distanceSqr;
        Vector2 force = direction.normalized * forceMagnitude;
        ship.rb.AddForce(force);
        // Centrifugal force
        /*
        float cForceMagnitude = ship.rb.mass * (ship.rb.velocity.sqrMagnitude / direction.magnitude);
        Vector2 cForce = -direction.normalized * cForceMagnitude;
        ship.rb.AddForce(cForce);
        */
        

    }

    void ShipOrbitVel()
    {
        ship.rb.velocity = transform.up.normalized * 7f;
    }

    void TurnTowardsRight()
    {
        Vector3 directionVector = (transform.position - ship.transform.position);
        Vector3 rotatedVector = Quaternion.Euler(0, 0, 90) * directionVector;
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, rotatedVector);
        ship.transform.rotation = Quaternion.RotateTowards(ship.transform.rotation, 
            targetRotation, 750 * Time.deltaTime);
    }

    void TurnTowardsLeft()
    {
        Vector3 directionVector = (transform.position - ship.transform.position) * -1;
        Vector3 rotatedVector = Quaternion.Euler(0, 0, 90) * directionVector;
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, rotatedVector);
        ship.transform.rotation = Quaternion.RotateTowards(ship.transform.rotation,
            targetRotation, 750 * Time.deltaTime);
    }

    IEnumerator OrbitRoutine()
    {
        //ship.transform.position = (transform.position - ship.transform.position).normalized * 0.5f;
        GameManager.isOrbiting = true;
        ship.isTouching = true;
        doOrbit = true;
        yield return new WaitUntil(() => GameManager.exitOrbit);
        ship.isTouching = false;
        doOrbit = false;
        GameManager.isOrbiting = false;
    }

    IEnumerator OrbitSpeedChange()
    {
        yield return new WaitForSeconds(0.3f);
        Time.timeScale = 0.4f;
        Time.fixedDeltaTime = Time.timeScale * .02f;
        yield return new WaitForSecondsRealtime(0.7f);
        Time.timeScale += (1f / 0.05f);
        Time.timeScale = Mathf.Clamp01(Time.timeScale);
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

    /* magnet gameplay (legacy)
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
    */

}