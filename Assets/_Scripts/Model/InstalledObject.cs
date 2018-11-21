using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstalledObject{

    Tile _tile;
    string _objectType;
    float _movementCost;
    int _width;
    int _height;

    public InstalledObject(string objectType, float movementCost = 1f, int width = 1, int height = 1)
    {
        this._objectType = objectType;
        this._movementCost = movementCost;
        this._width = width;
        this._height = height;
    }

    public InstalledObject(InstalledObject proto, Tile tile)
    {      
        this._objectType = proto._objectType;
        this._movementCost = proto._movementCost;
        this._width = proto._width;
        this._height = proto._height;

        this._tile = tile;
        //tile.InstalledObject = this;
    }
}
