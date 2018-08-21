using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This class represents the camera movement
public class CameraSystem : MonoBehaviour {

    private GameObject player; 

    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player"); //find player
    }

    // Update is called once per frame
    void LateUpdate () {
        transform.position = new Vector3(transform.position.x, player.transform.position.y+5,-10); //move camera with player
    }
}
