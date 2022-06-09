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
        {
            buyButtonText.text = skin.cost.ToString();
            audioManager.AudioOnPress(buyButton, 9);
        }
    }

    public void OnBuyButtonPressed()
    {
        coins = PlayerPrefs.GetInt("Coins", 0);

        // Equip skin if unlocked
        if (skinManager.IsUnlocked(skinIndex))
            skinManager.SelectSkin(skinIndex);
        else
        {
            // Unlock skin
            if (coins >= skin.cost && !skinManager.IsUnlocked(skinIndex))
            {
                PlayerPrefs.SetInt("Coins", coins - skin.cost);
                skinManager.Unlock(skinIndex);
                buyButtonText.text = "Equip";
                buyButton.onClick.RemoveListener(() => audioManager.PlaySound(9));
                if (PlayerPrefs.GetInt("EquipOnBuy") == 1)
                    skinManager.SelectSkin(skinIndex);
            }
            else
            {
                // TODO - Add actual popup message
                Debug.Log("Not enough coins");
            }
        }

    }
}
