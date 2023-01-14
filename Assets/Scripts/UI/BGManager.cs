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
        bgSprite.sprite = bg switch
        {
            0 => bgs[0],
            1 => bgs[1],
            2 => bgs[2],
            _ => bgSprite.sprite
        };
    }
}
