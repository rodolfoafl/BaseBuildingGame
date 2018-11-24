using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World {

    Tile[,] _tiles;

    Dictionary<string, InstalledObject> _installedObjectPrototypes;

    Action<InstalledObject> cbInstalledObjectCreated;

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

        InstalledObject wallPrototype = InstalledObject.CreatePrototype("Wall", 0, 1, 1, true);

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

    //Assumes 1x1 tiles.
    public void PlaceInstalledObject(string objectType, Tile t)
    {
        InstalledObject instObj = _installedObjectPrototypes[objectType];
        if (!_installedObjectPrototypes.TryGetValue(objectType, out instObj))
        {
            Debug.LogError("_installedObjectPrototypes doesn't contain the objectType!");
            return;
        }

        InstalledObject obj = InstalledObject.PlaceInstance(instObj, t);
        if(obj == null)
        {
            //Failed to place object. Most likely there was already something there.
            return;
        }

        if(cbInstalledObjectCreated != null)
        {
            cbInstalledObjectCreated(obj);
        }
    }

    #region Callbacks
    public void RegisterInstalledObjectCreated(Action<InstalledObject> callback)
    {
        cbInstalledObjectCreated += callback;
    }

    public void UnregisterInstalledObjectCreated(Action<InstalledObject> callback)
    {
        cbInstalledObjectCreated -= callback;
    }
    #endregion
}
