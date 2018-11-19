using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour {

    static WorldController _instance;

    [SerializeField] Sprite floorSprite;

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

        for (int x = 0; x < _world.Width; x++)
        {
            for (int y = 0; y < _world.Height; y++)
            {
                GameObject tile_go = new GameObject();
                Tile tile_data = _world.GetTileAt(x, y);
                tile_go.name = "Tile_" + x + "_" + y;
                tile_go.transform.position = new Vector3(tile_data.X, tile_data.Y, 0);
                tile_go.transform.SetParent(this.transform, true);

                tile_go.AddComponent<SpriteRenderer>();

                tile_data.RegisterTileTypeChangedCallback( (tile) => { OnTileTypeChanged(tile, tile_go); });
            }
        }

        _world.RandomizeTiles();
    }

    void OnTileTypeChanged(Tile tile_data, GameObject tile_go)
    {
        if(tile_data.Type == Tile.TileType.Floor)
        {
            tile_go.GetComponent<SpriteRenderer>().sprite = floorSprite;
        }
        else
        {
            tile_go.GetComponent<SpriteRenderer>().sprite = null;
        }
    }
}
