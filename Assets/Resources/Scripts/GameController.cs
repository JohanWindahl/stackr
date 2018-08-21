using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


//This class is the foundation of the game and controlls the mechanics
public class GameController : MonoBehaviour {

    //Different canvases
    public Canvas startCanvas; 
    public Canvas gameOverCanvas;
    public Canvas ScoreCanvas;

    //Texts to update
    public Text scoreText; //Current score
    public Text scoreText_gameOver; //Current score gameover canvas
    public Text coinText; //Current score per coin text
    public Text levelText; //Current level text
    public Text timeText;  //Current time text

    //Gameobjects to spawn
    public GameObject BBPrefab; //Building block prefab
    public GameObject CoinPrefab; //Coin prefab
    public GameObject DeathBlockPrefab; //Death block prefab
    public GameObject AvatarPrefab; //Avatar prefab
    public GameObject PowerUpPrefab; //Powerup prefab

    private bool gameIsPaused; //If gameplay is paused
    private bool finalScoreSet; //If final score is calculated and shown
    private bool gameOver; //Game is over

    private int coinScore; //Current score per coin
    private int currentScore; //Current score
    private int blockCount; // Number of spawned blocks

    private int totalElapsed; //Total time spent playing this round
    private float elapsed; //Time spent between each generated object

    public float globalMultiplier; //This is a global score multiplier that enhances score, starts at 1==100%
    private float yMult; //How long should the blocks be spaced between eachother in Y-direction
    private float yPos; //position on the y-axis the prefab should spawn
    private float xPos; //position on the x-axis the prefab should spawn
    private double spawnTimer; //When elapsed has reached this "spawnTimer" number a new prefab is spawned
    private int level; //Current level

    // Use this for initialization
    void Start () {
        startCanvas.enabled = true;  //Show startscreen
        gameOverCanvas.enabled = false; //Do not show gameover-screen
        ScoreCanvas.enabled = false; //Do not show score-screen

        gameIsPaused = true; //Pause game on startscreen
        finalScoreSet = false; //final score not set
        gameOver = false; //game is starting

        xPos = -25; //Default spawn location x-axis
        yPos = 1.0f; //Default spawn location y-axis
        yMult = 2.56f; //Default spacer between spawn location y-axis


        level = 1; //Level starts at 1
        coinScore = 5; //Score per coin is 5 in the beginning
        currentScore = 0; //Start with 0 score
        totalElapsed = 0; //Timer starts at 0
        blockCount = 0; //Block counter starts at 0
        globalMultiplier = 1f; //Global multiplier starts att 100%

        spawnTimer = 1.5f; //Default time between spawns

        updateScoreText(); //Set Scoretext
        updateCoinText(); //Set Cointext

        //Init player avatar
        GameObject avatar = Instantiate(AvatarPrefab, new Vector3(0, 10, 0), Quaternion.identity);
        avatar.transform.parent = gameObject.transform;
        avatar.SetActive(true);
    }

        // Update is called once per frame
        void Update () {

        if (gameIsPaused) //pause game
        {
            Time.timeScale = 0;
        }
        else //unpause game
        {
            Time.timeScale = 1;
        }

        if (gameIsPaused && Input.GetKeyDown(KeyCode.Space)) //go fromstartscreen to game play
        {
            gameIsPaused = false;
            startCanvas.enabled = false;
            gameOverCanvas.enabled = false;
            ScoreCanvas.enabled = true;
        }

        if (gameOver) //Game is over
        {
            startCanvas.enabled = false;
            gameOverCanvas.enabled = true;
            ScoreCanvas.enabled = false;

            if (!finalScoreSet) //Update final score
            {
                updateFinalScoreText();
            } 

            if (Input.GetKeyDown(KeyCode.Space)) //Restart game
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
        else //Game is not over
        {
            if (elapsed >= spawnTimer) //Check if timer has ended -> spawn object
            {
                blockCount++;
                elapsed = 0f;
                spawnObject(blockCount);
                if (blockCount % 10 == 0) { increaseLevel(); }
            }

            if (!gameIsPaused) //Dont count frames if game is paused
            {
                totalElapsed++; //Count total frames
                elapsed += Time.deltaTime; //Count frames
            }

            updateScoreText(); //Update score
            updateTimeText(); //Update time
        }
    }

    //This function generates an object and instantiates it.
    //Each call a BB or DB is spawn, each 4th object a coin is spawned and
    //each tenth object a powerup is spawned
    private void spawnObject(int i)
    {
        int whatBlockToSpawn = UnityEngine.Random.Range(0, 100);

        if (whatBlockToSpawn < 80) //80% chance to spawn spawn BuildingBlock
        {
            generateBuildingBlock();
        }

        else 
        {
            generateDeathBlock(); //20% chance spawn DeathBlock
        }

        if (i%4 == 0 ) //Each fourth object, a coin is also spawned 10 rows up, in one of the three lanes.
        {
            int[] lanes = { 1, 2, 3 };
            generateCoin(8, lanes);
        }
        if (i%10==0 && i>0) //Each tenth object, a power up is also spawned, from side or on lane
        {
            generatePowerUp();
        }
    }


        //Generates a building block and instantiates it
        private void generateBuildingBlock()
    {

        int color = UnityEngine.Random.Range(1, 4);
        int speed = (UnityEngine.Random.Range(7, 9)) + level;
        int spawnPos = UnityEngine.Random.Range(0, 2);

        int[] arrayWithArguments = { speed, color, 0 }; //speed, color{1=blue,2=green,3=red}, direction{0=right,1=left};
        xPos = -25;
        if (spawnPos.Equals(1))
        {
            arrayWithArguments[2] = 1;
            xPos = 25;
        }
        yPos = yPos + yMult;
        Vector3 pos = new Vector3(xPos, yPos, 0);
        GameObject go = Instantiate(BBPrefab, pos, Quaternion.identity);
        go.SetActive(true);
        go.SendMessage("runThis", arrayWithArguments); //sends a array with arguments to that class
    }

    //Generates a death block and instantiates it
    private void generateDeathBlock()
    {
        int speed = (UnityEngine.Random.Range(8, 10)) + level;
        int spawnPos = UnityEngine.Random.Range(0, 2);
        int[] arrayWithArguments = { speed, 0 };
        xPos = -25;
        if (spawnPos.Equals(1))
        {
            arrayWithArguments[1] = 1;
            xPos = 25;
        }

        Vector3 pos = new Vector3(xPos, yPos + yMult, 0);
        GameObject go = Instantiate(DeathBlockPrefab, pos, Quaternion.identity);
        go.SetActive(true);
        go.SendMessage("runThis", arrayWithArguments); //sends a array with arguments to that class
    }

    //Generates a coin and instantiates it
    //levels = how many rows over the current row
    //lanes = a list of integers of the lanes it can spawn
    private void generateCoin(int level, int[] lanes)
    {
        var shuffledLanes = lanes.OrderBy(a => Guid.NewGuid()).ToList();
        int coinPos = shuffledLanes[0];
        int color = UnityEngine.Random.Range(1, 4);

        if (coinPos.Equals(1)) {xPos = -4.4f;} //Which lane to spawn on
        else if (coinPos.Equals(2)) {xPos = 0;}
        else {xPos = 4.4f;}

        Vector3 pos = new Vector3(xPos, yPos + yMult * level, -1);
        GameObject go = Instantiate(CoinPrefab, pos, Quaternion.identity);
        go.SetActive(true);

        go.SendMessage("runThis", color);  //sends a array with arguments to that class
    }


    //Generates a random powerup and instantiates it
    //Which type of powerup -> Speeder or Combo generator
    //Which type of spawn -> Side spawn or lane spawn
    //Side spawn -> left or right side
    //Lane spawn -> which lane to spawn on
    private void generatePowerUp()
    {
        int powerUpType = UnityEngine.Random.Range(0, 2); //0=combogenerator or 1=speeder
        int laneOrSide = UnityEngine.Random.Range(0, 2); //type of spawn, 0=side or 1=lane
        int whichSide = UnityEngine.Random.Range(0, 2); //spawnlocation, 0=left or 1=right
        int whichLane = UnityEngine.Random.Range(0, 3); //which lane to spawn on, 0=left,1=middle,2=right

        float yPosTemp = yPos + yMult + yMult;

        if (laneOrSide.Equals(0)) //Side spawn
        {
            int[] arrayWithArguments = {powerUpType, whichSide, laneOrSide};
            xPos = -25;
            if (whichSide.Equals(1)) //Right or left side?
            {
                arrayWithArguments[1] = 1;
                xPos = 25;
            }
        Vector3 pos = new Vector3(xPos, yPosTemp + yMult, -1);
        GameObject go = Instantiate(PowerUpPrefab, pos, Quaternion.identity);
        go.SetActive(true); 
        go.SendMessage("runThis", arrayWithArguments); //sends a array with arguments to that class

        }
        else  //Lane spawn
        {
            if (whichLane.Equals(1)) { xPos = -4.4f; } //Which lane to spawn on
            else if (whichLane.Equals(2)) { xPos = 0; }
            else { xPos = 4.4f; }

            int[] arrayWithArguments = {powerUpType, whichSide, laneOrSide};
            Vector3 pos = new Vector3(xPos, yPosTemp, -1);
            GameObject go = Instantiate(PowerUpPrefab, pos, Quaternion.identity);
            go.SetActive(true);
            go.SendMessage("runThis", arrayWithArguments);  //sends a array with arguments to that class
        }
    }

    //Updates the Score text
     private void updateScoreText()
    {
        scoreText.text = "Score: " + currentScore.ToString();
    }

    //Updates the GameOver-Score-text
    private void updateFinalScoreText()
    {
        scoreText_gameOver.text = "Score: " + currentScore.ToString();
        finalScoreSet = true;
    }

    //Updates the Level text
    private void updateLevelText()
    {
        levelText.text = "Level: " + level.ToString();
    }

    //Updates the Score-per-Coin text
    private void updateCoinText()
    {
        coinText.text = "Coin score: " + coinScore.ToString();

    }

    //Updates the Time text (in seconds)
    private void updateTimeText()
    {
        timeText.text = "Time: " + Mathf.Round(totalElapsed / 100);
    }

    //Increase the current level of hardness by shrinkin spawn timer
    private void increaseLevel()
    {
        level++;
        spawnTimer = Math.Pow(0.99, level) * spawnTimer; //Reduce spawnTimer as level increases. Each level increase will be 99% of the previous one
        updateLevelText();
    }


    //Sets the game to game over. Called by Avatar class
    private void AvatarSetGameOver()
    {
        gameOver = true;
    }

    //A coin was picked up by Avatar. Called by Avatar class
    private void AvatarCoinPickUp()
    {
        currentScore = (int) Mathf.Round(currentScore + (coinScore * globalMultiplier));
        updateScoreText();
    }

    //Avatar touched a building block. Called by Avatar class
    private void AvatarBlockTouch()
    {
        currentScore = currentScore + (int)Mathf.Round(globalMultiplier + 1);
        updateScoreText();
    }

    //Avatar touched a Combo Generator. Called by Avatar class
    private void AvatarComboTouch()
    {
        coinScore = (int)Mathf.Round((coinScore * 2)*globalMultiplier);
        updateCoinText();
    }


    //Avatar touched a Speeder. Called by Avatar class
    private void AvatarSpeederTouch()
    {
        globalMultiplier = globalMultiplier * 1.2f;
        increaseLevel();
        increaseLevel();

        coinScore = (int)Mathf.Round(coinScore * globalMultiplier);
        updateScoreText();
        updateCoinText();
    }


}



