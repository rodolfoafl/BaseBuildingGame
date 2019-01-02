using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using UnityEngine;

public enum TileType { Empty, Floor };

public enum EnterableState { Yes, No, Soon };

public class Tile {

    Room _room;

    TileType _type = TileType.Empty;

    Action<Tile> _cbTileChanged;

    LooseObject _looseObject; 

    InstalledObject _installedObject;

    Job _pendingInstalledObjectJob;

    World _world;

    int _x;
    int _y;

    float baseTileMovementCost = 1;

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

        protected set
        {
            _x = value;
        }
    }

    public int Y
    {
        get
        {
            return _y;
        }

        protected set
        {
            _y = value;
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

    public float MovementCost
    {
        get
        {
            if(Type == TileType.Empty)
            {
                return 0;
            }
            if(InstalledObject == null)
            {
                return baseTileMovementCost;
            }
            return baseTileMovementCost * InstalledObject.MovementCost;
        }
    }

    public Room Room
    {
        get
        {
            return _room;
        }

        set
        {
            _room = value;
        }
    }

    public LooseObject LooseObject
    {
        get
        {
            return _looseObject;
        }

        set
        {
            _looseObject = value;
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

    public bool AssignLooseObject(LooseObject objInstance)
    {
        if(objInstance == null)
        {
            _looseObject = null;
            return true;
        }

        if(_looseObject != null)
        {
            if(_looseObject.ObjectType != objInstance.ObjectType)
            {
                Debug.LogError("Different looseObject type. Can't assign it to the tile!");
                return false;
            }

            int quantToMove = objInstance.StackSize;
            if(_looseObject.StackSize + quantToMove > _looseObject.MaxStackSize)
            {
                quantToMove = _looseObject.MaxStackSize - _looseObject.StackSize;
            }

            _looseObject.StackSize += quantToMove;
            objInstance.StackSize -= quantToMove;
            return true;
        }

        _looseObject = objInstance.Clone();
        _looseObject.Tile = this;
        objInstance.StackSize = 0;
        return true;
    }

    public bool IsNeighbour(Tile tile, bool diagonalOk = false)
    {
        if(this.X == tile.X && (Mathf.Abs(this.Y - tile.Y) == 1))
        {
            return true;
        }

        if (this.Y == tile.Y && (Mathf.Abs(this.X - tile.X) == 1))
        {
            return true;
        }

        if (diagonalOk)
        {
            if (this.X == tile.X + 1 && (this.Y == tile.Y + 1 || this.Y == tile.Y - 1))
            {
                return true;
            }

            if (this.X == tile.X - 1 && (this.Y == tile.Y + 1 || this.Y == tile.Y - 1))
            {
                return true;
            }
        }

        return false;
    }

    public Tile[] GetNeighbours(bool diagonalOk = false)
    {
        Tile[] neighbours;
        if (!diagonalOk)
        {
            neighbours = new Tile[4];
        }
        else
        {
            neighbours = new Tile[8];
        }

        Tile neighbour;
        neighbour = World.GetTileAt(X, Y + 1);
        neighbours[0] = neighbour;
        neighbour = World.GetTileAt(X + 1, Y);
        neighbours[1] = neighbour;
        neighbour = World.GetTileAt(X, Y - 1);
        neighbours[2] = neighbour;
        neighbour = World.GetTileAt(X - 1, Y);
        neighbours[3] = neighbour;

        if (diagonalOk)
        {
            neighbour = World.GetTileAt(X + 1, Y + 1);
            neighbours[4] = neighbour;
            neighbour = World.GetTileAt(X + 1, Y - 1);
            neighbours[5] = neighbour;
            neighbour = World.GetTileAt(X - 1, Y - 1);
            neighbours[6] = neighbour;
            neighbour = World.GetTileAt(X - 1, Y + 1);
            neighbours[7] = neighbour;
        }

        return neighbours;
    }

    public EnterableState CheckEnterableState()
    {
        if(MovementCost == 0)
        {
            return EnterableState.No;
        }

        if(_installedObject != null && _installedObject._checkEnterableState != null)
        {
            return _installedObject._checkEnterableState(_installedObject);
        }

        return EnterableState.Yes;
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

    #region Saving & Loading

    public Tile()
    {

    }

    public XmlSchema GetSchema()
    {
        return null;
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteAttributeString("X", X.ToString());
        writer.WriteAttributeString("Y", Y.ToString());
        writer.WriteAttributeString("Type", Type.ToString());
    }

    public void ReadXml(XmlReader reader)
    {
        string t = reader.GetAttribute("Type");
        Type = (TileType)Enum.Parse(typeof(TileType), t);
    }

    #endregion

}
