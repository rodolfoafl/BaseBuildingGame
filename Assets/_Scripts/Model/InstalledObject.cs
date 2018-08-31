using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstalledObject {

    Tile tile;

    string objectType;

    float movementCost;

    int width;
    int height;

    Action<InstalledObject> cbOnChanged;

    protected InstalledObject() {}

    public string ObjectType
    {
        get
        {
            return objectType;
        }
    }

    public Tile Tile
    {
        get
        {
            return tile;
        }
    }

    public static InstalledObject CreatePrototype(string objectType, float movementCost = 1f, int width = 1, int height = 1)
    {
        InstalledObject obj = new InstalledObject();

        obj.objectType = objectType;
        obj.movementCost = movementCost;
        obj.width = width;
        obj.height = height;

        return obj;
    }
    
    public static InstalledObject PlaceInstance(InstalledObject proto, Tile tile)
    {
        InstalledObject obj = CreatePrototype(proto.objectType, proto.movementCost, proto.width, proto.height);

        obj.tile = tile;

        if (!tile.PlaceObject(obj))
        {
            return null;
        }

        return obj;
    }

    public void RegisterOnChangedCallback(Action<InstalledObject> callback)
    {
        cbOnChanged += callback;
    }

    public void UnregisterOnChangedCallback(Action<InstalledObject> callback)
    {
        cbOnChanged -= callback;
    }

}


