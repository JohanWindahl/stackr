using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This class represents the Coins and their functionality.
public class CoinPickUpController : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    private SpriteRenderer sr;
    public int color; //Color of BB (color{1=blue,2=green,3=red})


    //This function is called by the GC when a object of this gets instantiated
    void runThis(int c)
    {
        this.color = c; //Set the color of the object
    }


    // Use this for initialization
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        sr = gameObject.GetComponent<SpriteRenderer>();

        if (color.Equals(1)) //Coin is blue
        {
            rigidBody.tag = "CoinBlue";
            sr.color = new Color(0, 0.1333333f, 0.3764706f, 1);
        }
        else if (color.Equals(2))  //Coin is green
        {
            rigidBody.tag = "CoinGreen";
            sr.color = Color.green;
        }
        else if (color.Equals(3)) //Coin is red
        {
            sr.color = new Color(0.5f, 0.5f, 0.5f, 1);
            rigidBody.tag = "CoinRed";
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

}
