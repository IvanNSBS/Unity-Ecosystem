using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteLoader : MonoBehaviour
{
    public static SpriteLoader instance;

    Dictionary<string, Vector2[]> tileUVMap;

    void Awake()
    {
        instance = this;
        tileUVMap = new Dictionary<string, Vector2[]>();
        Sprite[] sprites = Resources.LoadAll<Sprite>("New Sprites");

        float imageWidth = 0f;
        float imageHeight = 0f;

        foreach (Sprite sprite in sprites)
        {
            if (sprite.rect.x + sprite.rect.width > imageWidth)
                imageWidth = sprite.rect.x + sprite.rect.width;

            if (sprite.rect.y + sprite.rect.height > imageHeight)
                imageHeight = sprite.rect.y + sprite.rect.height;
        }

        foreach(Sprite sprite in sprites)
        {
            Vector2[] uvs = new Vector2[4];
            uvs[0] = new Vector2(sprite.rect.x / imageWidth, sprite.rect.y / imageHeight);
            uvs[1] = new Vector2((sprite.rect.x + sprite.rect.width) / imageWidth, sprite.rect.y / imageHeight);
            uvs[2] = new Vector2(sprite.rect.x / imageWidth, (sprite.rect.y + sprite.rect.height) / imageHeight);
            uvs[3] = new Vector2((sprite.rect.x + sprite.rect.width) / imageWidth, (sprite.rect.y + sprite.rect.height) / imageHeight);

            tileUVMap.Add(sprite.name, uvs);
        }
    }

    public Vector2[] GetTileUVs(WorldTile tile)
    {
        string key = tile.type.ToString();

        if (tileUVMap.ContainsKey(key) == true)
        {
            if (tile.type == WorldTile.Type.Water)
            {
                int random = Random.Range(0, 2);
                if (random != 0)
                    key = key + random.ToString();
                Debug.Log(key);
            }
            return tileUVMap[key];
        }
        else
        {
            Debug.LogError("There is no UV map for tile type: " + key);
            return tileUVMap ["Void"];
        }
    }
}
