using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public AudioSource[] audioSources;

    private void Awake()
    {
        AudioManager[] objects = FindObjectsOfType<AudioManager>();

        if (objects.Length > 1)
            Destroy(objects[1].gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (SPrefs.GetInt("Mute") == 1)
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
