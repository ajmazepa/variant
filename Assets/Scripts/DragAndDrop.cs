using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    //Public variables
    public GameManager gm;
    //Private variables
    private bool moveAllowed;
    private bool dragging;
    private float distance;
    private Collider2D col;
    private ParticleSystem ronaPS;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.FindObjectOfType(typeof(GameManager)) as GameManager;
        col = GetComponent<Collider2D>();
        dragging = false;
        ronaPS = GetComponentInChildren<ParticleSystem>();
        ParticleSystem.MainModule remo = ronaPS.main;
        remo.startColor = gm.currentLevel.ronaSporeColor;
    }

    // Update is called once per frame
    void Update()
    {
        //Handle touch and drop (mobile)
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            //Returns pixel co-ordinates of touch position
            Vector2 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
            //If pressed, start dragging
            if (touch.phase == TouchPhase.Began)
            {
                Collider2D touchedCollider = Physics2D.OverlapPoint(touchPosition);
                if (col == touchedCollider)
                {
                    moveAllowed = true;
                }
            }
            //If we're dragging, update the position
            if (touch.phase == TouchPhase.Moved)
            {
                if (moveAllowed)
                {
                    transform.position = new Vector2(touchPosition.x, touchPosition.y);
                }
            }
            //If released, stop dragging
            if (touch.phase == TouchPhase.Ended)
            {
                moveAllowed = false;
            }
        }
        //Mouse click and drop (desktop)
        if (dragging)
        {
            //Set the position based on mouse position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 rayPoint = ray.GetPoint(distance);
            transform.position = rayPoint;
        }
    }
    private void OnMouseDown()
    {
        //Record the distance of the drag
        distance = Vector3.Distance(transform.position, Camera.main.transform.position);
        //Set flag to start mouse dragging
        dragging = true;
    }

    void OnMouseUp()
    {
        //Set flag to stop mouse dragging
        dragging = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //If two ronas have collided
        if (collision.tag == "Rona" && gm.GameOn == true)
        {
            ParticleSystem.MainModule remo = ronaPS.main;
            remo.startColor = new ParticleSystem.MinMaxGradient(new Color(255, 246, 0, 255));
            gm.PlaySound(0);
            gm.infoText.text = "Oh no! The virus has mutated into the " + gm.currentLevel.Variant.ToUpper() + " variant!";
            gm.EndLevel();
        }
    }
}
