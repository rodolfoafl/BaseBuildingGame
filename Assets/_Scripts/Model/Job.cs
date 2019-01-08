using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Job {

    Tile _tile;

    float _jobTime;

    string _jobObjectType; 

    Action<Job> _cbJobCompleted;
    Action<Job> _cbJobCancelled;

    Dictionary<string, LooseObject> _looseObjectRequeriments;

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
    #endregion

    public Job(Tile tile, string jobObjectType, Action<Job> cbJobCompleted, float jobTime, LooseObject[] looseObjects)
    {
        this._tile = tile;
        this._jobObjectType = jobObjectType;
        this._cbJobCompleted += cbJobCompleted;
        this._jobTime = jobTime;

        this._looseObjectRequeriments = new Dictionary<string, LooseObject>();
        if(_looseObjectRequeriments != null)
        {
            foreach (LooseObject obj in _looseObjectRequeriments.Values)
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
        if (_looseObjectRequeriments != null)
        {
            foreach (LooseObject obj in other.LooseObjectRequeriments.Values)
            {
                this._looseObjectRequeriments[obj.ObjectType] = obj.Clone();
            }
        }
    }

    public virtual Job Clone()
    {
        return new Job(this);
    }

    public void WorkOnJob(float workTime)
    {
        _jobTime -= workTime;
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
    }

    #region Callbacks
    public void RegisterJobCompletedCallback(Action<Job> callback)
    {
        _cbJobCompleted += callback;
    }

    public void RegisterJobCancelledCallback(Action<Job> callback)
    {
        _cbJobCancelled += callback;
    }

    public void UnregisterJobCompletedCallback(Action<Job> callback)
    {
        _cbJobCompleted -= callback;
    }

    public void UnregisterJobCancelledCallback(Action<Job> callback)
    {
        _cbJobCancelled -= callback;
    }
    #endregion
}
