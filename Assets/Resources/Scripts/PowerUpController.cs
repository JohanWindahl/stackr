using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This class represents the Powerups and their functionality.

public class PowerUpController : MonoBehaviour
{
    private Rigidbody2D rigidBody;

    private Transform tBody;
    private bool isDone;

    private int lanes;  //State of PowerUp spawn on which lane {-1=left,0=middle,1=right}
    private int powerUpType; //State of PowerUp is it a Speeder=0 or a ComboGenerator=1
    private int direction; //State of PowerUp is it moving right=0 or left=1?
    private float maxSpeed; //Max speed of PowerUp


    //Use this for initialization
    void Start()
    {
        tBody = gameObject.GetComponent<Transform>();
        rigidBody = GetComponent<Rigidbody2D>();

        updateSprite();
    }

    // Update is called once per frame
    void Update()
    {
        //checking which direction, what position it has, and which speed it has, if PowerUp should move more
        if (direction.Equals(0) && rigidBody.position.x <= 50 && rigidBody.velocity.magnitude <= maxSpeed)
        {
            tBody.position = new Vector2(rigidBody.position.x + (maxSpeed * Time.deltaTime), tBody.position.y); //Move PowerUp to the right
        }
        else if (direction.Equals(1) && rigidBody.position.x >= -50 && rigidBody.velocity.magnitude <= maxSpeed)
        {
            tBody.position = new Vector2(rigidBody.position.x - (maxSpeed * Time.deltaTime), tBody.position.y); //Move PowerUp to the left
        }
        else  //it is in position, make it static
        {
            rigidBody.bodyType = RigidbodyType2D.Static;
        }
    }


    //This function is called by the GC when a object of this gets instantiated
    //An integer array 'v' with these variables: 1=powerUpType{0=ComboGenerator, 1=Speeder}, 2=direction{0=right,1=left}, 3=laneOrside{0=side or 1=lane}
    void runThis(int[] v)
    {
        this.powerUpType = v[0];
        this.direction = v[1];

        if (v[2].Equals(1)) //if it a lane-spawn, set speed to 0
        {
            this.maxSpeed = 0;
        }
        else //if it a side-spawn, set speed to static 12 
        {
            this.maxSpeed = 12;
        }
    }

    void updateSprite() //Update the sprite & tag of the PowerUp
    {
        Sprite[] allSprites = Resources.LoadAll<Sprite>("Sprites/Iconic1024x1024");

        if (powerUpType.Equals(1)) //set it to a Speeder sprite
        {
            rigidBody.tag = "SpeederPowerUp";
            this.GetComponent<SpriteRenderer>().sprite = allSprites[82];
        }
        else //set it to a ComboGenerator sprite
        {
            rigidBody.tag = "ComboGeneratorPowerUp";
            this.GetComponent<SpriteRenderer>().sprite = allSprites[118];
        }
    }
}

