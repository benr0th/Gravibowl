using System;
using System.Collections.Generic;
using Unity.Services.Core;
using Unity.Services.Mediation;
using UnityEngine;

public class AdsManager : MonoBehaviour
{
    IRewardedAd ad;
    readonly List<IRewardedAd> ads = new();
    GameManager GameManager;
    [SerializeField] GameOverScreen GameOverScreen;
    [SerializeField] UIController ui;

#if UNITY_IOS
    string[] adUnitIds = { "Rewarded_iOS", "Rewarded_iOS_2" };
    string gameId = "4722742";
#else
    string[] adUnitIds = { "Rewarded_Android", "Rewarded_Android_2" };
    string gameId = "4722743";
#endif

#if UNITY_WEBGL
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern string GetData(string key);

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void SetData(string key, string value);
#endif

    private void Start()
    {
        InitServices();
        GameManager = GetComponent<GameManager>();
    }

    public async void InitServices()
    {
        try
        {
            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetGameId(gameId);
            await UnityServices.InitializeAsync(initializationOptions);

            InitializationComplete();
        }
        catch (Exception e)
        {
            InitializationFailed(e);
        }
    }

    public void SetupAd()
    {
        //Create
        foreach (var adUnitId in adUnitIds)
        {
            ad = MediationService.Instance.CreateRewardedAd(adUnitId);
            ads.Add(ad);
        }
        //Subscribe to events
        foreach (var ad in ads)
        {
            ad.OnLoaded += AdLoaded;
            ad.OnFailedLoad += AdFailedLoad;

            ad.OnClosed += AdClosed;
            ad.OnClicked += AdClicked;
            ad.OnUserRewarded += UserRewarded;
        }
        // Impression Event
        MediationService.Instance.ImpressionEventPublisher.OnImpression += ImpressionEvent;
    }

    /* old ad code
    public void ShowAdCoin()
    {
        if (ads[0].AdState == AdState.Loaded)
            ads[0].Show();
    }

    public void ShowAdContinue()
    {
        if (ads[1].AdState == AdState.Loaded)
            ads[1].Show();
    }
    */

    public async void ShowAd()
    {
        if (ads[0].AdState == AdState.Loaded)
        {
            try
            {
                RewardedAdShowOptions showOptions = new();
                showOptions.AutoReload = true;
                await ads[0].ShowAsync(showOptions);
                AdShown();
            }
            catch (ShowFailedException e)
            {
                AdFailedShow(e);
            }
        }
    }


    void InitializationComplete()
    {
        SetupAd();
        //foreach (var ad in ads)
        //    ad.Load();
        LoadAd();
    }

    async void LoadAd()
    {
        try
        {
            await ads[0].LoadAsync();
        }
        catch (LoadFailedException)
        {
            // We will handle the failure in the OnFailedLoad callback
        }
    }

    void InitializationFailed(Exception e)
    {
        Debug.Log("Initialization Failed: " + e.Message);
    }

    void AdLoaded(object sender, EventArgs args)
    {
        Debug.Log("Ad loaded");
    }

    void AdFailedLoad(object sender, LoadErrorEventArgs args)
    {
        Debug.Log("Failed to load ad");
        Debug.Log(args.Message);
    }

    void AdShown()
    {
        Debug.Log("Ad shown!");
    }

    void AdClosed(object sender, EventArgs e)
    {
        LoadAd();
        Debug.Log("Ad has closed");
        // Execute logic after an ad has been closed.
    }


    void AdClicked(object sender, EventArgs e)
    {
        Debug.Log("Ad has been clicked");
        // Execute logic after an ad has been clicked.
    }

    void AdFailedShow(ShowFailedException e)
    {
        Debug.Log(e.Message);
    }

    void ImpressionEvent(object sender, ImpressionEventArgs args)
    {
        var impressionData = args.ImpressionData != null ? JsonUtility.ToJson(args.ImpressionData, true) : "null";
        Debug.Log("Impression event from ad unit id " + args.AdUnitId + " " + impressionData);
    }

    void UserRewarded(object sender, RewardEventArgs e)
    {
        if (GameManager.coinAdClicked)
        {
            GameManager.coins += GameOverScreen.coinsGained;
#if UNITY_WEBGL && !UNITY_EDITOR
            SetData("Coins", GameManager.coins.ToString());
#else
            SPrefs.SetInt("Coins", GameManager.coins);
#endif
            GameOverScreen.Invoke("AddCoins", 0.5f);
            GameManager.coinAdClicked = false;
            ui.coinAd.gameObject.SetActive(false);
        }
        if (GameManager.continueAdClicked)
        {
            GameManager.Respawn();
            GameManager.continueAdClicked = false;
            ui.continueAd.gameObject.SetActive(false);
        }
    }

}
