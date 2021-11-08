using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class GameOver : MonoBehaviour
{
    //Public variables
    public Text yourScoreText;
    private void Start()
    {
        //Show score
        yourScoreText.text = "YOUR SCORE: " + System.String.Format("{0:n0}", GlobalControl.Instance.score);
        //Create an AudioSource at runtime to play and loop our end game music music
        AudioSource bgMusicSource = gameObject.AddComponent<AudioSource>();
        AsyncOperationHandle<AudioClip> AudioClipHandle = Addressables.LoadAsset<AudioClip>("Game Over.wav");
        AudioClipHandle.Completed += handle => AudioClip_Completed(handle, bgMusicSource);
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
    public void RestartGame()
    {
        //Load the introduction scene
        SceneManager.LoadScene("MainMenu");
    }
}
