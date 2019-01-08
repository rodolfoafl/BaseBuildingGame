using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InstalledObjectAction {

	public static void Door_UpdateAction(InstalledObject obj, float deltaTime)
    {
        //Debug.Log("Door_UpdateAction");
        if(obj.GetParameter("openingState") >= 1)
        {
            obj.ChangeParameter("openness", deltaTime * 4);
            if (obj.GetParameter("openness") >= 1)
            {
                obj.SetParameter("openingState", 0);
            }
        }
        else
        {
            obj.ChangeParameter("openness", deltaTime * -4);
        }

        obj.SetParameter("openness", Mathf.Clamp01(obj.GetParameter("openness")));

        if (obj.CbOnInstalledObjectChanged != null)
        {
            obj.CbOnInstalledObjectChanged(obj);
        }
    }

    public static EnterableState Door_EnterableState(InstalledObject obj)
    {
        obj.SetParameter("openingState", 1);

        if(obj.GetParameter("openness") >= 1)
        {
            return EnterableState.Yes;
        }
        return EnterableState.Soon;
    }

    public static void OnInstalledObjectJobCompleted(Job job)
    {
        WorldController.Instance.World.PlaceInstalledObject(job.JobObjectType, job.Tile);
        job.Tile.PendingInstalledObjectJob = null;
    }
}
