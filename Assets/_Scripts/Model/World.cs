using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World {

    Tile[,] _tiles;

    Dictionary<string, InstalledObject> _installedObjectPrototypes;

    int _width;
    int _height;

    #region Properties
    public int Width
    {
        get
        {
            return _width;
        }
    }

    public int Height
    {
        get
        {
            return _height;
        }
    }
    #endregion

    public World(int width = 100, int height = 100)
    {
        this._width = width;
        this._height = height;

        _tiles = new Tile[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                _tiles[x, y] = new Tile(this, x, y);
            }
        }

        Debug.Log("World created with " + (width * height) + " tiles.");

        InitializeInstalledObjectPrototypesDictionary();
    }

    void InitializeInstalledObjectPrototypesDictionary()
    {
        _installedObjectPrototypes = new Dictionary<string, InstalledObject>();

        InstalledObject wallPrototype = InstalledObject.CreatePrototype("Wall", 0, 1, 1);

        _installedObjectPrototypes.Add("Wall", wallPrototype);
    }

    public void RandomizeTiles()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                if(UnityEngine.Random.Range(0, 2) == 0)
                {
                    _tiles[x, y].Type = TileType.Empty;
                }
                else
                {
                    _tiles[x, y].Type = TileType.Floor;
                }
               
            }
        }
    }

    public Tile GetTileAt(int x, int y)
    {
        if(x > _width || x < 0 || y > _height || y < 0)
        {
            Debug.LogError("Tile (" + x + ", " + y + ") is out of range.");
            return null;
        }
        return _tiles[x, y];
    }

    void PlaceInstalledObject(string buildModeObjectType, Tile t)
    {
        
    }
}
