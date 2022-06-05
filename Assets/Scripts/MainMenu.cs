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
    public GameObject loadingScreen;
    public Slider loadBar;
    public TextMeshProUGUI loadText;

    public void SkinShop() => SceneManager.LoadSceneAsync("Shop");
    public void SettingsMenu() => SceneManager.LoadSceneAsync("Settings");

    public void LoadGame(string sceneName)
    {
        PlayerPrefs.SetInt("TwoPlayer", 0);
        StartCoroutine(LoadScene(sceneName));
    }

    public void Load2Player()
    {
        PlayerPrefs.SetInt("TwoPlayer", 1);
        SceneManager.LoadSceneAsync("Game");
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
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        loadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            loadBar.value = progress;
            loadText.text = $"{progress * 100f}%";
            yield return null;
        }
    }

    //public void LoadGame()
    //{
    //    operations.Add(SceneManager.UnloadSceneAsync("Menu"));
    //    operations.Add(SceneManager.LoadSceneAsync("Game", LoadSceneMode.Additive));
    //    operations.Add(SceneManager.LoadSceneAsync("Shop", LoadSceneMode.Additive));
    //    operations.Add(SceneManager.LoadSceneAsync("Settings", LoadSceneMode.Additive));
    //    if (operations[1].isDone)
    //        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Game"));
    //}
}
