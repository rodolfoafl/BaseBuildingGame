using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobQueue {
    Queue<Job> _jobQueue;

    Action<Job> _cbJobCreated;

    public JobQueue()
    {
        _jobQueue = new Queue<Job>();
    }

    public void Enqueue(Job job)
    {
        if(job.JobTime < 0)
        {
            job.WorkOnJob(0);
            return;
        }

        _jobQueue.Enqueue(job);

        if(_cbJobCreated != null)
        {
            _cbJobCreated(job);
        }
    }

    public Job Dequeue()
    {
        if (_jobQueue.Count > 0)
        {
            return _jobQueue.Dequeue();
        }
        else
        {
            return null;
        }
    }

    public void Remove(Job job)
    {
        if (!_jobQueue.Contains(job))
        {
            //Debug.LogError("Trying to remove a job that doesn't exist on the queue!");
            return;
        }

        List<Job> jobs = new List<Job>(_jobQueue);
        jobs.Remove(job);
        _jobQueue = new Queue<Job>(jobs);
    }

    #region Callbacks
    public void RegisterJobCreationCallback(Action<Job> callback)
    {
        _cbJobCreated += callback;
    }

    public void UnregisterJobCreationCallback(Action<Job> callback)
    {
        _cbJobCreated -= callback;
    }
    #endregion
}
