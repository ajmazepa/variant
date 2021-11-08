using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using GoogleMobileAds.Api;
using System.ComponentModel;
using System.Net.Mail;

public class MainMenu : MonoBehaviour
{
    //Public variables
    public Text highScoreText;
    //Private variables
    private int highScore;
    private BannerView bannerView;
    private void Start()
    {
        //Initialize the gobal variables
        //Enable tunes
        GlobalControl.Instance.bgMusicEnabled = true;
        //Set starting score, time in game, and level
        GlobalControl.Instance.score = 0;
        GlobalControl.Instance.timeInGame = 0;
        GlobalControl.Instance.currentLevelIndex = 0;
        //Get the high score, if available
        highScore = PlayerPrefs.GetInt("High Score", 0);
        if (highScore != 0)
        {
            highScoreText.text = "HIGH SCORE: " + System.String.Format("{0:n0}", highScore);
        } else
        {
            highScoreText.text = "";
        }
        //Create an AudioSource at runtime to play and loop our intro music
        AudioSource bgMusicSource = gameObject.AddComponent<AudioSource>();
        AsyncOperationHandle<AudioClip> AudioClipHandle = Addressables.LoadAsset<AudioClip>("Assets/Audio/Intro.mp3");
        AudioClipHandle.Completed += handle => AudioClip_Completed(handle, bgMusicSource);

        // Initialize the Google Mobile Ads SDK if enabled
        if (GlobalControl.Instance.adsEnabled)
        {
            MobileAds.Initialize(initStatus => { });
            RequestBanner();
        }
    }

    private void AudioClip_Completed(AsyncOperationHandle<AudioClip> handle, AudioSource audioSource)
    {
        //If the addressable audio clip has loaded, set the source and play it @ 80% volume
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            AudioClip result = handle.Result;
            audioSource.clip = result;
            audioSource.loop = true;
            audioSource.volume = 0.8f;
            audioSource.Play();
        }
    }

    private void RequestBanner()
    {
        //Test ID
        //ca-app-pub-3940256099942544/6300978111
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/6300978111";
#else
            string adUnitId = "unexpected_platform";
#endif
        // Create a 728x90 banner at the bottom of the screen.
        bannerView = new BannerView(adUnitId, AdSize.Leaderboard, AdPosition.Bottom);
        // Called when an ad request has successfully loaded.
        this.bannerView.OnAdLoaded += this.HandleOnAdLoaded;
        // Called when an ad request failed to load.
        this.bannerView.OnAdFailedToLoad += this.HandleOnAdFailedToLoad;
        // Called when an ad is clicked.
        this.bannerView.OnAdOpening += this.HandleOnAdOpened;
        // Called when the user returned from the app after an ad click.
        this.bannerView.OnAdClosed += this.HandleOnAdClosed;
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the banner with the request.
        bannerView.LoadAd(request);
    }
    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLoaded event received");
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print("HandleFailedToReceiveAd event received with message: " + args.ToString());
    }

    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdOpened event received");
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdClosed event received");
    }

    public void StartGame()
    {
        //If we're showing ads, remove the ad before starting the game
        if (GlobalControl.Instance.adsEnabled && bannerView != null)
        {
            bannerView.Destroy();
        }
        //Load the introduction scene
        SceneManager.LoadScene("Introduction");
    }
 
}
