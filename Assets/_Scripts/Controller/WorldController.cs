using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour {

    static WorldController _instance;

    Dictionary<Tile, GameObject> _tileGameObjectMap;
    Dictionary<InstalledObject, GameObject> _installedObjectGameObjectMap;

    [SerializeField] Sprite _floorSprite;
    [SerializeField] Sprite _wallSprite;

    World _world;

    public static WorldController Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<WorldController>();
                if(_instance == null)
                {
                    var singleton = new GameObject();
                    _instance = singleton.AddComponent<WorldController>();
                    singleton.name = typeof(WorldController).ToString() + " (Singleton)";

                    DontDestroyOnLoad(singleton);
                }
            }
            return _instance;
        }
    }

    public World World
    {
        get
        {
            return _world;
        }
    }

    void Start()
    {
        _world = new World();

        _world.RegisterInstalledObjectCreated(OnInstalledObjectCreated);

        _tileGameObjectMap = new Dictionary<Tile, GameObject>();
        _installedObjectGameObjectMap = new Dictionary<InstalledObject, GameObject>();

        for (int x = 0; x < _world.Width; x++)
        {
            for (int y = 0; y < _world.Height; y++)
            {
                GameObject tile_go = new GameObject();
                Tile tile_data = _world.GetTileAt(x, y);

                _tileGameObjectMap.Add(tile_data, tile_go);

                tile_go.name = "Tile_" + x + "_" + y;
                tile_go.transform.position = new Vector3(tile_data.X, tile_data.Y, 0);
                tile_go.transform.SetParent(this.transform, true);

                tile_go.AddComponent<SpriteRenderer>();

                //tile_data.RegisterTileTypeChangedCallback( (tile) => { OnTileTypeChanged(tile, tile_go); });
                tile_data.RegisterTileTypeChangedCallback(OnTileTypeChanged);
            }
        }

        _world.RandomizeTiles();
    }

    //Old version, used by lambda
    /*void OnTileTypeChanged(Tile tile_data, GameObject tile_go)
    {
        if (tile_data.Type == Tile.TileType.Floor)
        {
            tile_go.GetComponent<SpriteRenderer>().sprite = floorSprite;
        }
        else
        {
            tile_go.GetComponent<SpriteRenderer>().sprite = null;
        }
    }*/

    void OnTileTypeChanged(Tile tile_data)
    {
        GameObject tile_go = _tileGameObjectMap[tile_data];
        if (!_tileGameObjectMap.TryGetValue(tile_data, out tile_go))
        {
            Debug.LogError("_tileGameObjectMap doesn't contain the tile_data!");
            return;
        }    

        if (tile_data.Type == TileType.Floor)
        {
            tile_go.GetComponent<SpriteRenderer>().sprite = _floorSprite;
        }
        else
        {
            tile_go.GetComponent<SpriteRenderer>().sprite = null;
        }
    }

    public Tile GetTileAtWorldCoordinate(Vector3 coordinate)
    {
        int x = Mathf.FloorToInt(coordinate.x);
        int y = Mathf.FloorToInt(coordinate.y);

        return Instance.World.GetTileAt(x, y);
    }

    public void OnInstalledObjectCreated(InstalledObject obj)
    {
        GameObject instObj = new GameObject();

        _installedObjectGameObjectMap.Add(obj, instObj);

        instObj.name = obj.ObjectType + "_" + obj.Tile.X + "_" + obj.Tile.Y;
        instObj.transform.position = new Vector3(obj.Tile.X, obj.Tile.Y, 0);
        instObj.transform.SetParent(this.transform, true);

        instObj.AddComponent<SpriteRenderer>().sprite = _wallSprite;
        instObj.GetComponent<SpriteRenderer>().sortingLayerName = "InstalledObjects";

        obj.RegisterOnInstalledObjectChangedCallback(OnInstalledObjectChanged);
    }

    void OnInstalledObjectChanged(InstalledObject obj)
    {
        Debug.Log("OnInstalledObjectChanged -- NOT IMPLEMENTED!");
    }
}
