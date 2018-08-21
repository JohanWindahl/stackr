using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//This class represents the Deathblock that spawns from either left or right edge and moves through the world.
public class DeathBlock : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    private Transform tBody;

    private float maxSpeed = 10.0f; //Maximum speed of object
    private int direction; //Which direction is this object moving


    //This function is called by the GC when a object of this gets instantiated
    //An integer array 'v' with these variables: 0=speed, 2=direction{0=right,1=left}
    void runThis(int[] v)
    {
        this.maxSpeed = v[0];
        this.direction = v[1];
    }

    // Use this for initialization
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        tBody = gameObject.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        //checking which direction, what position it has, and which speed it has, if DB should move more

        if (direction.Equals(0) && rigidBody.position.x <= 50 && rigidBody.velocity.magnitude <= maxSpeed) 
        {
            tBody.position = new Vector2(rigidBody.position.x + (maxSpeed * Time.deltaTime), tBody.position.y); //Move DB to the right
        }
        else if (direction.Equals(1) && rigidBody.position.x >= -50 && rigidBody.velocity.magnitude <= maxSpeed)
        {
            tBody.position = new Vector2(rigidBody.position.x - (maxSpeed * Time.deltaTime), tBody.position.y); //Move DB to the left
        }
        else //it is in position, make it static
        {
            rigidBody.bodyType = RigidbodyType2D.Static;
        }

    }


}
