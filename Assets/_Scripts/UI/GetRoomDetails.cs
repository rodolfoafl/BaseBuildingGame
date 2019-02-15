using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GetRoomDetails : MonoBehaviour {

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

        if(tile == null || tile.Room == null)
        {
            _text.text = "";
            return;
        }

        string details = "";
        foreach(string gas in tile.Room.GetGasNames())
        {
            details += gas + ": " + String.Format("{0:N}", tile.Room.GetGasAmount(gas)) + " (" + tile.Room.GetGasPercentage(gas) * 100 + "%) ";
        }

        _text.text = "Room Details: " + details;
    }
}
