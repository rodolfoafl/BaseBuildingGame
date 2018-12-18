using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path_Edge<T> {

    float _cost;

    Path_Node<T> _node;


    #region Properties
    public Path_Node<T> Node
    {
        get
        {
            return _node;
        }
        set
        {
            _node = value;
        }
    }

    public float Cost
    {
        get
        {
            return _cost;
        }
        set
        {
            _cost = value;
        }
    }
    #endregion
}
