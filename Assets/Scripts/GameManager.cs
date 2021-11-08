using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using GoogleMobileAds.Api;
public class GameManager : MonoBehaviour
{
    //Global variables
    public static bool bgMusicEnabled;
    public int score;
    public float timeInGame;
    public int currentLevelIndex;
    //Public variables
    public System.Random ran = new System.Random();
    public GameObject continuePanel;
    public AudioSource bgMusicSource;
    public List<GameObject> ronaPrefabs;
    public List<AudioClip> sounds;
    public bool GameOn;
    public float timeInLevel;
    public float maxTimeInLevel;
    public List<Level> levels;
    public Level currentLevel;
    public Text scoreText;
    public Text timeText;
    public Text infoText;
    //Private variables
    private List<Vector2> ronaPositions;
    private List<GameObject> ronas = new List<GameObject>();
    private float scoreTick;
    private SpriteRenderer bgSpriteRenderer;
    private bool countDown;
    private InterstitialAd interstitial;
    //Start is called before the first frame update
    void Start()
    {
        //Rendering settings
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        //Load state from global GameControl
        bgMusicEnabled = GlobalControl.Instance.bgMusicEnabled;
        score = GlobalControl.Instance.score;
        timeInGame = GlobalControl.Instance.timeInGame;
        currentLevelIndex = GlobalControl.Instance.currentLevelIndex;
        //Set possible starting position for all ronas
        ronaPositions = new List<Vector2>();
        ronaPositions.Add(new Vector2(-9.74f, 5.58f));
        ronaPositions.Add(new Vector2(-0.03f, 6f));
        ronaPositions.Add(new Vector2(9.93f, 4.8f));
        ronaPositions.Add(new Vector2(7.44f, -6.02f));
        ronaPositions.Add(new Vector2(-0.05f, -6.02f));
        ronaPositions.Add(new Vector2(-9.93f, -4.76f));
        //Create levels
        levels = new List<Level>();
        //Level 1
        Level level = new Level();
        level.Index = 1;
        level.Variant = "Beta";
        level.VirusCount = 3;
        level.ronaMinSpeedMultiplier = 1;
        level.ronaMaxSpeedMultiplier = 1;
        level.ronaAccelerationMultiplier = 1;
        level.ronaSporeColor = new Color(175f / 255f, 50f / 255f, 190f / 255f);
        level.Time = 20;
        level.Soundtrack = "Level1.mp3";
        level.BG = "GameBG1.png";
        levels.Add(level);
        //Level 2
        level = new Level();
        level.Index = 2;
        level.Variant = "Gamma";
        level.VirusCount = 4;
        level.ronaMinSpeedMultiplier = 1;
        level.ronaMaxSpeedMultiplier = 1.1f;
        level.ronaAccelerationMultiplier = 1;
        level.ronaSporeColor = new Color(75f / 255f, 120f / 255f, 200f / 255f);
        level.Time = 30;
        level.Soundtrack = "Level2.mp3";
        level.BG = "GameBG2.png";
        levels.Add(level);
        //Level 3
        level = new Level();
        level.Index = 3;
        level.Variant = "Delta";
        level.VirusCount = 4;
        level.ronaMinSpeedMultiplier = 1;
        level.ronaMaxSpeedMultiplier = 1.2f;
        level.ronaAccelerationMultiplier = 1;
        level.ronaSporeColor = new Color(46f / 255f, 175f / 255f, 215f / 255f);
        level.Time = 60;
        level.Soundtrack = "Level3.mp3";
        level.BG = "GameBG3.png";
        levels.Add(level);
        //Level 4
        level = new Level();
        level.Index = 4;
        level.Variant = "Epsilon";
        level.VirusCount = 5;
        level.ronaMinSpeedMultiplier = 1.1f;
        level.ronaMaxSpeedMultiplier = 1.2f;
        level.ronaAccelerationMultiplier = 1;
        level.ronaSporeColor = new Color(55f / 255f, 210f / 255f, 100f / 255f);
        level.Time = 90;
        level.Soundtrack = "Level4.mp3";
        level.BG = "GameBG4.png";
        levels.Add(level);
        //Level 5
        level = new Level();
        level.Index = 5;
        level.Variant = "Omega";
        level.VirusCount = 5;
        level.ronaMinSpeedMultiplier = 1.1f;
        level.ronaMaxSpeedMultiplier = 1.2f;
        level.ronaAccelerationMultiplier = 1.1f;
        level.ronaSporeColor = new Color(175f / 255f, 100f / 255f, 100f / 255f);
        level.Time = 666;
        level.Soundtrack = "Level5.mp3";
        level.BG = "GameBG5.png";
        levels.Add(level);
        //Create an audio source at run-time to play the audio track for the level
        bgMusicSource = gameObject.AddComponent<AudioSource>();
        //Level initialization
        GameOn = true;
        InitLevel();
        if (GlobalControl.Instance.adsEnabled)
        {
            RequestInterstitial();
        }
    }

    void Update()
    {
        //If game is not paused
        if (GameOn)
        {
            //Let's increase the score and time spent in current level
            scoreTick += Time.deltaTime;
            if (scoreTick > 1)
            {
                score += 1 * ((currentLevelIndex * 2) + 1);
                scoreTick -= 1;
            }
            //If we've reached the max time for the level
            if (currentLevel.Time - timeInLevel < 0)
            {
                //Set time in level, update the info message, and end the level
                timeInLevel = currentLevel.Time;
                infoText.text = "Great job! You prevented the virus from mutating!";
                PlaySound(1);
                score += (currentLevelIndex + 1) * 100;
                EndLevel();
            }
            //Otherwise update the time and score
            else
            {
                timeInLevel += Time.deltaTime;
                //If there's less than 3 seconds to go, alert the player via audio
                if (!countDown && currentLevel.Time - timeInLevel <= 3)
                {
                    countDown = true;
                    PlaySound(2);
                }
            }
            scoreText.text = "SCORE: " + System.String.Format("{0:n0}", score);
            timeText.text = (currentLevel.Time - timeInLevel).ToString("F0");
        }
    }
    public void SaveState()
    {
        //Save the global variables in the GlobalControl so we can keep the data while switching scenes
        GlobalControl.Instance.bgMusicEnabled = bgMusicEnabled;
        GlobalControl.Instance.score = score;
        GlobalControl.Instance.timeInGame = timeInGame;
        GlobalControl.Instance.currentLevelIndex = currentLevelIndex;
        GlobalControl.Instance.currentLevel = levels[currentLevelIndex];
        //Save the high score in the PlayerPrefs so it persists between sessions
        int currentHighScore = PlayerPrefs.GetInt("High Score", 0);
        if (score > currentHighScore)
        {
            PlayerPrefs.SetInt("High Score", score);
        }
    }

    //Complete asynchronous load of an image
    private void Sprite_Completed(AsyncOperationHandle<Sprite> handle, SpriteRenderer spriteRenderer)
    {
        //If the addressable image has loaded, apply it to the SpriteRenderer
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Sprite result = handle.Result;
            spriteRenderer.sprite = result;
        }
    }
    //Complete asynchronous load of an audio file
    private void AudioClip_Completed(AsyncOperationHandle<AudioClip> handle, AudioSource audioSource)
    {
        //If the addressable audio clip has loaded, set the source and play it @ 40% volume
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            AudioClip result = handle.Result;
            audioSource.clip = result;
            audioSource.loop = true;
            audioSource.volume = 0.4f;
            audioSource.Play();
        }
    }

    //Play the specified sound from the sounds array
    public void PlaySound(int index)
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.clip = sounds[index];
        audioSource.Play();
    }

    //Show the continue panel when the roudn ends
    public void ShowContinue()
    {
        //Show an ad if enabled
        if (GlobalControl.Instance.adsEnabled && interstitial != null)
        {
            if (interstitial.IsLoaded())
            {
                interstitial.Show();
            }
        }
        continuePanel.SetActive(true);
    }

    //End the game
    public void GameOver()
    {
        //Save the final state of the game
        SaveState();
        //Show an ad if enabled
        if (GlobalControl.Instance.adsEnabled && interstitial != null)
        {
            interstitial.Destroy();
        }
        //Jump to the GameOver scene
        SceneManager.LoadScene("GameOver");
    }

    public void InitLevel()
    {
        //Set starting parameters for the level
        countDown = false;
        currentLevel = levels[currentLevelIndex];
        GlobalControl.Instance.currentLevel = levels[currentLevelIndex];
        timeInLevel = 0;
        timeText.text = currentLevel.Time.ToString("F0");
        //Remove existing ronas
        foreach(GameObject rona in ronas)
        {
            Destroy(rona);
        }
        //Create the ronas
        List<Vector2> avaiablePositions = new List<Vector2>(ronaPositions);
        for (int t = 1; t <= currentLevel.VirusCount; t++)
        {
            int randomNumber = ran.Next(0, avaiablePositions.Count);
            GameObject rona = Instantiate(ronaPrefabs[currentLevel.Index - 1], avaiablePositions[randomNumber], Quaternion.identity);
            ronas.Add(rona);
            avaiablePositions.RemoveAt(randomNumber);
        }
        //Set background image based on current level
        bgSpriteRenderer = GameObject.Find("BG").GetComponent<SpriteRenderer>();
        AsyncOperationHandle<Sprite> SpriteHandle = Addressables.LoadAsset<Sprite>("Assets/Sprites/" + currentLevel.BG);
        SpriteHandle.Completed += handle => Sprite_Completed(handle, bgSpriteRenderer);
        //Play background music if enabled
        if (bgMusicEnabled)
        {
            AsyncOperationHandle<AudioClip> AudioClipHandle = Addressables.LoadAsset<AudioClip>("Assets/Audio/" + currentLevel.Soundtrack);
            AudioClipHandle.Completed += handle => AudioClip_Completed(handle, bgMusicSource);
        }
        //Hide the continue panel when the level starts
        continuePanel = GameObject.Find("/Canvas/ContinuePanel");
        continuePanel.SetActive(false);
    }

    public void EndLevel()
    {
        //Stop the game
        GameOn = false;
        //If we've finished the last level - gameover, otherwise move to the next level
        if (currentLevel.Index >= levels.Count)
        {
            Invoke("GameOver", 1);
        }
        else
        {
            Invoke("ShowContinue", 1);
        }
    }

    private void RequestInterstitial()
    {
        #if UNITY_ANDROID
                string adUnitId = "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_IPHONE
                        string adUnitId = "ca-app-pub-3940256099942544/1033173712";
#else
                        string adUnitId = "unexpected_platform";
#endif
        // Initialize an InterstitialAd.
        interstitial = new InterstitialAd(adUnitId);
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        interstitial.LoadAd(request);
    }

    public void NextLevel()
    {
        //Show an ad if enabled
        if (GlobalControl.Instance.adsEnabled && interstitial != null)
        {
            interstitial.Destroy();
        }
        //Increase the level index, save the state, and load the next level
        currentLevelIndex++;
        SaveState();
        SceneManager.LoadScene("Game");
    }

    //A class to hold our level data
    public class Level
    {
        public int Index;
        public string Variant;
        public int VirusCount;
        public float ronaMinSpeedMultiplier;
        public float ronaMaxSpeedMultiplier;
        public float ronaAccelerationMultiplier;
        public Color ronaSporeColor;
        public string Soundtrack;
        public string BG;
        public int Time;
    }
}

