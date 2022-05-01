using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AbilityHolder : MonoBehaviour
{
    [SerializeField] Ability[] abilities;
    [SerializeField] Image abilityImage;
    [SerializeField] Button abilityButton;
    float cooldownTime, activeTime;
    bool abilityClicked;

    enum AbilityState
    {
        ready,
        active,
        cooldown
    }
    AbilityState state = AbilityState.ready;

    private void Start()
    {
        abilityImage.fillAmount = 1;
    }

    void Update()
    {
        foreach (var ability in abilities)
            switch (state)
            {
                case AbilityState.ready:
                    if (abilityClicked)
                    {
                        ability.Activate();
                        state = AbilityState.active;
                        activeTime = ability.activeTime;
                        abilityClicked = false;
                        abilityImage.fillAmount = 0;
                    }
                    break;
                case AbilityState.active:
                    if (activeTime > 0)
                    {
                        activeTime -= Time.deltaTime;
                    }
                    else
                    {
                        state = AbilityState.cooldown;
                        cooldownTime = ability.cooldownTime;
                    }
                    break;
                case AbilityState.cooldown:
                    if (cooldownTime > 0)
                    {
                        abilityImage.fillAmount += 1 / ability.cooldownTime * Time.deltaTime;
                        cooldownTime -= Time.deltaTime;
                    }
                    else
                    {
                        state = AbilityState.ready;
                    }
                    break;
            }
    }

    public void AbilityClick()
    {
        if (state == AbilityState.ready)
            abilityClicked = true;
    }
}
