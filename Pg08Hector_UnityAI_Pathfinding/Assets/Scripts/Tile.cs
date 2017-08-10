using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;

public class Tile : MonoBehaviour {

    //Action is a built-in delegate, that allows us to define which parameters are sent with our event
    public static event Action<int, int> onClicked;

    internal int posX;
    internal int posY;

    //We store the tile that was before this tile, so when we reach our destination we can backtrack to the start
    internal Tile previousTile;

    internal bool isWall;

    private float wallChance = 0.3f;

	// Use this for initialization
	void Start () {
        isWall = Random.value < wallChance;
        if (isWall)
            GetComponent<SpriteRenderer>().color = Color.red;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //This method is called when you click an object (using raycast)
    void OnMouseDown() {
        //Before we call an event, we ALWAYS have to check if its not null
        //Events are null when no other objects are listening
        if (onClicked != null)
            onClicked(posX, posY);
    }
}
