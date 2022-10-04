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

#if UNITY_WEBGL
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern string GetData(string key);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void SetData(string key, string value);
#endif

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
#if UNITY_WEBGL && !UNITY_EDITOR
        int.TryParse(GetData("Coins"), out int parseCoins);
        coins = parseCoins;
#else
        coins = SPrefs.GetInt("Coins", 0);
#endif

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
#if UNITY_WEBGL && !UNITY_EDITOR
                SetData("Coins", (coins - skin.cost).ToString());
#else
                SPrefs.SetInt("Coins", coins - skin.cost);
#endif
                skinManager.Unlock(skinIndex);
                buyButtonText.text = "Equip";
                audioManager.PlaySound(9);
#if UNITY_WEBGL && !UNITY_EDITOR
                int.TryParse(GetData("EquipOnBuy"), out int eob);
                if (eob == 1)
#else
                if (SPrefs.GetInt("EquipOnBuy") == 1)
#endif
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
