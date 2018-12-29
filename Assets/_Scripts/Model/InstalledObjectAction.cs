using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InstalledObjectAction {

	public static void Door_UpdateAction(InstalledObject obj, float deltaTime)
    {
        //Debug.Log("Door_UpdateAction");
        if(obj._installedObjectParameters["openingState"] >= 1)
        {
            obj._installedObjectParameters["openness"] += deltaTime;
            if(obj._installedObjectParameters["openness"] >= 1)
            {
                obj._installedObjectParameters["openingState"] = 0;
            }
        }
        else
        {
            obj._installedObjectParameters["openness"] -= deltaTime;
        }

        obj._installedObjectParameters["openness"] = Mathf.Clamp01(obj._installedObjectParameters["openness"]);
    }

    public static EnterableState Door_EnterableState(InstalledObject obj)
    {
        obj._installedObjectParameters["openingState"] = 1;

        if(obj._installedObjectParameters["openness"] >= 1)
        {
            return EnterableState.Yes;
        }
        return EnterableState.Soon;
    }
}
