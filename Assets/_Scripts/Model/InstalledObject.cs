using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;

public class InstalledObject: IXmlSerializable{

    #region NOT DEFINITIVE
    public Dictionary<string, float> _installedObjectParameters;
    public Action<InstalledObject, float> _updateActions;

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

    Func <Tile, bool> funcPositionValidation;

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
    #endregion

    /// <summary>
    /// Copy Constructor
    /// </summary>
    /// <param name="other">InstalledObject to be copied</param>
    protected InstalledObject(InstalledObject other)
    {
        this._objectType = other._objectType;
        this._movementCost = other._movementCost;
        this._width = other._width;
        this._height = other._height;
        this._linksToNeighbor = other._linksToNeighbor;

        this._installedObjectParameters = new Dictionary<string, float>(other._installedObjectParameters);
        if (other._updateActions != null)
        {
            this._updateActions = (Action<InstalledObject, float>)other._updateActions.Clone();
        }
    }

    virtual public InstalledObject Clone()
    {
        return new InstalledObject(this);
    }

    public InstalledObject (string objectType, float movementCost = 1f, int width = 1, int height = 1, bool linksToNeighbor = false)
    {
        //InstalledObject obj = new InstalledObject();
        this._objectType = objectType;
        this._movementCost = movementCost;
        this._width = width;
        this._height = height;
        this._linksToNeighbor = linksToNeighbor;

        this.funcPositionValidation = this._IsValidPosition;

        this._installedObjectParameters = new Dictionary<string, float>();
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
        if(tile.Type != TileType.Floor)
        {
            return false;
        }

        if(tile.InstalledObject != null)
        {
            return false;
        }

        return true;
    }

    #region Callbacks
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
