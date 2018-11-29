using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstalledObject{

    Tile _tile;

    Action<InstalledObject> _cbOnInstalledObjectChanged;

    string _objectType;

    float _movementCost;

    int _width;
    int _height;

    bool _linksToNeighbor = false;

    Func <Tile, bool> funcPositionValidation;

    #region Properties
    public string ObjectType
    {
        get
        {
            return _objectType;
        }
    }

    public Tile Tile
    {
        get
        {
            return _tile;
        }
    }

    public bool LinksToNeighbor
    {
        get
        {
            return _linksToNeighbor;
        }
    }
    #endregion

    protected InstalledObject()
    {

    }

    public static InstalledObject CreatePrototype(string objectType, float movementCost = 1f, int width = 1, int height = 1, bool linksToNeighbor = false)
    {
        InstalledObject obj = new InstalledObject();
        obj._objectType = objectType;
        obj._movementCost = movementCost;
        obj._width = width;
        obj._height = height;
        obj._linksToNeighbor = linksToNeighbor;

        obj.funcPositionValidation = obj._IsValidPosition;

        return obj;
    }

    public static InstalledObject PlaceInstance(InstalledObject proto, Tile tile)
    {
        if(!proto.funcPositionValidation(tile))
        {
            Debug.LogError("PlaceInstance -- Position Validity Function returned FALSE!");
            return null;
        }

        InstalledObject obj = new InstalledObject();

        obj._objectType = proto._objectType;
        obj._movementCost = proto._movementCost;
        obj._width = proto._width;
        obj._height = proto._height;
        obj._linksToNeighbor = proto._linksToNeighbor;

        obj._tile = tile;
        if (!tile.AssignInstalledObject(obj))
        {
            return null;
        }

        if (obj.LinksToNeighbor)
        {
            Tile t;
            int x = tile.X;
            int y = tile.Y;

            t = tile.World.GetTileAt(x, y + 1);
            if (t != null && t.InstalledObject != null && t.InstalledObject.ObjectType.Equals(obj.ObjectType))
            {
                t.InstalledObject._cbOnInstalledObjectChanged(t.InstalledObject);
            }

            t = tile.World.GetTileAt(x + 1, y);
            if (t != null && t.InstalledObject != null && t.InstalledObject.ObjectType.Equals(obj.ObjectType))
            {
                t.InstalledObject._cbOnInstalledObjectChanged(t.InstalledObject);
            }

            t = tile.World.GetTileAt(x, y - 1);
            if (t != null && t.InstalledObject != null && t.InstalledObject.ObjectType.Equals(obj.ObjectType))
            {
                t.InstalledObject._cbOnInstalledObjectChanged(t.InstalledObject);
            }

            t = tile.World.GetTileAt(x - 1, y);
            if (t != null && t.InstalledObject != null && t.InstalledObject.ObjectType.Equals(obj.ObjectType))
            {
                t.InstalledObject._cbOnInstalledObjectChanged(t.InstalledObject);
            }
        }

        return obj;
    }

    public bool IsValidPosition(Tile tile)
    {
        return funcPositionValidation(tile);
    }

    public bool _IsValidPosition(Tile tile)
    {
        if(tile.Type != TileType.Floor)
        {
            return false;
        }

        if(tile.InstalledObject != null)
        {
            return false;
        }

        return true;
    }

    #region Callbacks
    public void RegisterOnInstalledObjectChangedCallback(Action<InstalledObject> callback)
    {
        _cbOnInstalledObjectChanged += callback;
    }

    public void UnregisterOnInstalledObjectChangedCallback(Action<InstalledObject> callback)
    {
        _cbOnInstalledObjectChanged -= callback;
    }
    #endregion
}
