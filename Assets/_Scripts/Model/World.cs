using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World {

    Tile[,] _tiles;
    List<Character> _characters;

    Dictionary<string, InstalledObject> _installedObjectPrototypes;

    Action<InstalledObject> _cbInstalledObjectCreated;
    Action<Character> _cbCharacterCreated;
    Action<Tile> _cbTileChanged;

    JobQueue _jobQueue;

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

    public JobQueue JobQueue
    {
        get
        {
            return _jobQueue;
        }

        set
        {
            _jobQueue = value;
        }
    }
    #endregion

    public World(int width = 100, int height = 100)
    {
        this._width = width;
        this._height = height;

        _tiles = new Tile[width, height];

        _jobQueue = new JobQueue();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                _tiles[x, y] = new Tile(this, x, y);
                _tiles[x, y].RegisterTileTypeChangedCallback(OnTileChanged);
            }
        }

        Debug.Log("World created with " + (width * height) + " tiles.");

        InitializeInstalledObjectPrototypesDictionary();

        _characters = new List<Character>();
    }

    public void Update(float deltaTime)
    {
        foreach(Character c in _characters)
        {
            c.Update(deltaTime);
        }
    }

    public Character CreateCharacter(Tile tile)
    {
        Character newCharacter = new Character(tile);
        _characters.Add(newCharacter);
        if (_cbCharacterCreated != null)
        {
            _cbCharacterCreated(newCharacter);
        }

        return newCharacter;
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
    public void PlaceInstalledObject(string objectType, Tile tile)
    {
        InstalledObject instObj;
        if (!_installedObjectPrototypes.TryGetValue(objectType, out instObj))
        {
            Debug.LogError("_installedObjectPrototypes doesn't contain the objectType!");
            return;
        }

        InstalledObject obj = InstalledObject.PlaceInstance(instObj, tile);
        if(obj == null)
        {
            //Failed to place object. Most likely there was already something there.
            return;
        }

        if(_cbInstalledObjectCreated != null)
        {
            _cbInstalledObjectCreated(obj);
        }
    }

    void OnTileChanged(Tile t)
    {
        if (_cbTileChanged == null)
        {
            return;
        }
        _cbTileChanged(t);
    }

    public bool IsInstalledObjectPlacementValid(string installedObjectType, Tile tile)
    {
        return _installedObjectPrototypes[installedObjectType].IsValidPosition(tile);
    }

    public InstalledObject GetInstalledObjectPrototype(string objectType)
    {
        InstalledObject instObj;
        if (!_installedObjectPrototypes.TryGetValue(objectType, out instObj))
        {
            Debug.LogError("_installedObjectPrototypes doesn't contain the objectType!");
            return null;
        }
        return _installedObjectPrototypes[objectType];
    }

    public void SetupPathfindingExample()
    {
        int l = Width / 2 - 5;
        int b = Height / 2 - 5;

        for (int x = l - 5; x < l + 15; x++)
        {
            for (int y = b - 5; y < b + 15; y++)
            {
                _tiles[x, y].Type = TileType.Floor;

                if(x == l || x == (l + 9) || y == b || y == (b + 9))
                {
                    if(x != (l + 9) && y != (b + 4))
                    {
                        PlaceInstalledObject("Wall", _tiles[x, y]);
                    }
                }
            }
        }
    }

    #region Callbacks
    public void RegisterInstalledObjectCreated(Action<InstalledObject> callback)
    {
        _cbInstalledObjectCreated += callback;
    }

    public void UnregisterInstalledObjectCreated(Action<InstalledObject> callback)
    {
        _cbInstalledObjectCreated -= callback;
    }

    public void RegisterTileChanged(Action<Tile> callback)
    {
        _cbTileChanged += callback;
    }

    public void UnregisterTileChanged(Action<Tile> callback)
    {
        _cbTileChanged -= callback;
    }

    public void RegisterCharacterCreated(Action<Character> callback)
    {
        _cbCharacterCreated += callback;
    }

    public void UnregisterCharacterCreated(Action<Character> callback)
    {
        _cbCharacterCreated -= callback;
    }
    #endregion


}
