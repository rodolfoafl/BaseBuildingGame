using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World {
    Tile[,] tiles;
    Dictionary<string, InstalledObject> installedObjectsPrototypes;

    Action<InstalledObject> cbInstalledObjectCreated;
    Action<Tile> cbTileChanged;

    public Queue<Job> jobQueue;

    int width;
    int height;

    public int Width
    {
        get
        {
            return width;
        }
    }

    public int Height
    {
        get
        {
            return height;
        }
    }

    public World(int width = 100, int height = 100)
    {
        jobQueue = new Queue<Job>();

        this.width = width;
        this.height = height;

        tiles = new Tile[width, height];
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                tiles[x, y] = new Tile(this, x, y);
                tiles[x, y].RegisterTileTypeChangedCallback(OnTileChanged);
            }
        }

        Debug.Log("World created with " + (width * height) + " tiles.");

        CreateInstalledObjectPrototypes();
    }

    void CreateInstalledObjectPrototypes()
    {
        installedObjectsPrototypes = new Dictionary<string, InstalledObject>();
        installedObjectsPrototypes.Add("Wall", InstalledObject.CreatePrototype("Wall", 0, 1, 1, true));
    }

    public void PlaceInstalledObject(string objectType, Tile t)
    {
        if (!installedObjectsPrototypes.ContainsKey(objectType))
        {
            Debug.LogError("installedObjectPrototypes doesn't contain a proto for key: " + objectType);
            return;
        }

        InstalledObject obj = InstalledObject.PlaceInstance(installedObjectsPrototypes[objectType], t);
        if(obj == null)
        {
            return;
        }

        if(cbInstalledObjectCreated != null)
        {
            cbInstalledObjectCreated(obj);
        }
    }

    public void RandomizeTiles()
    {
        Debug.Log("RandomizeTiles");
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(UnityEngine.Random.Range(0, 2) == 0)
                {
                    tiles[x, y].Type = Tile.TileType.Empty;
                }
                else
                {
                    tiles[x, y].Type = Tile.TileType.Floor;
                }
            }
        }
    }

    public Tile GetTileAt(int x, int y)
    {
        if(x > width || x < 0 || y > height || y < 0)
        {
            Debug.LogError("Tile (" + x + "," + y + ") is out of range.");
            return null;
        }
        return tiles[x, y];
    }

    public void RegisterInstalledObjectCreated(Action<InstalledObject> callback)
    {
        cbInstalledObjectCreated += callback;
    }

    public void UnregisterInstalledObjectCreated(Action<InstalledObject> callback)
    {
        cbInstalledObjectCreated -= callback;
    }

    public void RegisterTileChanged(Action<Tile> callback)
    {
        cbTileChanged += callback;
    }

    public void UnregisterTileChanged(Action<Tile> callback)
    {
        cbTileChanged -= callback;
    }

    void OnTileChanged(Tile t)
    {
        if(cbTileChanged == null)
        {
            return; 
        }
        cbTileChanged(t);
    }

    public bool IsInstalledObjectPlacementValid(string installedObjectType, Tile t)
    {
        return installedObjectsPrototypes[installedObjectType].IsValidPosition(t);
    }
}
