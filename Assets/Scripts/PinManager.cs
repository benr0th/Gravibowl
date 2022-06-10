using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinManager : MonoBehaviour
{
    TutorialManager tutorialManager;
    ScoreManager scoreManager;
    ShipControl ship;
    CPUPlayer cpu;
    public bool pinFallen, pinDetected, pinHit;

    private void Awake()
    {
        ship = GameObject.Find("Ship").GetComponentInParent<ShipControl>();
        cpu = ship.GetComponentInParent<CPUPlayer>();
        tutorialManager = GameObject.Find("TutorialManager").GetComponent<TutorialManager>();
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("CPU"))
        {
            pinDetected = true;
            ship.isTouching = false;
            ship.stoppedTouching = true;
            cpu.hasLetGo = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if ((other.collider.CompareTag("Ship") | other.collider.CompareTag("Pin"))
            & !pinHit)
        {
            pinHit = true;
            scoreManager.pinsHit++;
            Mathf.Clamp(scoreManager.pinsHit, 0, 10);
            scoreManager.pinsHitThisBowl++;

            StartCoroutine(HitSound());
        }
    }

    IEnumerator HitSound()
    {
        yield return new WaitForSeconds(0.075f);
        if (scoreManager.pinsHitThisBowl > 1 & !scoreManager.soundMultiPlayed)
        {
            scoreManager.multiPinHit.Play();
            scoreManager.soundMultiPlayed = true;
        }
        else if (scoreManager.pinsHitThisBowl == 1 & !scoreManager.soundSinglePlayed)
        {
            scoreManager.singlePinHit.Play();
            scoreManager.soundSinglePlayed = true;
        }
    }
}
