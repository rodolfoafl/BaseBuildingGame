using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Room {

    Dictionary<string, float> _atmosphericGasses;

    List<Tile> _tiles;

    World _world;

    #region Properties
    public List<Tile> Tiles
    {
        get
        {
            return _tiles;
        }

        set
        {
            _tiles = value;
        }
    }
    #endregion

    public Room(World world)
    {
        this._world = world;
        _tiles = new List<Tile>();
        _atmosphericGasses = new Dictionary<string, float>();
    }

    public void AssignTile(Tile tile)
    {
        if (_tiles.Contains(tile))
        {
            return;
        }

        if(tile.Room != null)
        {
            tile.Room.Tiles.Remove(tile);
        }

        tile.Room = this;
        _tiles.Add(tile);
    }

    public void UnassingnAllTiles()
    {
        for (int i = 0; i < _tiles.Count; i++)
        {
            _tiles[i].Room = _tiles[i].World.GetOutsideRoom();
        }
        _tiles = new List<Tile>();
    }

    public static void DoRoomFloodFill(InstalledObject sourceObject)
    {
        World world = sourceObject.Tile.World;
        Room oldRoom = sourceObject.Tile.Room;

        Tile[] neighbours = sourceObject.Tile.GetNeighbours(true);
        foreach(Tile n in neighbours)
        {
            FloodFill(n, oldRoom);
        }

        sourceObject.Tile.Room = null;
        oldRoom.Tiles.Remove(sourceObject.Tile);

        if (!oldRoom.IsOutsideRoom())
        {
            if(oldRoom.Tiles.Count > 0)
            {
                Debug.LogError("oldRoom still has tiles assigned to him!");
            }
            world.DeleteRoom(oldRoom);
        }
    }

    static void FloodFill(Tile tile, Room oldRoom)
    {
        if (tile == null
            || tile.Room != oldRoom
            || (tile.InstalledObject != null && tile.InstalledObject.RoomEnclosure)
            || tile.Type == TileType.Empty)
        {
            return;
        }

        Room newRoom = new Room(oldRoom._world);
        Queue<Tile> tilesToCheck = new Queue<Tile>();
        tilesToCheck.Enqueue(tile);

        while(tilesToCheck.Count > 0)
        {
            Tile t = tilesToCheck.Dequeue();            

            if (t.Room == oldRoom)
            {
                newRoom.AssignTile(t);

                Tile[] neighbours = t.GetNeighbours(true);
                foreach(Tile n in neighbours)
                {
                    if(n == null || n.Type == TileType.Empty)
                    {
                        newRoom.UnassingnAllTiles();
                        return;
                    }

                    if(n != null && n.Room == oldRoom 
                        && (n.InstalledObject == null || !n.InstalledObject.RoomEnclosure))
                    {
                        tilesToCheck.Enqueue(n);
                    }
                }                
            }
        }

        newRoom.CopyGas(oldRoom);

        tile.World.AddRoom(newRoom);
    }

    public string[] GetGasNames()
    {
        return _atmosphericGasses.Keys.ToArray();
    }

    void CopyGas(Room other)
    {
        foreach(string n in other._atmosphericGasses.Keys)
        {
            this._atmosphericGasses[n] = other._atmosphericGasses[n];
        }
    }

    public bool IsOutsideRoom()
    {
        return this == _world.GetOutsideRoom();
    }

    public void ChangeGas(string name, float amount)
    {
        if (IsOutsideRoom())
        {
            return;
        }

        if (_atmosphericGasses.ContainsKey(name))
        {
            _atmosphericGasses[name] += amount;
        }
        else
        {
            _atmosphericGasses[name] = amount;
        }

        if(_atmosphericGasses[name] < 0)
        {
            _atmosphericGasses[name] = 0;
        }
    }

    public float GetGasAmount(string name)
    {
        if (_atmosphericGasses.ContainsKey(name))
        {
            return _atmosphericGasses[name];
        }
        return 0;
    }

    public float GetGasPercentage(string name)
    {
        if (!_atmosphericGasses.ContainsKey(name))
        {
            return 0;
        }

        float total = 0;
        foreach (string n in _atmosphericGasses.Keys)
        {
            total += _atmosphericGasses[n];
        }

        return _atmosphericGasses[name] / total;
    }
}
