using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour {

    World _world;

    void Start()
    {
        _world = new World();
    }
}
