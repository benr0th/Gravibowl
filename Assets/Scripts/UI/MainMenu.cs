using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    //List<AsyncOperation> operations = new();
    [SerializeField] Button playOptions, singlePlayer, twoPlayer, vsCPU, shop, settings, backButton;
    [SerializeField] GameObject loadingScreen;
    [SerializeField] Slider loadBar;
    [SerializeField] TextMeshProUGUI loadText;
    AudioManager audioManager;
    int timesPlayed;

#if UNITY_WEBGL
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern string GetData(string key);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void SetData(string key, string value);
#endif

    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
#if UNITY_WEBGL && !UNITY_EDITOR
        int.TryParse(GetData("TimesPlayed"), out int timesplayed);
        timesPlayed = timesplayed;
        if (GetData("Coins") == null)
            SetData("Coins", "0");
#else
        timesPlayed = SPrefs.GetInt("TimesPlayed", 0);
#endif
    }

    private void Start()
    {
        audioManager.AudioOnPress(singlePlayer, 1);
        audioManager.AudioOnPress(twoPlayer, 1);
        audioManager.AudioOnPress(vsCPU, 1);
        audioManager.AudioOnPress(playOptions, 0);
        audioManager.AudioOnPress(backButton, 0);
        audioManager.AudioOnPress(shop, 0);
        audioManager.AudioOnPress(settings, 0);
    }

    public void SkinShop() => SceneManager.LoadSceneAsync("Shop");
    public void SettingsMenu() => SceneManager.LoadSceneAsync("Settings");

    public void LoadGame(string sceneName)
    {
        SPrefs.SetInt("TwoPlayer", 0);
        SPrefs.SetInt("CPU", 0);
        StartCoroutine(LoadScene(sceneName));
    }

    public void Load2Player()
    {
        SPrefs.SetInt("TwoPlayer", 1);
        SPrefs.SetInt("CPU", 0);
        StartCoroutine(LoadScene("Game"));
    }

    public void LoadCPUPlayer()
    {
        SPrefs.SetInt("TwoPlayer", 1);
        SPrefs.SetInt("CPU", 1);
        StartCoroutine(LoadScene("Game"));
    }

    public void PlayOptions()
    {
        shop.gameObject.SetActive(false);
        settings.gameObject.SetActive(false);
        playOptions.gameObject.SetActive(false);
        singlePlayer.gameObject.SetActive(true);
        twoPlayer.gameObject.SetActive(true);
        vsCPU.gameObject.SetActive(true);
        backButton.gameObject.SetActive(true);
    }

    public void Back()
    {
        shop.gameObject.SetActive(true);
        settings.gameObject.SetActive(true);
        playOptions.gameObject.SetActive(true);
        singlePlayer.gameObject.SetActive(false);
        twoPlayer.gameObject.SetActive(false);
        vsCPU.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);
    }

    IEnumerator LoadScene(string sceneName)
    {
        timesPlayed++;
#if UNITY_WEBGL && !UNITY_EDITOR
        SetData("TimesPlayed", timesPlayed.ToString());
#else
        SPrefs.SetInt("TimesPlayed", timesPlayed);
#endif
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        loadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            loadBar.value = progress;
            float progressText = progress * 100f;
            loadText.text = progressText.ToString("F0") + "%";
            yield return null;
        }
    }

    /*
    public void LoadGame()
    {
        operations.Add(SceneManager.UnloadSceneAsync("Menu"));
        operations.Add(SceneManager.LoadSceneAsync("Game", LoadSceneMode.Additive));
        operations.Add(SceneManager.LoadSceneAsync("Shop", LoadSceneMode.Additive));
        operations.Add(SceneManager.LoadSceneAsync("Settings", LoadSceneMode.Additive));
        if (operations[1].isDone)
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("Game"));
    }
    */
}
