using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WorldController : MonoBehaviour {

    static WorldController _instance;
    public static WorldController Instance
    {
        get
        {
            return _instance;
        }
        protected set
        {
            _instance = value;
        }
    }

    [SerializeField] Sprite floorSprite;

    World world;
    public World World
    {
        get
        {
            return world;
        }

        set
        {
            world = value;
        }
    }

    // Use this for initialization
    void Start () {
        if(_instance != null)
        {
            Debug.LogError("There should never be more than one WorldController");
        }
        _instance = this;

        world = new World();
        //world.RandomizeTiles();

        for (int x = 0; x < world.Width; x++)
        {
            for (int y = 0; y < world.Height; y++)
            {
                Tile tile_data = world.GetTileAt(x, y);

                GameObject tile_go = new GameObject();
                tile_go.name = "Tile_" + x + "_" + y;
                tile_go.transform.position = new Vector3(tile_data.X, tile_data.Y, 0);

                tile_go.transform.SetParent(transform, true);

                tile_go.AddComponent<SpriteRenderer>();

                tile_data.RegisterTileTypeChangedCallback((tile) => { OnTileTypeChanged(tile, tile_go); });
            }
        }

        world.RandomizeTiles();
	}

    //float randomizeTileTimer = 2f;

    void Update()
    {
        /*randomizeTileTimer -= Time.deltaTime;

        if(randomizeTileTimer < 0)
        {
            world.RandomizeTiles();
            randomizeTileTimer = 2f;
        }*/
    }

    void OnTileTypeChanged(Tile tile_data, GameObject tile_go)
    {

        if (tile_data.Type == Tile.TileType.Floor)
        {
            tile_go.GetComponent<SpriteRenderer>().sprite = floorSprite;
        }
        else if(tile_data.Type == Tile.TileType.Empty)
        {
            tile_go.GetComponent<SpriteRenderer>().sprite = null;
        }
        else
        {
            Debug.LogError("OnTileTypeChanged - Unrecognized tile type!");
        }
    }

    public Tile GetTileAtWorldCoordinate(Vector3 coord)
    {
        int x = Mathf.FloorToInt(coord.x);
        int y = Mathf.FloorToInt(coord.y);

        return WorldController.Instance.World.GetTileAt(x, y);
    }

}
