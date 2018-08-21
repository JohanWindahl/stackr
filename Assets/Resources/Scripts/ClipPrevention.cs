using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class represents a clipping prevention as a Buildingblock child, it moves along with the parent
public class ClipPrevention : MonoBehaviour {
    public Transform parent; //parents transform


    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        parent = transform.parent; //parent
        transform.position = Vector3.MoveTowards(transform.position, parent.transform.position, .03f); //center it on the parent
    }
}
