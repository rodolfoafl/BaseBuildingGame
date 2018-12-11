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
        _jobQueue.Enqueue(job);

        if(_cbJobCreated != null)
        {
            _cbJobCreated(job);
        }
    }

    public void RegisterJobCreationCallback(Action<Job> callback)
    {
        _cbJobCreated += callback;
    }

    public void UnregisterJobCreationCallback(Action<Job> callback)
    {
        _cbJobCreated -= callback;
    }

}
