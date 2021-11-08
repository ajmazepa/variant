using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalControl : MonoBehaviour
{
    //Global variables
    public static GlobalControl Instance;
    public bool bgMusicEnabled = false;
    public bool adsEnabled = true;
    public int score;
    public float timeInGame;
    public int currentLevelIndex;
    public GameManager.Level currentLevel;

    void Awake()
    {
        //Preserve a single instance of the GlobalControl to maintain state across scenes
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
}
