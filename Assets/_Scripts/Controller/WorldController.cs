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
    [SerializeField] Sprite wallSprite;
    Dictionary<Tile, GameObject> tileGameObjectMap;
    Dictionary<InstalledObject, GameObject> installedObjectGameObjectMap;

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
        world.RegisterInstalledObjectCreated(OnInstalledObjectCreated);

        tileGameObjectMap = new Dictionary<Tile, GameObject>();
        installedObjectGameObjectMap = new Dictionary<InstalledObject, GameObject>();
        //world.RandomizeTiles();

        for (int x = 0; x < world.Width; x++)
        {
            for (int y = 0; y < world.Height; y++)
            {
                Tile tile_data = world.GetTileAt(x, y);

                GameObject tile_go = new GameObject();

                tileGameObjectMap.Add(tile_data, tile_go);
                tile_go.name = "Tile_" + x + "_" + y;
                tile_go.transform.position = new Vector3(tile_data.X, tile_data.Y, 0);

                tile_go.transform.SetParent(transform, true);

                tile_go.AddComponent<SpriteRenderer>();

                //tile_data.RegisterTileTypeChangedCallback((tile) => { OnTileTypeChanged(tile, tile_go); });
                tile_data.RegisterTileTypeChangedCallback(OnTileTypeChanged);
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

    /*void OnTileTypeChanged(Tile tile_data, GameObject tile_go)
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
    }*/

    void OnTileTypeChanged(Tile tile_data)
    {
        GameObject tile_go = tileGameObjectMap[tile_data];
        if (!tileGameObjectMap.TryGetValue(tile_data, out tile_go))
        {
            Debug.LogError("tileGameObjectMap doesn't contain the tile_data");
            return;
        }

        if (tile_data.Type == Tile.TileType.Floor)
        {
            tile_go.GetComponent<SpriteRenderer>().sprite = floorSprite;
        }
        else if (tile_data.Type == Tile.TileType.Empty)
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

    public void OnInstalledObjectCreated(InstalledObject obj) 
    {
        //Debug.Log("OnInstalledObjectCreated");
        GameObject obj_go = new GameObject();

        installedObjectGameObjectMap.Add(obj, obj_go);
        obj_go.name = obj.ObjectType + "_" + obj.Tile.X + "_" + obj.Tile.Y;
        obj_go.transform.position = new Vector3(obj.Tile.X, obj.Tile.Y, 0);

        obj_go.transform.SetParent(transform, true);

        obj_go.AddComponent<SpriteRenderer>().sprite = wallSprite;

        //tile_data.RegisterTileTypeChangedCallback((tile) => { OnTileTypeChanged(tile, tile_go); });
        obj.RegisterOnChangedCallback(OnInstalledObjectChanged);
    }

    void OnInstalledObjectChanged(InstalledObject obj)
    {
        Debug.LogError("Not Implemented");
    }

}
