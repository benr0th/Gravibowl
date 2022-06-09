using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class ShipControl : MonoBehaviour
{
    public Rigidbody2D rb;
    [SerializeField] LineRenderer lr;
    [SerializeField] SkinManager skinManager;
    [SerializeField] MagnetGauge magnetGauge;
    [SerializeField] float power, maxDrag, moveSpeed;
    [SerializeField] ScoreManager scoreManager;
    [SerializeField] public GameObject thrustPrefab;
    public AudioSource thrustAudio;
    CPUPlayer cpu;
    GameManager GameManager;
    //public ParticleSystem hitEffect = null;

    bool notMoving, notMovingUp, launchButtonPressed;
    public float stoppedMoving, magnetSpeed, rotateSpeed, timePressed;
    public bool isTouching, notAtStart, ready, hasTarget, orbitVel, stoppedTouching, inputEnabled = true;

    Vector3 difference = Vector3.zero;
    Vector3 draggingPos, dragStartPos;
    public Vector3 targetPos;
    public Touch touch;

    private void Awake()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>(); 
        thrustAudio = GetComponent<AudioSource>();
        GetComponent<SpriteRenderer>().sprite = skinManager.GetSelectedSkin().sprite;
        rb = GetComponent<Rigidbody2D>();
        cpu = GetComponent<CPUPlayer>();
    }

    private void Start()
    {
        if (PlayerPrefs.GetInt("CPU") == 1)
            cpu.enabled = true;
    }

    private void FixedUpdate()
    {
        #region magnet (legacy)
        /*
        // Magnet ability
        if (hasTarget && Vector3.Distance(targetPos, transform.position) <= 3)
        {
            Vector2 targetDirection = (targetPos - transform.position).normalized;
            rb.velocity += targetDirection * magnetSpeed * Time.fixedDeltaTime;
            transform.DOMove(targetPos, magnetSpeed * Time.fixedDeltaTime);
            transform.position = Vector3.MoveTowards(transform.position, targetPos,
                magnetSpeed * Time.fixedDeltaTime);
        }
        */
        #endregion
        if (isTouching & !GameManager.exitOrbit & timePressed > 0.065f)
        {
            // Force of object
            //rb.AddRelativeForce(new Vector2(0, 4));
            GetComponent<ConstantForce2D>().relativeForce = new Vector2(0, 5);
            /*vel=(12.65, 4.45)
            velMag=13.40864
            PosVector=(0.07, -0.57, 0.00)
            PosFloat=0.5714426
            normalized=(0.12, -0.99, 0.00)
            */
            if (orbitVel)
                //    //rb.velocity = transform.up.normalized * 13.0058f;
                rb.velocity = transform.up.normalized * 6f;

        }
        else
            GetComponent<ConstantForce2D>().relativeForce = Vector2.zero;


    }

    private void Update()
    {
        notAtStart = transform.position.y != GameManager.shipStartPos.y;
        #region legacy logic
        // Logic for moving ball when not moving
        /*
        if (rb.velocity.magnitude <= 0.25f && !GameManager.inHole)
        {
            notMoving = true;
            rb.velocity = Vector2.zero;
            if (notAtStart && !GameManager.canHitAgain)
                stoppedMoving += Time.deltaTime;
        }
        else { notMoving = false; stoppedMoving = 0; }
        */

        // Prevents moving ball left and right infinitely
        /*
        if (rb.velocity.y <= 0.5f)
        {
            notMovingUp = true;
            stoppedMoving += Time.deltaTime;
            //rb.velocity = Vector2.zero;
        } else { stoppedMoving = 0f; notMovingUp = false; }
        */
        #endregion
        if (Input.touchCount > 0 & !GameManager.isPaused 
        & !EventSystem.current.IsPointerOverGameObject(touch.fingerId))
        {
            if (!scoreManager.switchedPlayer || PlayerPrefs.GetInt("CPU") == 0)
            {
                touch = Input.GetTouch(0);
                //stoppedTouching = false;
                // When touching
                if (!EventSystem.current.IsPointerOverGameObject(touch.fingerId)
                    & !stoppedTouching & !GameManager.gameOver & !GameManager.canHitAgain)
                {
                    if (touch.phase == TouchPhase.Moved | touch.phase == TouchPhase.Stationary)
                    {
                        isTouching = true;
                        timePressed += Time.deltaTime;
                        thrustPrefab.SetActive(true);
                        thrustAudio.enabled = true;
                    }
                    else
                    {
                        thrustPrefab.SetActive(false);
                        thrustAudio.enabled = false;
                    }
                }

                if (touch.phase == TouchPhase.Ended)
                {
                    if (GameManager.canStopTouching)
                        isTouching = false;
                    inputEnabled = true;
                    if (notAtStart)
                        stoppedTouching = true;
                }
            }
            #region drag&shoot mechanism (legacy)
            // Only move when ball is not moving, uses voodoo magic, do not touch or it will break
            /*
            if (notMoving)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    ready = true;
                }
            }
            else { ready = false; }
            */
            // Code for power up logic - GameManager.grabbedPowerUp && inputEnabled == true

            // Drag and shoot
            /*
            if (ready && !notAtStart || ready && GameManager.canHitAgain)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    DragStart();
                }

                if (touch.phase == TouchPhase.Moved)
                {
                    Dragging();
                }

                if (touch.phase == TouchPhase.Ended)
                {
                    DragRelease();
                }
            }
            */

            // Move left and right while moving
            /*
            if (!notMovingUp && stoppedMoving < 1f && touch.position.y < Screen.height / 1.2f)
            {
                if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    Move();
                }
            }
            */
            #endregion
        }
    }

    void Move()
    {
        Vector2 vel = rb.velocity;
        draggingPos = Camera.main.ScreenToWorldPoint(touch.position);
        draggingPos.z = 0f;
        difference = draggingPos - transform.position;
        vel.x = 0f;
        rb.velocity = vel;

        transform.position = Vector3.Lerp(transform.position,
                new Vector3(draggingPos.x, transform.position.y, 0f), 0.5f);

        /* Alternative moving methods
        transform.position = new Vector3(draggingPos.x, transform.position.y, transform.position.z);
        rb.transform.Translate(draggingPos.x * 0.01f, 0f, 0f);
        rb.MovePosition(new Vector2(draggingPos.x, transform.position.y));
        transform.position = Vector3.MoveTowards(transform.position,
                                new Vector3(draggingPos.x, transform.position.y, transform.position.z),
                                1f * Time.deltaTime);

        vel.x = difference.x * moveSpeed;
        rb.velocity = vel;
        */
    }

    public void LaunchButton()
    {
        GameManager.checkpointHits = 0;
        SceneManager.LoadSceneAsync("Game");
    }

    /* drag&shoot methods (legacy)
    void DragStart()
    {
        dragStartPos = Camera.main.ScreenToWorldPoint(touch.position);
        dragStartPos.z = 0f;
        lr.positionCount = 1;
        lr.SetPosition(0, dragStartPos);
    }

    void Dragging()
    {
        draggingPos = Camera.main.ScreenToWorldPoint(touch.position);
        draggingPos.z = 0f;
        lr.positionCount = 2;
        lr.SetPosition(1, draggingPos);
    }

    void DragRelease()
    {
        // Only after hitting ship
        if (lr.positionCount == 2)
        {
            hitAudio.Play();
            //hitEffect.Play();
            if (GameManager.hasRespawned)
            {
                //Physics.IgnoreLayerCollision(10, 3, false);
                GameManager.canHitAgain = false;
            }
        }

        lr.positionCount = 0;
        Vector3 dragReleasePos = Camera.main.ScreenToWorldPoint(touch.position);
        dragReleasePos.z = 0f;

        Vector3 force = (dragStartPos - dragReleasePos);
        Vector3 clampedForce = Vector3.ClampMagnitude(force, maxDrag) * power;
        
        rb.AddForce(clampedForce, ForceMode2D.Impulse);
    }

    public void SetTarget(Vector3 position)
    {
        targetPos = position;
        hasTarget = true;
    }
    */

}
