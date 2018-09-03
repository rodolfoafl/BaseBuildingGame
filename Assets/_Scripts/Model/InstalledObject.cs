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

    bool linksToNeighbour = false;

    Action<InstalledObject> cbOnChanged;

    Func<Tile, bool> funcPositionValidation;

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

    public bool LinksToNeighbour
    {
        get
        {
            return linksToNeighbour;
        }

        set
        {
            linksToNeighbour = value;
        }
    }

    public static InstalledObject CreatePrototype(string objectType, float movementCost = 1f, int width = 1, int height = 1, bool linksToNeighbour = false)
    {
        InstalledObject obj = new InstalledObject();

        obj.objectType = objectType;
        obj.movementCost = movementCost;
        obj.width = width;
        obj.height = height;
        obj.linksToNeighbour = linksToNeighbour;

        obj.funcPositionValidation = obj.IsValidPosition;

        return obj;
    }
    
    public static InstalledObject PlaceInstance(InstalledObject proto, Tile tile)
    {
        if(!proto.funcPositionValidation(tile))
        {
            Debug.LogError("Position validity returned FALSE!");
            return null;
        }

        InstalledObject obj = CreatePrototype(proto.objectType, proto.movementCost, proto.width, proto.height, proto.linksToNeighbour);

        obj.tile = tile;

        if (!tile.PlaceObject(obj))
        {
            return null;
        }

        if (obj.LinksToNeighbour)
        {
            int x = tile.X;
            int y = tile.Y;

            Tile t;
            t = tile.World.GetTileAt(x, y + 1);
            if (t != null && t.InstalledObject != null && t.InstalledObject.ObjectType == obj.ObjectType)
            {
                t.InstalledObject.cbOnChanged(t.InstalledObject);
            }
            t = tile.World.GetTileAt(x + 1, y);
            if (t != null && t.InstalledObject != null && t.InstalledObject.ObjectType == obj.ObjectType)
            {
                t.InstalledObject.cbOnChanged(t.InstalledObject);
            }
            t = tile.World.GetTileAt(x, y - 1);
            if (t != null && t.InstalledObject != null && t.InstalledObject.ObjectType == obj.ObjectType)
            {
                t.InstalledObject.cbOnChanged(t.InstalledObject);
            }
            t = tile.World.GetTileAt(x - 1, y);
            if (t != null && t.InstalledObject != null && t.InstalledObject.ObjectType == obj.ObjectType)
            {
                t.InstalledObject.cbOnChanged(t.InstalledObject);
            }
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

    public bool IsValidPosition(Tile t)
    {
        if(t.Type != Tile.TileType.Floor)
        {
            return false;
        }

        if(t.InstalledObject != null)
        {
            return false;
        }
        return true;
    }

}


