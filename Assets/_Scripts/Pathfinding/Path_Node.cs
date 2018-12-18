using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path_Node<T> {

    T _data;

    Path_Edge<T>[] _edges;

    #region Properties
    public T Data
    {
        get
        {
            return _data;
        }
        set
        {
            _data = value;
        }
    }

    public Path_Edge<T>[] Edges
    {
        get
        {
            return _edges;
        }
        set
        {
            _edges = value;
        }
    }
    #endregion
}
