using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LooseObjectManager {

    Dictionary<string, List<LooseObject>> _stringLooseObjectMap;

    #region Properties
    public Dictionary<string, List<LooseObject>> StringLooseObjectMap
    {
        get
        {
            return _stringLooseObjectMap;
        }

        set
        {
            _stringLooseObjectMap = value;
        }
    }
    #endregion

    public LooseObjectManager()
    {
        _stringLooseObjectMap = new Dictionary<string, List<LooseObject>>();
    }

    public bool PlaceLooseObject(Tile tile, LooseObject obj)
    {
        bool tileWasEmpty = tile.LooseObject == null;

        if (!tile.AssignLooseObject(obj)) {
            return false;
        }

        if(obj.StackSize == 0)
        {
            if (_stringLooseObjectMap.ContainsKey(tile.LooseObject.ObjectType))
            {
                _stringLooseObjectMap[obj.ObjectType].Remove(obj);
            }                
        }

        if (tileWasEmpty)
        {
            if (!_stringLooseObjectMap.ContainsKey(tile.LooseObject.ObjectType))
            {
                _stringLooseObjectMap[tile.LooseObject.ObjectType] = new List<LooseObject>(); 
            }
            _stringLooseObjectMap[tile.LooseObject.ObjectType].Add(tile.LooseObject);
        }

        return true;
    }


}
