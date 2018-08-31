using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Tile {

    public enum TileType {
        Empty,
        Floor
    };

    private TileType _type = TileType.Empty;

    Action<Tile> cbTileTypeChanged;

    LooseObject looseObject;
    InstalledObject installedObject;

    World world;
    int x, y;

    public TileType Type
    {
        get
        {
            return _type;
        }

        set
        {
            TileType oldType = _type;

            _type = value;
            if (cbTileTypeChanged != null && oldType != _type)
            {
                cbTileTypeChanged(this);
            }
        }
    }

    public int X
    {
        get
        {
            return x;
        }
    }

    public int Y
    {
        get
        {
            return y;
        }
    }

    public Tile(World world, int x, int y)
    {
        this.world = world;
        this.x = x;
        this.y = y;
    }

    public void RegisterTileTypeChangedCallback(Action<Tile> callback)
    {
        cbTileTypeChanged += callback;
    }

    public void UngisterTileTypeChangedCallback(Action<Tile> callback)
    {
        cbTileTypeChanged -= callback;
    }

    public bool PlaceObject(InstalledObject objInstance)
    {
        if(objInstance == null)
        {
            installedObject = null;
            return true;
        }

        if(installedObject != null)
        {
            Debug.LogError("Trying to assign an installed objecto to a tile that already has one!");
            return false;
        }

        installedObject = objInstance;
        return true;
    }

}
