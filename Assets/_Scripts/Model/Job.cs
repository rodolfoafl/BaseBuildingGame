using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Job {

    Tile _tile;

    float _jobTime;

    string _jobObjectType;

    bool _acceptsAnyLooseObjectItem = false;
    bool _canFetchFromStockpile = true;

    Action<Job> _cbJobCompleted;
    Action<Job> _cbJobCancelled;
    Action<Job> _cbJobProgressed;

    Dictionary<string, LooseObject> _looseObjectRequeriments;

    InstalledObject _installedObjectPrototype;

    #region Properties
    public Tile Tile
    {
        get
        {
            return _tile;
        }

        set
        {
            _tile = value;
        }
    }

    public string JobObjectType
    {
        get
        {
            return _jobObjectType;
        }

        set
        {
            _jobObjectType = value;
        }
    }

    public float JobTime
    {
        get
        {
            return _jobTime;
        }

        set
        {
            _jobTime = value;
        }
    }

    public Action<Job> CbJobCompleted
    {
        get
        {
            return _cbJobCompleted;
        }

        set
        {
            _cbJobCompleted = value;
        }
    }

    public Action<Job> CbJobCancelled
    {
        get
        {
            return _cbJobCancelled;
        }

        set
        {
            _cbJobCancelled = value;
        }
    }

    public Dictionary<string, LooseObject> LooseObjectRequeriments
    {
        get
        {
            return _looseObjectRequeriments;
        }

        set
        {
            _looseObjectRequeriments = value;
        }
    }

    public bool AcceptsAnyLooseObjectItem
    {
        get
        {
            return _acceptsAnyLooseObjectItem;
        }

        set
        {
            _acceptsAnyLooseObjectItem = value;
        }
    }

    public bool CanFetchFromStockpile
    {
        get
        {
            return _canFetchFromStockpile;
        }

        set
        {
            _canFetchFromStockpile = value;
        }
    }

    public InstalledObject InstalledObjectPrototype
    {
        get
        {
            return _installedObjectPrototype;
        }

        set
        {
            _installedObjectPrototype = value;
        }
    }
    #endregion

    public virtual Job Clone()
    {
        return new Job(this);
    }

    public Job(Tile tile, string jobObjectType, Action<Job> cbJobCompleted, float jobTime, LooseObject[] looseObjects)
    {
        this._tile = tile;
        this._jobObjectType = jobObjectType;
        this._cbJobCompleted += cbJobCompleted;
        this._jobTime = jobTime;

        this._looseObjectRequeriments = new Dictionary<string, LooseObject>();
        if(looseObjects != null)
        {
            foreach (LooseObject obj in looseObjects)
            {
                this._looseObjectRequeriments[obj.ObjectType] = obj.Clone();
            }
        }
    }

    protected Job(Job other)
    {
        this._tile = other.Tile;
        this._jobObjectType = other.JobObjectType;
        this._cbJobCompleted += other.CbJobCompleted;
        this._jobTime = other.JobTime;

        this._looseObjectRequeriments = new Dictionary<string, LooseObject>();
        if (other.LooseObjectRequeriments != null)
        {
            foreach (LooseObject obj in other.LooseObjectRequeriments.Values)
            {
                this._looseObjectRequeriments[obj.ObjectType] = obj.Clone();
            }
        }
    }

    public void WorkOnJob(float workTime)
    {
        if (!HasAllMaterials())
        {
            //Debug.LogError("Tried to do work on a job that doesn't have all the materials!");
            if(_cbJobProgressed != null)
            {
                _cbJobProgressed(this);
            }
            return;
        }

        _jobTime -= workTime;

        if(_cbJobProgressed != null)
        {
            _cbJobProgressed(this);
        }

        if (_jobTime <= 0)
        {
            if (_cbJobCompleted != null)
            {
                _cbJobCompleted(this);
            }
        }
    }

    public void CancelJob()
    {
        if(_cbJobCancelled != null)
        {
            _cbJobCancelled(this);
        }

        WorldController.Instance.World.JobQueue.Remove(this);
    }

    public bool HasAllMaterials()
    {
        foreach (LooseObject obj in _looseObjectRequeriments.Values)
        {
            if (obj.MaxStackSize > obj.StackSize)
            {
                return false;
            }
        }
        return true;
    }

    public int RequiredLooseObjectAmount(LooseObject obj)
    {
        if (_acceptsAnyLooseObjectItem)
        {
            return obj.MaxStackSize;
        }

        if (!_looseObjectRequeriments.ContainsKey(obj.ObjectType))
        {
            return 0;
        }

        if(_looseObjectRequeriments[obj.ObjectType].StackSize >= _looseObjectRequeriments[obj.ObjectType].MaxStackSize)
        {
            return 0;
        }

        return _looseObjectRequeriments[obj.ObjectType].MaxStackSize - _looseObjectRequeriments[obj.ObjectType].StackSize;
    }

    public LooseObject GetFirstRequiredLooseObject()
    {
        foreach (LooseObject obj in _looseObjectRequeriments.Values)
        {
            if (obj.MaxStackSize > obj.StackSize)
            {
                return obj;
            }
        }
        return null;
    }

    #region Callbacks
    public void RegisterJobCompletedCallback(Action<Job> callback)
    {
        _cbJobCompleted += callback;
    }

    public void UnregisterJobCompletedCallback(Action<Job> callback)
    {
        _cbJobCompleted -= callback;
    }

    public void RegisterJobCancelledCallback(Action<Job> callback)
    {
        _cbJobCancelled += callback;
    }   

    public void UnregisterJobCancelledCallback(Action<Job> callback)
    {
        _cbJobCancelled -= callback;
    }

    public void RegisterJobProgressedCallback(Action<Job> callback)
    {
        _cbJobProgressed += callback;
    }

    public void UnregisterJobProgressedCallback(Action<Job> callback)
    {
        _cbJobProgressed -= callback;
    }
    #endregion
}
