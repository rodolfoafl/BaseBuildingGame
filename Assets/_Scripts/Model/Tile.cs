using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile {

    public enum TileType { Empty, Floor };

    TileType _type = TileType.Empty;

    Action<Tile> _cbTileTypeChanged;

    LooseObject _looseObject;
    InstalledObject _installedObject;

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
                if (_cbTileTypeChanged != null)
                {
                    _cbTileTypeChanged(this);
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
    #endregion

    public Tile( World world, int x, int y)
    {
        this._world = world;
        this._x = x;
        this._y = y;
    }

    public void RegisterTileTypeChangedCallback(Action<Tile> callback)
    {
        _cbTileTypeChanged += callback;
    }

    public void UnregisterTileTypeChangedCallback(Action<Tile> callback)
    {
        _cbTileTypeChanged -= callback;
    }
}
