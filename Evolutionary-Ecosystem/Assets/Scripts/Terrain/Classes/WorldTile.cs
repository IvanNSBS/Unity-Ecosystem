using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTile
{
    public Type type;

    public enum Type
    {
        Dirt,
        Grass,
        HighGrass,
        Sand,
        Water,
        Stone,
        Void
    }

    public WorldTile (Type type)
    {
        this.type = type;
    }
}
