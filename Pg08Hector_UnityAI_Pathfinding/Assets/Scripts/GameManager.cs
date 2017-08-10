using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    public Tile tilePrefab;

    public int gridSizeX;
    public int gridSizeY;

    private Tile[,] tiles;

    private Queue<Tile> frontier;

    //Its called when the object is instantiated and when the object is activated (after being deactivated)
    void OnEnable() {
        Tile.onClicked += CreatePath;
    }

    //Its called when the object is destroyed and when the object is deactivated (after being activated)
    //We always add listeners in OnEnable and we have to remove them in OnDisable
    //If we dont remove the listener it can lead to errors and memory leaks
    void OnDisable() {
        Tile.onClicked -= CreatePath;
    }

    // Use this for initialization
    void Start () {
        tiles = new Tile[gridSizeX, gridSizeY];
        //We loop through gridSizeX, to place the tiles horizontally
        for (int i = 0; i < gridSizeX; i++) {
            //We loop through gridSizeY, to place the tiles vertically
            for (int j = 0; j < gridSizeY; j++) {
                Tile tileClone = Instantiate(tilePrefab);
                tileClone.posX = i;
                tileClone.posY = j;
                tileClone.transform.position = new Vector3(i, j);
                tiles[i, j] = tileClone;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void CreatePathOld(int tileX, int tileY) {
        print("Tile clicked:\nX: "+tileX+" | Y: "+tileY);

        //Before we do our search, we clear all previous data
        foreach (Tile tile in tiles) {
            tile.previousTile = null;
            if (!tile.isWall)
                tile.GetComponent<SpriteRenderer>().color = Color.white;
        }

        Tile startTile = tiles[0, 0];
        frontier = new Queue<Tile>();
        frontier.Enqueue(startTile);

        //We keep searching until our frontier is empty meaning there is nothng left to search
        while (frontier.Count > 0) {
            //We get the first tile from our queue
            Tile currentTile = frontier.Dequeue();
            //If the current tile is the rile that was clicked, we have reached our destination
            if (currentTile == tiles[tileX, tileY]) {
                print("Destination reached");
                startTile.GetComponent<SpriteRenderer>().color = Color.green;
                while (currentTile != startTile && currentTile != null) {
                    currentTile.GetComponent<SpriteRenderer>().color = Color.green;
                    //We set our current tile to be the previous tile
                    currentTile = currentTile.previousTile;
                }
                return;
            }
            SearchSurroundingTilesOld(currentTile);
        }
    }

    void SearchSurroundingTilesOld(Tile origin) {
        //We check the tiles to the right, top, left and bottom
        SearchAdjecentTilesOld(origin, 1, 0);
        SearchAdjecentTilesOld(origin, 0, 1);
        SearchAdjecentTilesOld(origin, -1, 0);
        SearchAdjecentTilesOld(origin, 0, -1);
    }

    void SearchAdjecentTilesOld(Tile origin, int dirX, int dirY) {
        int newX = origin.posX + dirX;
        int newY = origin.posY + dirY;
        //If newX/newY is out of bounds, return
        if (newX < 0 || newY < 0 || newX >= gridSizeX || newY >= gridSizeY)
            return;
        //We get the adjecent tile using the original position and the direction in the tiles array
        Tile adjecentTile = tiles[newX, newY];
        //If the tile has been searched already, we dont want to add it to the frontier
        if (adjecentTile.previousTile != null || adjecentTile.isWall)
            return;

        //We store the origin as the previous tile for the newly found tile
        adjecentTile.previousTile = origin;
        adjecentTile.GetComponent<SpriteRenderer>().color = Color.green;
        frontier.Enqueue(adjecentTile);
    }

    void CreatePath(int tileX, int tileY) {
        StopAllCoroutines();
        StartCoroutine(CreatePathCoroutine(tileX, tileY));
    }

    IEnumerator CreatePathCoroutine(int tileX, int tileY) {
        print("Tile clicked:\nX: " + tileX + " | Y: " + tileY);

        //Before we do our search, we clear all previous data
        foreach (Tile tile in tiles)
        {
            tile.previousTile = null;
            if (!tile.isWall)
                tile.GetComponent<SpriteRenderer>().color = Color.white;
        }

        Tile startTile = tiles[0, 0];
        frontier = new Queue<Tile>();
        frontier.Enqueue(startTile);

        //We keep searching until our frontier is empty meaning there is nothng left to search
        while (frontier.Count > 0)
        {
            //We get the first tile from our queue
            Tile currentTile = frontier.Dequeue();
            //If the current tile is the rile that was clicked, we have reached our destination
            if (currentTile == tiles[tileX, tileY])
            {
                print("Destination reached");
                startTile.GetComponent<SpriteRenderer>().color = Color.green;
                while (currentTile != startTile && currentTile != null)
                {
                    currentTile.GetComponent<SpriteRenderer>().color = Color.green;
                    //We set our current tile to be the previous tile
                    currentTile = currentTile.previousTile;
                    yield return null;
                }
                yield break;
            }
            yield return StartCoroutine(SearchSurroundingTilesCoroutine(currentTile));
        }
    }

    IEnumerator SearchSurroundingTilesCoroutine(Tile origin) {
        //We check the tiles to the right, top, left and bottom
        yield return StartCoroutine(SearchAdjecentTilesCoroutine(origin, 1, 0));
        yield return StartCoroutine(SearchAdjecentTilesCoroutine(origin, 0, 1));
        yield return StartCoroutine(SearchAdjecentTilesCoroutine(origin, -1, 0));
        yield return StartCoroutine(SearchAdjecentTilesCoroutine(origin, 0, -1));
    }

    IEnumerator SearchAdjecentTilesCoroutine(Tile origin, int dirX, int dirY) {
        int newX = origin.posX + dirX;
        int newY = origin.posY + dirY;
        //If newX/newY is out of bounds, return
        if (newX < 0 || newY < 0 || newX >= gridSizeX || newY >= gridSizeY)
            yield break;
        //We get the adjecent tile using the original position and the direction in the tiles array
        Tile adjecentTile = tiles[newX, newY];
        //If the tile has been searched already, we dont want to add it to the frontier
        if (adjecentTile.previousTile != null || adjecentTile.isWall)
            yield break;

        //We store the origin as the previous tile for the newly found tile
        adjecentTile.previousTile = origin;
        adjecentTile.GetComponent<SpriteRenderer>().color = Color.blue;
        frontier.Enqueue(adjecentTile);
        yield return null;
    }
}
