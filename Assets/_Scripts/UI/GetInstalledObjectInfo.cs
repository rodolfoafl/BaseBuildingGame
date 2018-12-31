using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetInstalledObjectInfo : MonoBehaviour {

    Text _text;
    MouseController _mouseController;

    void Start()
    {
        _text = GetComponent<Text>();
        if (_text == null)
        {
            Debug.LogError("GetTileTypeInfo -- No Text UI component on this object!");
            this.enabled = false;
            return;
        }

        _mouseController = FindObjectOfType<MouseController>();
        if (_mouseController == null)
        {
            Debug.LogError("There is no instance of MouseController!");
        }

    }

    void Update()
    {
        Tile tile = _mouseController.GetMouseOverTile();

        string objType = "NULL";
        if(tile.InstalledObject != null)
        {
            objType = tile.InstalledObject.ObjectType;
        }

        _text.text = "InstalledObject Type: " + objType;
    }
}
