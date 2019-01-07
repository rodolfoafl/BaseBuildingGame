using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSortingLayer : MonoBehaviour {

    [SerializeField] string _sortingLayerName = "default";

	void Start()
    {
        GetComponent<Renderer>().sortingLayerName = _sortingLayerName;
    }
}
