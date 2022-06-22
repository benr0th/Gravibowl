using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkinManager", menuName = "Scriptable/Skin Manager")]
public class SkinManager : ScriptableObject
{
    [SerializeField] public Skin[] skins;
    private const string Prefix = "Skin_";
    private const string SelectedSkin = "SelectedSkin";

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern string GetData(string key);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void SetData(string key, string value);

    public void SelectSkin(int skinIndex) =>
#if UNITY_WEBGL && !UNITY_EDITOR
        SetData(SelectedSkin, skinIndex.ToString());
#else
        SPrefs.SetInt(SelectedSkin, skinIndex);
#endif

    public Skin GetSelectedSkin()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        int.TryParse(GetData(SelectedSkin), out int selectedskin);
        int skinIndex = selectedskin;
#else
        int skinIndex = SPrefs.GetInt(SelectedSkin, 0);
#endif
        if (skinIndex >= 0 && skinIndex < skins.Length)
        {
            return skins[skinIndex];
        } else
        {
            return null;
        }
    }

    public void Unlock(int skinIndex) =>
#if UNITY_WEBGL && !UNITY_EDITOR
        SetData(Prefix + skinIndex, "1");
#else
        SPrefs.SetInt(Prefix + skinIndex, 1);
#endif

#if UNITY_WEBGL && !UNITY_EDITOR
    public bool IsUnlocked(int skinIndex)
    {
        int.TryParse(GetData(Prefix + skinIndex), out int unlocked);
        return unlocked == 1;
    }
#else
    public bool IsUnlocked(int skinIndex) => SPrefs.GetInt(Prefix + skinIndex, 0) == 1;
#endif
}
