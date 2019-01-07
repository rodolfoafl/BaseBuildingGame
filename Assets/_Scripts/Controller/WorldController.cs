using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldController : MonoBehaviour {
    static bool _loadWorld = false;

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
        if (_loadWorld)
        {
            _loadWorld = false;
            LoadWorldFromSave();
        }
        else
        {
            CreateEmptyWorld();
        }
    }

    void Update()
    {
        World.Update(Time.deltaTime);
    }

    public void CreateNewWorld()
    {
        Debug.Log("CreateNewWorld");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SaveWorld()
    {
        Debug.Log("SaveWorld");

        XmlSerializer serializer = new XmlSerializer(typeof(World));
        TextWriter writer = new StringWriter();
        serializer.Serialize(writer, World);
        writer.Close();

        Debug.Log(writer.ToString());

        PlayerPrefs.SetString("SaveGame00", writer.ToString());
    }

    public void LoadWorld()
    {
        Debug.Log("LoadWorld");

        _loadWorld = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void LoadWorldFromSave()
    {
        Debug.Log("LoadWorldFromSave");

        XmlSerializer deserializer = new XmlSerializer(typeof(World));
        TextReader reader = new StringReader(PlayerPrefs.GetString("SaveGame00"));
        _world = (World)deserializer.Deserialize(reader);
        reader.Close();

        Camera.main.transform.position = new Vector3(_world.Width / 2, _world.Height / 2, Camera.main.transform.position.z);
    }

    void CreateEmptyWorld()
    {
        _world = new World(100, 100);

        Camera.main.transform.position = new Vector3(_world.Width / 2, _world.Height / 2, Camera.main.transform.position.z);
    }

    public Tile GetTileAtWorldCoordinate(Vector3 coordinate)
    {
        int x = Mathf.RoundToInt(coordinate.x);
        int y = Mathf.RoundToInt(coordinate.y);

        return Instance.World.GetTileAt(x, y);
    }
}
