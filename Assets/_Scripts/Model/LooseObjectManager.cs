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

    void CleanupLooseObject(LooseObject obj)
    {
        if (obj.StackSize == 0)
        {
            if (_stringLooseObjectMap.ContainsKey(obj.ObjectType))
            {
                _stringLooseObjectMap[obj.ObjectType].Remove(obj);
                if (obj.Tile != null)
                {
                    obj.Tile.LooseObject = null;
                    obj.Tile = null;
                }
                if (obj.Character != null)
                {
                    obj.Character.LooseObject = null;
                    obj.Character = null;
                }
            }
        }
    }

    public bool PlaceLooseObject(Tile tile, LooseObject obj)
    {
        bool tileWasEmpty = tile.LooseObject == null;

        if (!tile.AssignLooseObject(obj)) {
            return false;
        }

        CleanupLooseObject(obj);

        if (tileWasEmpty)
        {
            if (!_stringLooseObjectMap.ContainsKey(tile.LooseObject.ObjectType))
            {
                _stringLooseObjectMap[tile.LooseObject.ObjectType] = new List<LooseObject>(); 
            }
            _stringLooseObjectMap[tile.LooseObject.ObjectType].Add(tile.LooseObject);

            tile.World.OnLooseObjectCreated(tile.LooseObject);
        }
        return true;
    }

    public bool PlaceLooseObject(Job job, LooseObject obj)
    {
        if (!job.LooseObjectRequeriments.ContainsKey(obj.ObjectType))
        {
            Debug.LogError("Trying to add looseObject to a job that doesn't want it!");
            return false;
        }

        job.LooseObjectRequeriments[obj.ObjectType].StackSize += obj.StackSize;

        if(job.LooseObjectRequeriments[obj.ObjectType].MaxStackSize < job.LooseObjectRequeriments[obj.ObjectType].StackSize)
        {
            obj.StackSize = job.LooseObjectRequeriments[obj.ObjectType].StackSize - job.LooseObjectRequeriments[obj.ObjectType].MaxStackSize;
            job.LooseObjectRequeriments[obj.ObjectType].StackSize = job.LooseObjectRequeriments[obj.ObjectType].MaxStackSize;
        }
        else
        {
            obj.StackSize = 0;
        }

        CleanupLooseObject(obj);

        return true;
    }

    public bool PlaceLooseObject(Character character, LooseObject obj, int amount = -1)
    {
        if(amount < 0)
        {
            amount = obj.StackSize;
        }
        else
        {
            amount = Mathf.Min(amount, obj.StackSize);
        }

        if(character.LooseObject == null)
        {
            character.LooseObject = obj.Clone();
            character.LooseObject.StackSize = 0;
            _stringLooseObjectMap[character.LooseObject.ObjectType].Add(character.LooseObject);
        }
        else if (character.LooseObject.ObjectType != obj.ObjectType)
        {
            Debug.LogError("Character is trying to pick up a mismatched looseObject type!");
            return false;
        }

        character.LooseObject.StackSize += amount;

        if (character.LooseObject.MaxStackSize < character.LooseObject.StackSize)
        {
            obj.StackSize = character.LooseObject.StackSize - character.LooseObject.MaxStackSize;
            character.LooseObject.StackSize = character.LooseObject.MaxStackSize;
        }
        else
        {
            obj.StackSize -= amount;
        }

        CleanupLooseObject(obj);

        return true;
    }


    public LooseObject GetClosestLooseObjectOfType(string objectType, Tile tile, int requiredAmount, bool canFetchFromStockpile)
    {
        if (!_stringLooseObjectMap.ContainsKey(objectType))
        {
            Debug.LogError("GetClosestLooseObjectOfType -- No items of required type!");
            return null;
        }

        foreach(LooseObject obj in _stringLooseObjectMap[objectType])
        {
            if(obj.Tile != null 
                && (canFetchFromStockpile || obj.Tile.InstalledObject == null || !obj.Tile.InstalledObject.IsStockpileJob()))
            {
                return obj;
            }
        }
        return null;
    }

}
