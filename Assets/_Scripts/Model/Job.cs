using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Job {

    Tile tile;
    float jobTIme;

    Action<Job> cbJobComplete;
    Action<Job> cbJobCancel;

    public Tile Tile
    {
        get
        {
            return tile;
        }

        set
        {
            tile = value;
        }
    }

    public Job(Tile tile, Action<Job> cbJobComplete, float jobTime = 1f)
    {
        this.tile = tile;
        this.cbJobComplete += cbJobComplete;
    }

    public void RegisterJobCompleteCallback(Action<Job> callback)
    {
        cbJobComplete += callback;
    }

    public void UnregisterJobCompleteCallback(Action<Job> callback)
    {
        cbJobComplete -= callback;
    }

    public void RegisterJobCancelCallback(Action<Job> callback)
    {
        cbJobCancel += callback;
    }

    public void UnregisterJobCancelCallback(Action<Job> callback)
    {
        cbJobCancel -= callback;
    }

    public void StartJob(float workTime)
    {
        jobTIme -= workTime;

        if (jobTIme <= 0)
        {
            if (cbJobComplete != null)
            {
                cbJobComplete(this);
            }
        }
    }

    public void CancelJob()
    {
        if (cbJobCancel != null)
        {
            cbJobCancel(this);
        }
    }


}
