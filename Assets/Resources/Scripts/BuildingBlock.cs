using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//This class represents the Buildingblock that spawns from either left or right edge and moves through the world to the center of the x-axis.
public class BuildingBlock : MonoBehaviour {
    private Rigidbody2D rigidBody;
    private Transform tBody;
    private SpriteRenderer sr;
    private BoxCollider2D bc;

    private float maxSpeed; //Max speed of BB
    private int color; //Color of BB (color{1=blue,2=green,3=red})
    private int direction; //State of Block is it moving right=0 or left=1?
    private bool isDone; //State of BB, has it moved to the center

    void runThis(int[] v) //This function is called by the GC when a object of this gets instantiated
    {
        //speed, accel, color{1=blue,2=green,3=red}, direction{0=right,1=left};
        //An integer array 'v' with these variables: 0=speed, 2=direction{0=right,1=left}
        this.maxSpeed = v[0];
        this.color = v[1];
        this.direction = v[2];
    }

    // Use this for initialization
    void Start () {

        rigidBody = GetComponent<Rigidbody2D>();
        tBody = gameObject.GetComponent<Transform>();

        isDone = false; //BB is not in center
        Sprite[] smoothBlocks = Resources.LoadAll<Sprite>("Sprites/SmoothBlocks");

        if (color.Equals(1)) //BB is blue
        {
            rigidBody.tag = "Blue";
            this.GetComponent<SpriteRenderer>().sprite = smoothBlocks[8];
        }
        else if (color.Equals(2)) //BB is green
        {
            rigidBody.tag = "Green";
            this.GetComponent<SpriteRenderer>().sprite = smoothBlocks[11];
        }
        else if (color.Equals(3)) //BB is red
        {
            rigidBody.tag = "Red";
            this.GetComponent<SpriteRenderer>().sprite = smoothBlocks[2];
        }
    }

    // Update is called once per frame

    void Update () {
        //checking if block is in middle, which direction, what position it has, and which speed it has, if DB should move more

        if (!isDone && direction.Equals(0) && rigidBody.position.x <= 0 && rigidBody.velocity.magnitude <= maxSpeed)
        {
            tBody.position = new Vector2(rigidBody.position.x + (maxSpeed * Time.deltaTime), tBody.position.y); //Move DB to the right

        }
        else if (!isDone && direction.Equals(1) && rigidBody.position.x >= 0 && rigidBody.velocity.magnitude <= maxSpeed)
        {
            tBody.position = new Vector2(rigidBody.position.x - (maxSpeed * Time.deltaTime), tBody.position.y); //Move DB to the left
        }
        else //it is in position, make it static
        {
            isDone = true;
            tBody.position = new Vector2(0, tBody.position.y);
            rigidBody.bodyType = RigidbodyType2D.Static;
        }

    }


}
