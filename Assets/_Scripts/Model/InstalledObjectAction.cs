using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InstalledObjectAction {

	public static void Door_UpdateAction(InstalledObject obj, float deltaTime)
    {
        //Debug.Log("Door_UpdateAction");
        if(obj._installedObjectParameters["openingState"] >= 1)
        {
            obj._installedObjectParameters["openness"] += deltaTime * 4;
            if(obj._installedObjectParameters["openness"] >= 1)
            {
                obj._installedObjectParameters["openingState"] = 0;
            }
        }
        else
        {
            obj._installedObjectParameters["openness"] -= deltaTime * 4;
        }

        obj._installedObjectParameters["openness"] = Mathf.Clamp01(obj._installedObjectParameters["openness"]);

        if (obj.CbOnInstalledObjectChanged != null)
        {
            obj.CbOnInstalledObjectChanged(obj);
        }
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
