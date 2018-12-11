using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobSpriteController : MonoBehaviour {

    InstalledObjectSpriteController _fSC;

    Dictionary<Job, GameObject> _jobGameObjectMap;

	// Use this for initialization
	void Start () {
        _fSC = FindObjectOfType<InstalledObjectSpriteController>();
        _jobGameObjectMap = new Dictionary<Job, GameObject>();

        WorldController.Instance.World.JobQueue.RegisterJobCreationCallback(OnJobCreated);
	}

    void OnJobCreated(Job job)
    {
        GameObject job_go = new GameObject();

        _jobGameObjectMap.Add(job, job_go);

        job_go.name = "JOB_" + job.JobObjectType + "_" + job.Tile.X + "_" + job.Tile.Y;
        job_go.transform.position = new Vector3(job.Tile.X, job.Tile.Y, 0);
        job_go.transform.SetParent(this.transform, true);

        SpriteRenderer sp = job_go.AddComponent<SpriteRenderer>();
        sp.sprite = _fSC.GetSpriteForInstalledObject(job.JobObjectType);
        sp.color = new Color(0.5f, 1f, 1f, 0.25f);
        sp.sortingLayerName = "InstalledObjects";

        job.RegisterJobCompletedCallback(OnJobEnded);
        job.RegisterJobCancelledCallback(OnJobEnded);
    }

    void OnJobEnded(Job job)
    {
        GameObject job_go = _jobGameObjectMap[job];
        job.UnregisterJobCancelledCallback(OnJobEnded);
        job.UnregisterJobCompletedCallback(OnJobEnded);

        Destroy(job_go);
    }
}
