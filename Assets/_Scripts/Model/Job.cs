using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Job {

    Tile _tile;

    float _jobTile;

    string _jobObjectType; 

    Action<Job> _cbJobCompleted;
    Action<Job> _cbJobCancelled;

    #region Properties
    public Tile Tile
    {
        get
        {
            return _tile;
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
    #endregion

    public Job(Tile tile, string jobObjectType, Action<Job> cbJobCompleted, float jobTime = 1f)
    {
        this._tile = tile;
        this._jobObjectType = jobObjectType;
        this._cbJobCompleted += cbJobCompleted;
    }

    public void WorkOnJob(float workTime)
    {
        _jobTile -= workTime;
        if (_jobTile <= 0)
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
