using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BGManager : MonoBehaviour
{
    [SerializeField] Sprite[] bgs;

#if UNITY_WEBGL
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern string GetData(string key);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void SetData(string key, string value);
#endif

    private void Awake()
    {
        BGManager[] objects = FindObjectsOfType<BGManager>();

        if (objects.Length > 1)
            Destroy(objects[1].gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        int.TryParse(GetData("Background"), out int background);
        var bg = background;
#else
        var bg = SPrefs.GetInt("Background", 0);
#endif
        var bgSprite = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
        if (bg == 0)
            bgSprite.sprite = bgs[0];
        if (bg == 1)
            bgSprite.sprite = bgs[1];
        if (bg == 2)
            bgSprite.sprite = bgs[2];
    }
}
