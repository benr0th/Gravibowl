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
    int coins;
    Skin skin;

    void Start()
    {
        skin = skinManager.skins[skinIndex];
        GetComponent<Image>().sprite = skin.sprite;

        if (skinManager.IsUnlocked(skinIndex))
        {
            buyButtonText.text = "Equip";
        } else { buyButtonText.text = skin.cost.ToString(); }
    }

    public void OnBuyButtonPressed()
    {
        coins = PlayerPrefs.GetInt("Coins", 0);

        // Equip skin if unlocked
        if (skinManager.IsUnlocked(skinIndex))
        {
            skinManager.SelectSkin(skinIndex);
        } else
        {
            // Unlock skin
            if (coins >= skin.cost && !skinManager.IsUnlocked(skinIndex))
            {
                PlayerPrefs.SetInt("Coins", coins - skin.cost);
                skinManager.Unlock(skinIndex);
                buyButtonText.text = "Equip";
            }
            else
            {
                Debug.Log("Not enough coins");
            }
        }

    }
}
