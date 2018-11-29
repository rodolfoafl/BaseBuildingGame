using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType { Empty, Floor };

public class Tile {    

    TileType _type = TileType.Empty;

    Action<Tile> _cbTileChanged;

    LooseObject _looseObject;

    InstalledObject _installedObject;

    Job _pendingInstalledObjectJob;

    World _world;

    int _x;
    int _y;

    #region Properties
    public TileType Type
    {
        get
        {
            return _type;
        }

        set
        {
            if (Type != value)
            {
                _type = value;
                if (_cbTileChanged != null)
                {
                    _cbTileChanged(this);
                }
            }
        }
    }

    public int X
    {
        get
        {
            return _x;
        }
    }

    public int Y
    {
        get
        {
            return _y;
        }
    }

    public InstalledObject InstalledObject
    {
        get
        {
            return _installedObject;
        }
    }

    public World World
    {
        get
        {
            return _world;
        }
    }

    public Job PendingInstalledObjectJob
    {
        get
        {
            return _pendingInstalledObjectJob;
        }

        set
        {
            _pendingInstalledObjectJob = value;
        }
    }
    #endregion

    public Tile( World world, int x, int y)
    {
        this._world = world;
        this._x = x;
        this._y = y;
    }

    public bool AssignInstalledObject(InstalledObject objInstance)
    {
        if(objInstance == null)
        {
            _installedObject = null;
            return true;
        }

        if(_installedObject != null)
        {
            Debug.LogError("Trying to assign an installed object to a tile that already has one!");
            return false;
        }

        _installedObject = objInstance;
        return true;
    }

    #region Callbacks
    public void RegisterTileTypeChangedCallback(Action<Tile> callback)
    {
        _cbTileChanged += callback;
    }

    public void UnregisterTileTypeChangedCallback(Action<Tile> callback)
    {
        _cbTileChanged -= callback;
    }
    #endregion
}
