using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



//This class represents the avatar of the player, its triggers and move functionality.
public class PlayerController : MonoBehaviour
{
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private Transform tform;
    private AudioSource audioSource;

    public AudioClip jumpSound;
    public AudioClip pickupSound;
    public AudioClip deathSound;

    private String currentColor; //State of color on avatar
    private bool isDead; //State of avatar is dead or not
    private bool gameOver; //Is the game over?
    private bool faceRight; //State of avatar facing direction
    private bool isGrounded; //State of avatar standing on something
    private int lane; //State of avatar standing on which lane {-1=left,0=middle,1=right}
    private float jumpPower; //How high can the avatar jump

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        currentColor = "Red"; //start red
        isGrounded = false; //start flying
        jumpPower = 1200; //tuned jumping height
        isDead = false; //start alive
        gameOver = false; //game is not over
        faceRight = true; //start facing right
        lane = 0; //start in middle lane
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Death") //touches death object
        {
            isDead = true;
        }
        else if (collision.gameObject.tag == "CoinGreen") //touches green coin
        {
            touchCoin("Green", collision);
        }
        else if (collision.gameObject.tag == "CoinRed") //touches red coin
        {
            touchCoin("Red", collision);
        }
        else if (collision.gameObject.tag == "CoinBlue")  //touches blue coin
        {
            touchCoin("Blue", collision);
        }
        else if (collision.gameObject.tag == "Ground") 
        {
            isGrounded = true;
        }
        else if (collision.gameObject.tag == "Red") //touches red building block
        { 
            touchBlock("Red", Color.red);
        }
        else if (collision.gameObject.tag == "Green")  //touches green building block
        {
            touchBlock("Green", Color.green);
        }
        else if (collision.gameObject.tag == "Blue") //touches blue building block
        {
            touchBlock("Blue", new Color(0, 0.3411765f, 1, 1));
        }
        else if (collision.gameObject.tag == "ComboGeneratorPowerUp") //touches powerup combo generator
        {
            moveObjAway(collision);
            sendMessageToGC("AvatarComboTouch");
            audioSource.PlayOneShot(pickupSound, 1.5F);
        }
        else if (collision.gameObject.tag == "SpeederPowerUp") //touches powerup speeder 
        {
            moveObjAway(collision);
            sendMessageToGC("AvatarSpeederTouch");
            audioSource.PlayOneShot(pickupSound, 1.5F);
        }
        
    }

    private void sendMessageToGC(string s) //run the fuction "s" on in Gamecontroller script. Communicate triggers to Gamecontroller
    {
        transform.parent.gameObject.GetComponent("GameController").SendMessage(s);
    }

    private void touchBlock(string v, Color c) //trigger changes: avatar touches a block
    {
        isGrounded = true;
        currentColor = v;
        sr.color = c;
        incScoreBlock();
    }

    private void touchCoin(string str, Collision2D collision) //trigger changes: avatar touches a coin
    {
        if (currentColor.Equals(str)) //correct color => increase score
        {
            moveObjAway(collision);
            incScoreCoin();
            audioSource.PlayOneShot(pickupSound, 1.5F);
        }
        else //wrong color => dead
        {
            isDead = true;
        }
    }

    private void incScoreCoin() //Call to GC, run this function
    {
        sendMessageToGC("AvatarCoinPickUp");
    }

    private void incScoreBlock()  //Call to GC, run this function
    {
        sendMessageToGC("AvatarBlockTouch");
    }


    private void moveObjAway(Collision2D collision) //Instead of destroying objects they are moved to the screen edge, when picked up
    {
        collision.gameObject.transform.position = new Vector2(50, collision.gameObject.transform.position.y);
    }


    private void OnCollisionExit2D(Collision2D collision) //trigger: when leaving building block or ground, you are now in the air
    {
        if (rb.gameObject.tag == "Ground" || collision.gameObject.tag == "Green"  || collision.gameObject.tag == "Blue" || collision.gameObject.tag == "Red")
        {
            isGrounded = false;
        }
    }


    // Update is called once per frame
    void Update()
    {
        GetComponent<Animator>().SetBool("grounded", isGrounded); //trigger: jump-animation, if not grounded


        if (GetComponent<Transform>().position.y < -1)  //trigger: falling below ground level -> dead
        {
            isDead = true;
        }

        if (!isDead) //trigger: alive -> being able to move
        {
            playerMove();
        }
        else if (isDead && !gameOver) //trigger: dead -> tell GC you are dead
        {
            gameOver = true; //preventing multiple messages to GC
            sendMessageToGC("AvatarSetGameOver");
            audioSource.PlayOneShot(deathSound, 1F);

        }

    }

    private void playerMove() //Avatar move functionaliy
    {
        if (Input.GetButtonDown("Jump") && isGrounded) //Jump if you are grounded and button is pushed
        {
            isGrounded = false;
            jump();
        }

        float currentXPos = getXCoord(); //Check if avatar got pushed closer to any other than current lane

        if (currentXPos>-7 && currentXPos<-2)
        {
            lane = -1;
        }
        else if (currentXPos < 2 && currentXPos > -2)
        {
            lane = 0;
        }
        else if (currentXPos < 7 && currentXPos > 2)
        {
            lane = 1;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) && isGrounded) //Move left if you are grounded and button is pushed
        {
            
            faceRight = false;
            flipFaceDirection();

            if (lane > -1) {lane--;}
            Vector2 newPosition = new Vector2();
            newPosition.y = transform.position.y;
            newPosition.x = lane * 4;
            transform.position = newPosition;


        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && isGrounded) //Move right if you are grounded and button is pushed
        {
            faceRight = true;
            flipFaceDirection();

            if (lane < 1) { lane++; }
            Vector2 newPosition = new Vector2();
            newPosition.y = transform.position.y;
            newPosition.x = lane * 4;
            transform.position = newPosition;
        }
    }

    private float getXCoord()
    {
        return transform.position.x;
    }

    private void flipFaceDirection() //flip direction of avatar-sprite on x-axis
    {
        if (faceRight==true)
        {
            sr.flipX = false;
        }
        else
        {
            sr.flipX = true;
        }
    }

    private void jump() //add a force upwards
    {
        audioSource.PlayOneShot(jumpSound, 1F);
        rb.AddForce(Vector2.up * jumpPower);
    }
}

