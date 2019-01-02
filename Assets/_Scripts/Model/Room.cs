using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room {

    float _atmosO2 = 0f;
    float _atmosN = 0f;
    float _atmosC02 = 0f;

    List<Tile> _tiles;

    #region Properties
    public float AtmosO2
    {
        get
        {
            return _atmosO2;
        }

        set
        {
            _atmosO2 = value;
        }
    }

    public float AtmosN
    {
        get
        {
            return _atmosN;
        }

        set
        {
            _atmosN = value;
        }
    }

    public float AtmosC02
    {
        get
        {
            return _atmosC02;
        }

        set
        {
            _atmosC02 = value;
        }
    }

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

    public Room()
    {
        _tiles = new List<Tile>();
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

        if (oldRoom != world.GetOutsideRoom())
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

        Room newRoom = new Room();
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

        newRoom.AtmosC02 = oldRoom.AtmosC02;
        newRoom.AtmosN = oldRoom.AtmosN;
        newRoom.AtmosO2 = oldRoom.AtmosO2;

        tile.World.AddRoom(newRoom);
    }
}
