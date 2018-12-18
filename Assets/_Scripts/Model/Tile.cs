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
                return 1;
            }
            return 1 * InstalledObject.MovementCost;
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
