using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LooseObject {

    string _objectType = "SteelPlate_";

    int _maxStackSize = 50;
    int _stackSize;

    Tile _tile;
    Character _character;

    #region Properties
    public string ObjectType
    {
        get
        {
            return _objectType;
        }

        set
        {
            _objectType = value;
        }
    }

    public int MaxStackSize
    {
        get
        {
            return _maxStackSize;
        }

        set
        {
            _maxStackSize = value;
        }
    }

    public int StackSize
    {
        get
        {
            return _stackSize;
        }

        set
        {
            _stackSize = value;
        }
    }

    public Tile Tile
    {
        get
        {
            return _tile;
        }

        set
        {
            _tile = value;
        }
    }

    public Character Character
    {
        get
        {
            return _character;
        }

        set
        {
            _character = value;
        }
    }
    #endregion

    public LooseObject()
    {

    }

    public LooseObject(string objectType, int maxStackSize, int stackSize)
    {
        this._objectType = objectType;
        this._maxStackSize = maxStackSize;
        this._stackSize = stackSize;
    }

    protected LooseObject(LooseObject other)
    {
        this._objectType = other._objectType;
        this._maxStackSize = other._maxStackSize;
        this._stackSize = other._stackSize;
    }

    public virtual LooseObject Clone()
    {
        return new LooseObject(this);
    }
}
