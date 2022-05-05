using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    //List<AsyncOperation> operations = new();
    public GameObject loadingScreen;
    public Slider loadBar;
    public TextMeshProUGUI loadText;

    public void SkinShop() => SceneManager.LoadSceneAsync("Shop");
    public void SettingsMenu() => SceneManager.LoadSceneAsync("Settings");

    public void LoadGame(string sceneName)
    {
        StartCoroutine(LoadScene(sceneName));
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
