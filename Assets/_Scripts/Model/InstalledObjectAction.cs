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

    public static LooseObject[] Stockpile_GetItemsFromFilter()
    {
        return new LooseObject[] { new LooseObject("SteelPlate_", 50, 0) };
    }

    public static void Stockpile_UpdateAction(InstalledObject obj, float deltaTime)
    {
        if(obj.Tile.LooseObject != null && obj.Tile.LooseObject.StackSize >= obj.Tile.LooseObject.MaxStackSize)
        {
            obj.ClearJobs();
            return;
        }


        if (obj.JobCount() > 0)
        {
            return;
        }

        if (obj.Tile.LooseObject != null && obj.Tile.LooseObject.StackSize == 0)
        {
            Debug.LogError("Stockpile has a zero-size stack!");
            obj.ClearJobs();
            return;
        }

        LooseObject[] requiredItems;

        if(obj.Tile.LooseObject == null)
        {
            requiredItems = Stockpile_GetItemsFromFilter();
        }
        else
        {
            LooseObject desiredLooseObject = obj.Tile.LooseObject.Clone();
            desiredLooseObject.MaxStackSize -= desiredLooseObject.StackSize;
            desiredLooseObject.StackSize = 0;

            requiredItems = new LooseObject[] { desiredLooseObject };
        }

        Job job = new Job(obj.Tile, null, null, 0, requiredItems);

        job.CanFetchFromStockpile = false;

        job.RegisterJobProgressedCallback(Stockpile_JobProgressed);
        obj.AddJob(job);
    }

    public static void OnInstalledObjectJobCompleted(Job job)
    {
        WorldController.Instance.World.PlaceInstalledObject(job.JobObjectType, job.Tile);
        job.Tile.PendingInstalledObjectJob = null;
    }

    static void Stockpile_JobProgressed(Job job)
    {
        job.Tile.InstalledObject.RemoveJob(job);
        foreach (LooseObject obj in job.LooseObjectRequeriments.Values)
        {
            if (obj.StackSize > 0)
            {
                WorldController.Instance.World.LooseObjectManager.PlaceLooseObject(job.Tile, obj);
                return;
            }
        }
    }
}
