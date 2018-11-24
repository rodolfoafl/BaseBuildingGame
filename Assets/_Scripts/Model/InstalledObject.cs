using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstalledObject{

    Tile _tile;
    Action<InstalledObject> cbOnInstalledObjectChanged;
    string _objectType;
    float _movementCost;
    int _width;
    int _height;

    bool _linksToNeighbor = false;

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

        return obj;
    }

    public static InstalledObject PlaceInstance(InstalledObject proto, Tile tile)
    {
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

        return obj;
    }

    #region Callbacks
    public void RegisterOnInstalledObjectChangedCallback(Action<InstalledObject> callback)
    {
        cbOnInstalledObjectChanged += callback;
    }

    public void UnregisterOnInstalledObjectChangedCallback(Action<InstalledObject> callback)
    {
        cbOnInstalledObjectChanged -= callback;
    }
    #endregion
}
