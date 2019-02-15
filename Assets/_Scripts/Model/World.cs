using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;

public class World : IXmlSerializable{

    Tile[,] _tiles;

    List<Character> _characters;
    List<InstalledObject> _installedObjects;
    List<Room> _rooms;

    LooseObjectManager _looseObjectManager;

    Path_TileGraph _tileGraph;

    Dictionary<string, InstalledObject> _installedObjectPrototypes;
    Dictionary<string, Job> _installedObjectJobPrototypes;
    
    Action<InstalledObject> _cbInstalledObjectCreated;
    Action<Character> _cbCharacterCreated;
    Action<Tile> _cbTileChanged;
    Action<LooseObject> _cbLooseObjectCreated;

    JobQueue _jobQueue;

    int _width;
    int _height;

    #region Properties
    public int Width
    {
        get
        {
            return _width;
        }
    }

    public int Height
    {
        get
        {
            return _height;
        }
    }

    public JobQueue JobQueue
    {
        get
        {
            return _jobQueue;
        }

        set
        {
            _jobQueue = value;
        }
    }

    public Path_TileGraph TileGraph
    {
        get
        {
            return _tileGraph;
        }

        set
        {
            _tileGraph = value;
        }
    }

    public List<InstalledObject> InstalledObjects
    {
        get
        {
            return _installedObjects;
        }
    }

    public List<Character> Characters
    {
        get
        {
            return _characters;
        }
    }

    public List<Room> Rooms
    {
        get
        {
            return _rooms;
        }

        set
        {
            _rooms = value;
        }
    }

    public LooseObjectManager LooseObjectManager
    {
        get
        {
            return _looseObjectManager;
        }

        set
        {
            _looseObjectManager = value;
        }
    }

    public Dictionary<string, Job> InstalledObjectJobPrototypes
    {
        get
        {
            return _installedObjectJobPrototypes;
        }

        set
        {
            _installedObjectJobPrototypes = value;
        }
    }

    public Action<LooseObject> CbLooseObjectCreated
    {
        get
        {
            return _cbLooseObjectCreated;
        }

        set
        {
            _cbLooseObjectCreated = value;
        }
    }

    public Dictionary<string, InstalledObject> InstalledObjectPrototypes
    {
        get
        {
            return _installedObjectPrototypes;
        }

        set
        {
            _installedObjectPrototypes = value;
        }
    }
    #endregion

    public World(int width, int height)
    {
        SetupNewWorld(width, height);

        CreateCharacter(GetTileAt(width / 2, height / 2));
    }

    void SetupNewWorld(int width, int height)
    {
        this._width = width;
        this._height = height;

        _tiles = new Tile[width, height];

        _jobQueue = new JobQueue();

        _rooms = new List<Room>();
        _rooms.Add(new Room(this));

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                _tiles[x, y] = new Tile(this, x, y);
                _tiles[x, y].RegisterTileTypeChangedCallback(OnTileChanged);
                _tiles[x, y].Room = GetOutsideRoom();
            }
        }

        Debug.Log("World created with " + (width * height) + " tiles.");

        InitializeInstalledObjectPrototypesDictionary();

        _characters = new List<Character>();
        _installedObjects = new List<InstalledObject>();
        _looseObjectManager = new LooseObjectManager();
    }

    public void Update(float deltaTime)
    {
        foreach(Character c in _characters)
        {
            c.Update(deltaTime);
        }

        foreach(InstalledObject obj in _installedObjects)
        {
            obj.Update(deltaTime);
        }
    }

    public Character CreateCharacter(Tile tile)
    {
        Character newCharacter = new Character(tile);
        _characters.Add(newCharacter);
        if (_cbCharacterCreated != null)
        {
            _cbCharacterCreated(newCharacter);
        }

        return newCharacter;
    }

    void InitializeInstalledObjectPrototypesDictionary()
    {
        _installedObjectPrototypes = new Dictionary<string, InstalledObject>();
        _installedObjectJobPrototypes = new Dictionary<string, Job>();

        InstalledObject wallPrototype =  new InstalledObject("Wall", 0, 1, 1, true, true);
        _installedObjectPrototypes.Add("Wall", wallPrototype);
        _installedObjectJobPrototypes.Add("Wall", new Job(null, "Wall", InstalledObjectAction.OnInstalledObjectJobCompleted, 1f, new LooseObject[] { new LooseObject("SteelPlate_", 5, 0) }));

        InstalledObject doorPrototype = new InstalledObject("Door", 1, 1, 1, false, true);
        _installedObjectPrototypes.Add("Door", doorPrototype);

        _installedObjectPrototypes["Door"].SetParameter("openness", 0);
        _installedObjectPrototypes["Door"].SetParameter("openingState", 0);
        _installedObjectPrototypes["Door"].RegisterUpdateAction(InstalledObjectAction.Door_UpdateAction);
        _installedObjectPrototypes["Door"]._checkEnterableState = InstalledObjectAction.Door_EnterableState;

        InstalledObject stockPilePrototype = new InstalledObject("Stockpile", 1, 1, 1, true, false);
        _installedObjectPrototypes.Add("Stockpile", stockPilePrototype);
        _installedObjectPrototypes["Stockpile"].RegisterUpdateAction(InstalledObjectAction.Stockpile_UpdateAction);
        _installedObjectPrototypes["Stockpile"].Tint = new Color(1f, 0f, 0f, 1f);
        _installedObjectJobPrototypes.Add("Stockpile", new Job(null, "Stockpile", InstalledObjectAction.OnInstalledObjectJobCompleted, -1f, null));

        InstalledObject o2GeneratorPrototype = new InstalledObject("O2Generator", 10, 2, 2, false, false);
        _installedObjectPrototypes.Add("O2Generator", o2GeneratorPrototype);
        _installedObjectPrototypes["O2Generator"].RegisterUpdateAction(InstalledObjectAction.OxygenGenerator_UpdateAction);
    }

    public void RandomizeTiles()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                if(UnityEngine.Random.Range(0, 2) == 0)
                {
                    _tiles[x, y].Type = TileType.Empty;
                }
                else
                {
                    _tiles[x, y].Type = TileType.Floor;
                }
               
            }
        }
    }

    public Tile GetTileAt(int x, int y)
    {
        if(x >= _width || x < 0 || y >= _height || y < 0)
        {
            //Debug.LogError("Tile (" + x + ", " + y + ") is out of range.");
            return null;
        }
        return _tiles[x, y];
    }

    public InstalledObject PlaceInstalledObject(string objectType, Tile tile)
    {
        InstalledObject instObj;
        if (!_installedObjectPrototypes.TryGetValue(objectType, out instObj))
        {
            Debug.LogError("_installedObjectPrototypes doesn't contain the objectType!");
            return null;
        }

        InstalledObject obj = InstalledObject.PlaceInstance(instObj, tile);
        if(obj == null)
        {
            //Failed to place object. Most likely there was already something there.
            return null;
        }

        _installedObjects.Add(obj);

        if (obj.RoomEnclosure)
        {
            Room.DoRoomFloodFill(obj);
        }

        if(_cbInstalledObjectCreated != null)
        {
            _cbInstalledObjectCreated(obj);
            if (obj.MovementCost != 1)
            {
                InvalidateTileGraph();
            }
        }

        return obj;
    }

    void OnTileChanged(Tile t)
    {
        InvalidateTileGraph();
        if (_cbTileChanged == null)
        {
            return;
        }
        _cbTileChanged(t);
    }

    public void InvalidateTileGraph()
    {
        _tileGraph = null;
    }

    public bool IsInstalledObjectPlacementValid(string installedObjectType, Tile tile)
    {
        return _installedObjectPrototypes[installedObjectType].IsValidPosition(tile);
    }

    public InstalledObject GetInstalledObjectPrototype(string objectType)
    {
        InstalledObject instObj;
        if (!_installedObjectPrototypes.TryGetValue(objectType, out instObj))
        {
            Debug.LogError("_installedObjectPrototypes doesn't contain the objectType!");
            return null;
        }
        return _installedObjectPrototypes[objectType];
    }

    public void SetupPathfindingExample()
    {
        int l = Width / 2 - 5;
        int b = Height / 2 - 5;

        for (int x = l - 5; x < l + 15; x++)
        {
            for (int y = b - 5; y < b + 15; y++)
            {
                _tiles[x, y].Type = TileType.Floor;

                if(x == l || x == (l + 9) || y == b || y == (b + 9))
                {
                    if(x != (l + 9) && y != (b + 4))
                    {
                        PlaceInstalledObject("Wall", _tiles[x, y]);
                    }
                }
            }
        }
    }

    public void OnLooseObjectCreated(LooseObject obj)
    {
        if(_cbLooseObjectCreated != null)
        {
            _cbLooseObjectCreated(obj);
        }
    }

    #region Room Related Methods
    public Room GetOutsideRoom()
    {
        return _rooms[0];
    }

    public void DeleteRoom(Room room)
    {
        if (room == GetOutsideRoom())
        {
            Debug.LogError("Tried to delete the outside room!");
            return;
        }
        _rooms.Remove(room);
        room.UnassingnAllTiles();
    }

    public void AddRoom(Room room)
    {
        _rooms.Add(room);
    }
    #endregion

    #region Callbacks
    public void RegisterInstalledObjectCreated(Action<InstalledObject> callback)
    {
        _cbInstalledObjectCreated += callback;
    }

    public void UnregisterInstalledObjectCreated(Action<InstalledObject> callback)
    {
        _cbInstalledObjectCreated -= callback;
    }

    public void RegisterTileChanged(Action<Tile> callback)
    {
        _cbTileChanged += callback;
    }

    public void UnregisterTileChanged(Action<Tile> callback)
    {
        _cbTileChanged -= callback;
    }

    public void RegisterCharacterCreated(Action<Character> callback)
    {
        _cbCharacterCreated += callback;
    }

    public void UnregisterCharacterCreated(Action<Character> callback)
    {
        _cbCharacterCreated -= callback;
    }

    public void RegisterLooseObjectCreated(Action<LooseObject> callback)
    {
        _cbLooseObjectCreated += callback;
    }

    public void UnregisterLooseObjectCreated(Action<LooseObject> callback)
    {
        _cbLooseObjectCreated -= callback;
    }
    #endregion

    #region Saving & Loading

    public World()
    {

    }

    public XmlSchema GetSchema()
    {
        return null;
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteAttributeString("Width", Width.ToString());
        writer.WriteAttributeString("Height", Height.ToString());

        //TILES
        writer.WriteStartElement("Tiles");
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (_tiles[x, y].Type != TileType.Empty)
                {
                    writer.WriteStartElement("Tile");
                    _tiles[x, y].WriteXml(writer);
                    writer.WriteEndElement();
                }
            }
        }
        writer.WriteEndElement();

        //INSTALLEDOBJECTS
        writer.WriteStartElement("InstalledObjects");
        foreach(InstalledObject obj in _installedObjects)
        {
            writer.WriteStartElement("InstalledObject");
            obj.WriteXml(writer);
            writer.WriteEndElement();
        }
        writer.WriteEndElement();

        //CHARACTERS
        writer.WriteStartElement("Characters");
        foreach (Character c in _characters)
        {
            writer.WriteStartElement("Character");
            c.WriteXml(writer);
            writer.WriteEndElement();
        }
        writer.WriteEndElement();
    }

    public void ReadXml(XmlReader reader)
    {
        int w = int.Parse(reader.GetAttribute("Width"));
        int h = int.Parse(reader.GetAttribute("Height"));

        SetupNewWorld(w, h);

        while (reader.Read())
        {
            switch (reader.Name)
            {
                case "Tiles":
                    ReadXml_Tiles(reader);
                    break;
                case "InstalledObjects":
                    ReadXml_InstalledObjects(reader);
                    break;
                case "Characters":
                    ReadXml_Characters(reader);
                    break;

                default:
                    break;
            }
        }

        //TEST ONLY!
        //Create an LooseObject Item
        LooseObject looseObject = new LooseObject("SteelPlate_", 50, 10);
        Tile tile = GetTileAt(Width / 2 + 4, Height / 2);
        _looseObjectManager.PlaceLooseObject(tile, looseObject);
        if(_cbLooseObjectCreated != null)
        {
            _cbLooseObjectCreated(tile.LooseObject);
        }

        looseObject = new LooseObject("SteelPlate_", 50, 20);
        tile = GetTileAt(Width / 2 + 4, Height / 2 + 4);
        _looseObjectManager.PlaceLooseObject(tile, looseObject);
        if (_cbLooseObjectCreated != null)
        {
            _cbLooseObjectCreated(tile.LooseObject);
        }
    }

    void ReadXml_Characters(XmlReader reader)
    {
        if (reader.ReadToDescendant("Character"))
        {
            do
            {
                int x = int.Parse(reader.GetAttribute("X"));
                int y = int.Parse(reader.GetAttribute("Y"));

                Character c = CreateCharacter(_tiles[x, y]);
                c.ReadXml(reader);
            } while (reader.ReadToNextSibling("Character"));
        }
}

    void ReadXml_Tiles(XmlReader reader)
    {
        if (reader.ReadToDescendant("Tile"))
        {
            do
            {
                int x = int.Parse(reader.GetAttribute("X"));
                int y = int.Parse(reader.GetAttribute("Y"));
                _tiles[x, y].ReadXml(reader);
            } while (reader.ReadToNextSibling("Tile"));
        }
    }

    void ReadXml_InstalledObjects(XmlReader reader)
    {

        if(reader.ReadToDescendant("InstalledObject"))
        {
            do
            {
                int x = int.Parse(reader.GetAttribute("X"));
                int y = int.Parse(reader.GetAttribute("Y"));

                InstalledObject obj = PlaceInstalledObject(reader.GetAttribute("ObjectType"), _tiles[x, y]);
                obj.ReadXml(reader);
            } while (reader.ReadToNextSibling("InstalledObject"));
        }
    }

    #endregion

}
