using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
public class TileAutomata : MonoBehaviour
{
    [Header("Conways Settings")]
    [Range(0, 100)] public int initialChance;
    [Range(1, 8)] public int birthLimit, deathLimit;
    [Range(1, 10)] public int numberRepetitions;
    private int count = 0;

    [Header("Tilemap Settings")]
    public Vector3Int tileMapSize;
    public Tilemap topMap;
    public Tilemap botMap;
    public Tile topTile;
    public Tile botTile;
    private int[,] terrainMap;

    int width;
    int height;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            Simulate(numberRepetitions);

        if (Input.GetMouseButtonDown(1))
            ClearMap(true);
    }

    public void Simulate(int numberRepetitions)
    {
        ClearMap(false);
        width = tileMapSize.x;
        height = tileMapSize.y;

        if(terrainMap == null)
        {
            terrainMap = new int[width, height];
            initPos();
        }

        for (int i = 0; i < numberRepetitions; i++)
        {
            terrainMap = GenerateTilePosition(terrainMap);
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (terrainMap[x, y] == 1)
                {
                    topMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), topTile);
                }
                botMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), botTile);
            }
        }
    }

    public int [,] GenerateTilePosition(int[,] map)
    {
        int[,] newMap = new int[width, height];
        int neighbour;
        BoundsInt boundsInt = new BoundsInt(-1, -1, 0, 3, 3, 1);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                neighbour = 0;
                foreach(var b in boundsInt.allPositionsWithin)
                {
                    if (b.x == 0 && b.y == 0)
                        continue;

                    if (x + b.x >= 0 && x + b.x < width && y + b.y >= 0 && y + b.y < height)
                        neighbour += map[x + b.x, y + b.y];
                    else
                        neighbour++;
                }

                if (map[x, y] == 1)
                {
                    if (neighbour < deathLimit)
                        newMap[x, y] = 0;
                    else
                        newMap[x, y] = 1;
                }

                if (map[x, y] == 0)
                {
                    if (neighbour > birthLimit)
                        newMap[x, y] = 1;
                    else
                        newMap[x, y] = 0;
                }
            }
        }
        return newMap;
    }

    public void initPos()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                terrainMap[x, y] = Random.Range(1, 101) < initialChance ? 1 : 0;
            }
        }
    }

    public void ClearMap(bool complete)
    {
        topMap.ClearAllTiles();
        botMap.ClearAllTiles();
        if (complete)
        {
            terrainMap = null;
        }
    }
}
