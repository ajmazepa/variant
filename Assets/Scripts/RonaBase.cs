using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RonaBase : MonoBehaviour
{
    //Public variables
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;
    public float minSpeed;
    public float maxSpeed;
    public float acceleration;
    //Private variables
    private Vector2 targetPosition;
    private float speed;
    private GameManager gm;
    // Start is called before the first frame update
    void Start()
    {
        //Set our initial variables
        gm = GameObject.FindObjectOfType(typeof(GameManager)) as GameManager;
        targetPosition = GetRandomPosition();
        acceleration = 60 * GlobalControl.Instance.currentLevel.ronaAccelerationMultiplier;
        minSpeed = minSpeed * GlobalControl.Instance.currentLevel.ronaMinSpeedMultiplier;
        maxSpeed = maxSpeed * GlobalControl.Instance.currentLevel.ronaMaxSpeedMultiplier;
    }

    // Update is called once per frame
    void Update()
    {
        //If game is not paused
        if (gm.GameOn)
        {
            //If we have not reached out target position, move towards it
            if ((Vector2)transform.position != targetPosition)
            {
                //Set our speed based on level parameters and move
                speed = Mathf.Lerp(minSpeed, maxSpeed, GetDifficultyPercentage());
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            }
            //Otherwise, get new target position
            else
            {
                targetPosition = GetRandomPosition();
            }
        }
    }
    //Get a random Vector2 position
    Vector2 GetRandomPosition()
    {
        float randomX = Random.Range(minX, maxY);
        float randomY = Random.Range(minY, maxY);
        return new Vector2(randomX, randomY);
    }

    float GetDifficultyPercentage()
    {
        //Increase our difficulty/speed over time
        return Mathf.Clamp01(Time.timeSinceLevelLoad / acceleration);
    }
}
