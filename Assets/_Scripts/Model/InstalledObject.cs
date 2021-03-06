﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;

public class InstalledObject: IXmlSerializable{

    #region NOT DEFINITIVE
    Dictionary<string, float> _installedObjectParameters;
    Action<InstalledObject, float> _updateActions;

    public Func<InstalledObject, EnterableState> _checkEnterableState;

    public void Update(float deltaTime)
    {
        if(_updateActions != null)
        {
            _updateActions(this, deltaTime);
        }
    }
    #endregion

    Tile _tile;

    Action<InstalledObject> _cbOnInstalledObjectChanged;

    string _objectType;

    float _movementCost;

    int _width;
    int _height;

    bool _linksToNeighbor = false;
    bool _roomEnclosure;

    Func <Tile, bool> funcPositionValidation;

    List<Job> _jobs;

    Color _tint = Color.white;

    #region Properties
    public string ObjectType
    {
        get
        {
            return _objectType;
        }

        protected set
        {
            _objectType = value;
        }
    }

    public Tile Tile
    {
        get
        {
            return _tile;
        }
    }

    public bool LinksToNeighbor
    {
        get
        {
            return _linksToNeighbor;
        }
    }

    public float MovementCost
    {
        get
        {
            return _movementCost;
        }
        protected set
        {
            _movementCost = value;
        }
    }

    public Action<InstalledObject> CbOnInstalledObjectChanged
    {
        get
        {
            return _cbOnInstalledObjectChanged;
        }

        set
        {
            _cbOnInstalledObjectChanged = value;
        }
    }

    public bool RoomEnclosure
    {
        get
        {
            return _roomEnclosure;
        }

        set
        {
            _roomEnclosure = value;
        }
    }

    public List<Job> Jobs
    {
        get
        {
            return _jobs;
        }

        set
        {
            _jobs = value;
        }
    }

    public Color Tint
    {
        get
        {
            return _tint;
        }

        set
        {
            _tint = value;
        }
    }

    public int Width
    {
        get
        {
            return _width;
        }

        set
        {
            _width = value;
        }
    }

    public int Height
    {
        get
        {
            return _height;
        }

        set
        {
            _height = value;
        }
    }
    #endregion

    protected InstalledObject(InstalledObject other)
    {
        this._objectType = other._objectType;
        this._movementCost = other._movementCost;
        this._roomEnclosure = other._roomEnclosure;
        this._width = other._width;
        this._height = other._height;
        this._linksToNeighbor = other._linksToNeighbor;
        this._tint = other._tint;

        this._jobs = new List<Job>();
        this._installedObjectParameters = new Dictionary<string, float>(other._installedObjectParameters);
        if (other._updateActions != null)
        {
            this._updateActions = (Action<InstalledObject, float>)other._updateActions.Clone();
        }

        if(other.funcPositionValidation != null)
        {
            this.funcPositionValidation = (Func<Tile, bool>) other.funcPositionValidation.Clone();
        }

        this._checkEnterableState = other._checkEnterableState;
    }

    public virtual InstalledObject Clone()
    {
        return new InstalledObject(this);
    }

    public InstalledObject (string objectType, float movementCost = 1f, int width = 1, int height = 1, bool linksToNeighbor = false, bool roomEnclosure = false)
    {
        //InstalledObject obj = new InstalledObject();
        this._objectType = objectType;
        this._movementCost = movementCost;
        this._roomEnclosure = roomEnclosure;
        this._width = width;
        this._height = height;
        this._linksToNeighbor = linksToNeighbor;

        this.funcPositionValidation = this._IsValidPosition;

        this._installedObjectParameters = new Dictionary<string, float>();
        this._jobs = new List<Job>();
    }

    public static InstalledObject PlaceInstance(InstalledObject proto, Tile tile)
    {
        if(!proto.funcPositionValidation(tile))
        {
            Debug.LogError("PlaceInstance -- Position Validity Function returned FALSE!");
            return null;
        }

        InstalledObject obj = proto.Clone();
        obj._tile = tile;

        if (!tile.AssignInstalledObject(obj))
        {
            return null;
        }

        if (obj.LinksToNeighbor)
        {
            Tile t;
            int x = tile.X;
            int y = tile.Y;

            t = tile.World.GetTileAt(x, y + 1);
            if (t != null && t.InstalledObject != null && t.InstalledObject.ObjectType.Equals(obj.ObjectType) && t.InstalledObject._cbOnInstalledObjectChanged != null)
            {
                t.InstalledObject._cbOnInstalledObjectChanged(t.InstalledObject);
            }

            t = tile.World.GetTileAt(x + 1, y);
            if (t != null && t.InstalledObject != null && t.InstalledObject.ObjectType.Equals(obj.ObjectType) && t.InstalledObject._cbOnInstalledObjectChanged != null)
            {
                t.InstalledObject._cbOnInstalledObjectChanged(t.InstalledObject);
            }

            t = tile.World.GetTileAt(x, y - 1);
            if (t != null && t.InstalledObject != null && t.InstalledObject.ObjectType.Equals(obj.ObjectType) && t.InstalledObject._cbOnInstalledObjectChanged != null)
            {
                t.InstalledObject._cbOnInstalledObjectChanged(t.InstalledObject);
            }

            t = tile.World.GetTileAt(x - 1, y);
            if (t != null && t.InstalledObject != null && t.InstalledObject.ObjectType.Equals(obj.ObjectType) && t.InstalledObject._cbOnInstalledObjectChanged != null)
            {
                t.InstalledObject._cbOnInstalledObjectChanged(t.InstalledObject);
            }
        }

        return obj;
    }

    public bool IsValidPosition(Tile tile)
    {
        return funcPositionValidation(tile);
    }

    public bool _IsValidPosition(Tile tile)
    {
        for (int x_off = tile.X; x_off < (tile.X + Width); x_off++)
        {
            for (int y_off = tile.Y; y_off < (tile.Y + Height); y_off++)
            {
                Tile t2 = tile.World.GetTileAt(x_off, y_off);
                if(t2 != null)
                {
                    if (tile.Type != TileType.Floor)
                    {
                        return false;
                    }

                    if (tile.InstalledObject != null)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    public float GetParameter(string key, float defaultValue = 0)
    {
        if (!_installedObjectParameters.ContainsKey(key))
        {
            return defaultValue;
        }
        return _installedObjectParameters[key];
    }

    public void SetParameter(string key, float value)
    {
        _installedObjectParameters[key] = value;
    }

    public void ChangeParameter(string key, float value)
    {
        if (!_installedObjectParameters.ContainsKey(key))
        {
            _installedObjectParameters[key] = value;
        }
        _installedObjectParameters[key] += value;
    }


    public int JobCount()
    {
        return _jobs.Count;
    }

    public void AddJob(Job job)
    {
        _jobs.Add(job);
        WorldController.Instance.World.JobQueue.Enqueue(job);
    }

    public void RemoveJob(Job job)
    {
        _jobs.Remove(job);
        job.CancelJob();
        WorldController.Instance.World.JobQueue.Remove(job);
    }

    public void ClearJobs()
    {
        foreach(Job j in _jobs)
        {
            RemoveJob(j);
        }
    }

    public bool IsStockpileJob()
    {
        return _objectType == "Stockpile";
    }

    #region Callbacks
    public void RegisterUpdateAction(Action<InstalledObject, float> action)
    {
        _updateActions += action;
    }

    public void UnregisterUpdateAction(Action<InstalledObject, float> action)
    {
        _updateActions -= action;
    }

    public void RegisterOnInstalledObjectChangedCallback(Action<InstalledObject> callback)
    {
        _cbOnInstalledObjectChanged += callback;
    }

    public void UnregisterOnInstalledObjectChangedCallback(Action<InstalledObject> callback)
    {
        _cbOnInstalledObjectChanged -= callback;
    }
    #endregion

    #region Saving & Loading
    public InstalledObject()
    {
        _installedObjectParameters = new Dictionary<string, float>();
        _jobs = new List<Job>();
    }

    public XmlSchema GetSchema()
    {
        return null;
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteAttributeString("X", Tile.X.ToString());
        writer.WriteAttributeString("Y", Tile.Y.ToString());
        writer.WriteAttributeString("ObjectType", ObjectType);
        //writer.WriteAttributeString("MovementCost", MovementCost.ToString());

        foreach (string key in _installedObjectParameters.Keys) {
            writer.WriteStartElement("Parameter");
            writer.WriteAttributeString("name", key);
            writer.WriteAttributeString("value", _installedObjectParameters[key].ToString());
            writer.WriteEndElement();
        }
    }

    public void ReadXml(XmlReader reader)
    {
        //MovementCost = int.Parse(reader.GetAttribute("MovementCost"));

        if (reader.ReadToDescendant("Parameter"))
        {
            do
            {
                string key = reader.GetAttribute("name");
                float value = float.Parse(reader.GetAttribute("value"));
                _installedObjectParameters[key] = value;
            } while (reader.ReadToNextSibling("Parameter"));
        }
    }
    #endregion
}
