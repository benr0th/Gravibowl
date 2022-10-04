using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public AudioSource[] audioSources;
    
#if UNITY_WEBGL
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern string GetData(string key);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void SetData(string key, string value);
#endif

    private void Awake()
    {
        AudioManager[] objects = FindObjectsOfType<AudioManager>();

        if (objects.Length > 1)
            Destroy(objects[1].gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        int.TryParse(GetData("Mute"), out int muted);
        if (muted == 1)
#else
        if (SPrefs.GetInt("Mute") == 1)
        
#endif
            AudioListener.volume = 0;
        else
            AudioListener.volume = 1;
    }

    public void PlaySound(int index)
    {
        audioSources[index].Play();
    }

    public void AudioOnPress(Button button, int index)
    {
        button.onClick.AddListener(() => PlaySound(index));
    }

}
