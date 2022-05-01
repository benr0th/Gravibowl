using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagnetGauge : MonoBehaviour
{
    public Slider magnetBar;
    float maxMagnet = 100;
    float currentMagnet;
    public bool outOfMagnet;
    WaitForSeconds regenTick = new WaitForSeconds(0.05f);
    Coroutine regen;
    /* singleton
    public static MagnetGauge instance;

    private void Awake()
    {
        instance = this;
    }
    */

    void Start()
    {
        currentMagnet = maxMagnet;
        magnetBar.maxValue = maxMagnet;
        magnetBar.value = maxMagnet;
    }

    private void Update()
    {
        if (currentMagnet - 1f >= 0)
            outOfMagnet = false;
        else
            outOfMagnet = true;
    }

    public void UseMagnet(float amount)
    {
        if (currentMagnet - 1f >= 0)
        {
            currentMagnet -= amount * Time.deltaTime;
            magnetBar.value = currentMagnet;
            if (regen != null)
                StopCoroutine(regen);
            regen = StartCoroutine(RegenMagnet());
        }
        else
        {
            //TODO - Change to a popup message
            
        }
    }

    IEnumerator RegenMagnet()
    {
        yield return new WaitForSeconds(1.3f);
        while (currentMagnet < maxMagnet)
        {
            currentMagnet += 5;
            magnetBar.value = currentMagnet;
            yield return regenTick;
        }
        regen = null;
    }
}
