using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGManager : MonoBehaviour
{
    [SerializeField] Sprite[] bgs;

    private void Awake()
    {
        BGManager[] objects = FindObjectsOfType<BGManager>();

        if (objects.Length > 1)
            Destroy(objects[1].gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        var bg = SPrefs.GetInt("Background", 0);
        var bgSprite = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
        if (bg == 0)
            bgSprite.sprite = bgs[0];
        if (bg == 1)
            bgSprite.sprite = bgs[1];
        if (bg == 2)
            bgSprite.sprite = bgs[2];
    }
}
