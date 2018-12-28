using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstalledObjectSpriteController : MonoBehaviour {

    Dictionary<InstalledObject, GameObject> _installedObjectGameObjectMap;
    Dictionary<string, Sprite> _stringSpritesMap;

    World _world;

    void Start()
    {
        LoadSprites();

        _world = WorldController.Instance.World;

        _installedObjectGameObjectMap = new Dictionary<InstalledObject, GameObject>();

        _world.RegisterInstalledObjectCreated(OnInstalledObjectCreated);

        foreach(InstalledObject obj in _world.InstalledObjects){
            OnInstalledObjectCreated(obj);
        }
    }

    void LoadSprites()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("_Sprites/NewSprites/Walls/");
        _stringSpritesMap = new Dictionary<string, Sprite>();

        foreach (Sprite s in sprites)
        {
            _stringSpritesMap[s.name] = s;
        }
    }

    public void OnInstalledObjectCreated(InstalledObject obj)
    {
        GameObject instObj = new GameObject();

        _installedObjectGameObjectMap.Add(obj, instObj);

        instObj.name = obj.ObjectType + "_" + obj.Tile.X + "_" + obj.Tile.Y;
        instObj.transform.position = new Vector3(obj.Tile.X, obj.Tile.Y, 0);
        instObj.transform.SetParent(this.transform, true);

        instObj.AddComponent<SpriteRenderer>().sprite = GetSpriteForInstalledObject(obj);
        instObj.GetComponent<SpriteRenderer>().sortingLayerName = "InstalledObjects";

        obj.RegisterOnInstalledObjectChangedCallback(OnInstalledObjectChanged);
    }

    public Sprite GetSpriteForInstalledObject(InstalledObject obj)
    {
        if (!obj.LinksToNeighbor)
        {
            return _stringSpritesMap[obj.ObjectType];
        }

        string spriteName = obj.ObjectType + "_";

        int x = obj.Tile.X;
        int y = obj.Tile.Y;

        Tile t;
        t = _world.GetTileAt(x, y + 1);
        if(t != null && t.InstalledObject != null && t.InstalledObject.ObjectType.Equals(obj.ObjectType)) 
        {
            spriteName += "N";
        }

        t = _world.GetTileAt(x + 1, y);
        if (t != null && t.InstalledObject != null && t.InstalledObject.ObjectType.Equals(obj.ObjectType))
        {
            spriteName += "E";
        }

        t = _world.GetTileAt(x, y - 1);
        if (t != null && t.InstalledObject != null && t.InstalledObject.ObjectType.Equals(obj.ObjectType))
        {
            spriteName += "S";
        }

        t = _world.GetTileAt(x - 1, y);
        if (t != null && t.InstalledObject != null && t.InstalledObject.ObjectType.Equals(obj.ObjectType))
        {
            spriteName += "W";
        }

        Sprite sprite;
        if (!_stringSpritesMap.TryGetValue(spriteName, out sprite))
        {
            Debug.LogError("_stringSpritesMap doesn't contain the Sprite!");
            return null;
        }

        return _stringSpritesMap[spriteName];
    }

    public Sprite GetSpriteForInstalledObject(string objectType)
    {
        Sprite sprite;
        if (!_stringSpritesMap.TryGetValue(objectType, out sprite))
        {
            //Walls
            if (!_stringSpritesMap.TryGetValue(objectType + "_", out sprite))
            {
                Debug.LogError("_stringSpritesMap doesn't contain the Sprite for: " + objectType);
                return null;
            }
            else
            {
                return _stringSpritesMap[objectType + "_"];
            }
        }
        else
        {
            return _stringSpritesMap[objectType];
        }

        /*
        if (_stringSpritesMap.ContainsKey(objectType))
        {
            return _stringSpritesMap[objectType];
        }

        if (_stringSpritesMap.ContainsKey(objectType + "_"))
        {
            return _stringSpritesMap[objectType + "_"];
        }

        Debug.LogError("_stringSpritesMap doesn't contain the Sprite for: " + objectType);
        return null;
        */
    }

    void OnInstalledObjectChanged(InstalledObject obj)
    {
        GameObject inst_go;
        if (!_installedObjectGameObjectMap.TryGetValue(obj, out inst_go))
        {
            Debug.LogError("_installedObjectGameObjectMap doesn't contain the installed object!");
            return;
        }
        inst_go.GetComponent<SpriteRenderer>().sprite = GetSpriteForInstalledObject(obj);
    }
}
