using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Introduction : MonoBehaviour
{
    public void StartGame()
    {
        //Load the game scene
        SceneManager.LoadScene("Game");
    }
}
