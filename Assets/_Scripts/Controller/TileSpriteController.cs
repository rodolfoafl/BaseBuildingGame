using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSpriteController : MonoBehaviour {

    Dictionary<Tile, GameObject> _tileGameObjectMap;

    World _world;

    [SerializeField] Sprite _floorSprite;
    [SerializeField] Sprite _emptySprite;

    void Start()
    {
        _world = WorldController.Instance.World;

        _tileGameObjectMap = new Dictionary<Tile, GameObject>();

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

                tile_go.AddComponent<SpriteRenderer>().sprite = _emptySprite;
                OnTileChanged(tile_data);
                //tile_data.RegisterTileTypeChangedCallback( (tile) => { OnTileTypeChanged(tile, tile_go); });
                //tile_data.RegisterTileTypeChangedCallback(OnTileTypeChanged);
            }
        }

        _world.RegisterTileChanged(OnTileChanged);
    }

    void OnTileChanged(Tile tile_data)
    {
        GameObject tile_go;
        if (!_tileGameObjectMap.TryGetValue(tile_data, out tile_go))
        {
            Debug.LogError("_tileGameObjectMap doesn't contain the tile_data!");
            return;
        }    

        if (tile_data.Type == TileType.Floor)
        {
            tile_go.GetComponent<SpriteRenderer>().sprite = _floorSprite;
        }
        else if (tile_data.Type == TileType.Empty)
        {
            tile_go.GetComponent<SpriteRenderer>().sprite = _emptySprite;
        }
        else
        {
            Debug.LogError("OnTileTypeChanged - Unrecongized tile type!");
            tile_go.GetComponent<SpriteRenderer>().sprite = null;
        }
    }
}
