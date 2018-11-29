using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Job {

    Tile _tile;

    float _jobTile;

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
    #endregion

    public Job(Tile tile, Action<Job> cbJobCompleted, float jobTime = 1f)
    {
        this._tile = tile;
        this._cbJobCompleted += cbJobCompleted;
    }

    public void CompleteJob(float workTime)
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
    #endregion
}
