using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour {

    static WorldController _instance;

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

    void Awake()
    {
        _world = new World();

        Camera.main.transform.position = new Vector3(_world.Width / 2, _world.Height / 2, Camera.main.transform.position.z);
    }

    public Tile GetTileAtWorldCoordinate(Vector3 coordinate)
    {
        int x = Mathf.FloorToInt(coordinate.x);
        int y = Mathf.FloorToInt(coordinate.y);

        return Instance.World.GetTileAt(x, y);
    }
}
