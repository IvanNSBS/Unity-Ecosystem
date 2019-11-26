using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public static World instance;
    Noise noise;

    [Header("World Settings")]
    public int width;
    public int height;
    public WorldTile[,] tiles;
    public Material material;
    public GameObject WaterPrefab;

    [Header("Noise Settings")]
    public string seed;
    public bool randomSeed;
    public float frequency;
    public float amplitude;
    public float lacunarity;
    public float persistance;
    public int octaves;

    [Header("Heights Settings")]
    public float seaLevel;
    public float dirtStartHeight;
    public float dirtEndHeight;
    public float grassStartHeight;
    public float grassEndHeight;
    public float highGrassStartHeight;
    public float highGrassEndHeight;

    [Header("Food Settings")]
    public int m_InitialFoodAmount = 20;
    public float m_SpawnPct = 0.05f;
    public GameObject m_FoodPrefab;
    public int actualFoodAmount = 0;
    [SerializeField]private int maximumFoodAmount;

    [Header("Rabbit Population Settings")]
    public int initialRabbitAmount;
    public GameObject rabbitPrefab;

    void Awake()
    {
        instance = this;

        if (randomSeed)
        {
            int value = Random.Range(-10000, 10000);
            seed = value.ToString();
        }

        noise = new Noise(seed.GetHashCode(), frequency, amplitude, lacunarity, persistance, octaves);
    }

    // Start is called before the first frame update
    void Start()
    {
        CreateTiles();
        SubdivideTilesArray();
        SpawnInitialFood();
        SpawnInitialRabbitPopulation();
    }

    // Update is called once per frame
    void Update()
    {
        if(actualFoodAmount <= maximumFoodAmount)
        {
            if (Random.Range(0.0f, 1.0f) < m_SpawnPct)
            {
                int foodX, foodY;
                do
                {
                    foodX = Random.Range(0, width);
                    foodY = Random.Range(0, height);

                } while (GetTileAt(foodX, foodY).type == WorldTile.Type.Water || GetTileAt(foodX, foodY).type == WorldTile.Type.Void);
                var obj = Instantiate(m_FoodPrefab);
                obj.gameObject.transform.position = new Vector2(foodX + 0.5f, foodY + 0.5f);
                actualFoodAmount++;
            }
        }
        
    }

    void CreateTiles()
    {
        tiles = new WorldTile[width, height];
        float[,] noiseValues = noise.GetNoiseValues(width, height);

        for(int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                tiles[i, j] = MakeTileAtHeight(noiseValues[i, j]);
            }
        }

        SpawnWaterPrefab();
    }

    WorldTile MakeTileAtHeight (float currentHeight)
    {
        if (currentHeight <= seaLevel)
            return new WorldTile(WorldTile.Type.Water);

        if (currentHeight >= grassStartHeight && currentHeight <= grassEndHeight)
            return new WorldTile(WorldTile.Type.Grass);

        if (currentHeight >= highGrassStartHeight && currentHeight <= highGrassEndHeight)
            return new WorldTile(WorldTile.Type.HighGrass);

        if (currentHeight >= dirtStartHeight && currentHeight <= dirtEndHeight)
            return new WorldTile(WorldTile.Type.Dirt);

        return new WorldTile(WorldTile.Type.Void);
    }

    void SubdivideTilesArray (int index1 = 0, int index2 = 0)
    {
        if (index1 > tiles.GetLength(0) && index2 > tiles.GetLength(1))
            return;

        int sizeX, sizeY;

        if (tiles.GetLength(0) - index1 > 100)
            sizeX = 100;
        else
            sizeX = tiles.GetLength(0) - index1;

        if (tiles.GetLength(1) - index2 > 100)
            sizeY = 100;
        else
            sizeY = tiles.GetLength(1) - index2;

        GenerateMesh(index1, index2, sizeX, sizeY);

        if (tiles.GetLength(0) > index1 + 100)
        {
            SubdivideTilesArray(index1 + 100, index2);
            return;
        }

        if (tiles.GetLength(1) > index2 + 100)
        {
            SubdivideTilesArray(0, index2 + 100);
            return;
        }
    }

    void GenerateMesh(int x, int y, int width, int height)
    {
        MeshData data = new MeshData(x, y, width, height);

        GameObject meshGameObject = new GameObject("Chunk " + x + ", " + y);
        meshGameObject.transform.SetParent(this.transform);

        MeshFilter filter = meshGameObject.AddComponent<MeshFilter>();
        MeshRenderer renderer = meshGameObject.AddComponent<MeshRenderer>();
        renderer.material = material;

        Mesh mesh = filter.mesh;
        mesh.vertices = data.vertices.ToArray();
        mesh.triangles = data.triangles.ToArray();
        mesh.uv = data.UVs.ToArray();
    }

    public WorldTile GetTileAt(int x, int y)
    {
        if(x < 0 || x >= width || y < 0 || y >= height)
        {
            return null;
        }
        return tiles[x, y];
    }

    void SpawnWaterPrefab()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (tiles[i, j].type == WorldTile.Type.Water)
                {
                    Vector3 position = new Vector3(i + 0.5f, j + 0.5f, 0);
                    GameObject go = Instantiate(WaterPrefab, position,Quaternion.identity);
                    go.transform.SetParent(this.gameObject.transform);
                }
            }
        }
    }

    void SpawnInitialFood()
    {
        for (int i = 0; i < m_InitialFoodAmount; i++)
        {
            int foodX, foodY;
            do
            {
                foodX = Random.Range(0, width);
                foodY = Random.Range(0, height);

            } while (GetTileAt(foodX, foodY).type == WorldTile.Type.Water || GetTileAt(foodX, foodY).type == WorldTile.Type.Void);
            var obj = Instantiate(m_FoodPrefab);
            obj.gameObject.transform.position = new Vector2(foodX + 0.5f, foodY + 0.5f);
            actualFoodAmount++;
        }
    }

    void SpawnInitialRabbitPopulation()
    {
        for (int i = 0; i < initialRabbitAmount; i++)
        {
            int rabbitX, rabbitY;
            do
            {
                rabbitX = Random.Range(3, width - 3);
                rabbitY = Random.Range(3, height - 3);

            } while (GetTileAt(rabbitX, rabbitY).type == WorldTile.Type.Water || GetTileAt(rabbitX, rabbitY).type == WorldTile.Type.Void);
            var obj = Instantiate(rabbitPrefab);
            obj.gameObject.transform.position = new Vector2(rabbitX + 0.5f, rabbitY + 0.5f);
            obj.GetComponent<LifeComponent>().curTimeToAdulthood = 1.0f;
        }
    }
}
