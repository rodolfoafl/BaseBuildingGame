using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World {

    Tile[,] _tiles;
    int _width;
    int _height;

    public World(int width = 100, int height = 100)
    {
        this._width = width;
        this._height = height;

        _tiles = new Tile[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                _tiles[x, y] = new Tile(this, x, y);
            }
        }

        Debug.Log("World created with " + (width * height) + " tiles.");
    }

    public Tile GetTileAt(int x, int y)
    {
        if(x > _width || x < 0 || y > _height || y < 0)
        {
            Debug.LogError("Tile (" + x + ", " + y + ") is out of range.");
            return null;
        }
        return _tiles[x, y];
    }

}
