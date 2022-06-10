using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkinShopItem : MonoBehaviour
{
    [SerializeField] SkinManager skinManager;
    [SerializeField] int skinIndex;
    [SerializeField] Button buyButton;
    [SerializeField] TextMeshProUGUI buyButtonText;
    Skin skin;
    AudioManager audioManager;
    int coins;

    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    void Start()
    {
        skin = skinManager.skins[skinIndex];
        GetComponent<Image>().sprite = skin.sprite;
        skinManager.Unlock(0); // Default skin, always unlocked

        if (skinManager.IsUnlocked(skinIndex))
            buyButtonText.text = "Equip";
        else
            buyButtonText.text = skin.cost.ToString();
    }

    public void OnBuyButtonPressed()
    {
        coins = SPrefs.GetInt("Coins", 0);

        // Equip skin if unlocked
        if (skinManager.IsUnlocked(skinIndex))
        {
            skinManager.SelectSkin(skinIndex);
            audioManager.PlaySound(0);
        }
        else
        {
            // Unlock skin
            if (coins >= skin.cost && !skinManager.IsUnlocked(skinIndex))
            {
                SPrefs.SetInt("Coins", coins - skin.cost);
                skinManager.Unlock(skinIndex);
                buyButtonText.text = "Equip";
                audioManager.PlaySound(9);
                if (SPrefs.GetInt("EquipOnBuy") == 1)
                    skinManager.SelectSkin(skinIndex);
            }
            else
            {
                // TODO - Add actual popup message
                audioManager.PlaySound(4);
                Debug.Log("Not enough coins");
            }
        }

    }
}
