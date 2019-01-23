using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;

public class Character: IXmlSerializable {

    float _x;
    float _y;
    float _movementPercentage;
    float _speed = 5f;

    Tile _currentTile;
    Tile _destinationTile;
    Tile _nextTile;

    Action<Character> _cbCharacterMoved;

    Job _myJob;

    Path_AStar _pathAStar;

    LooseObject _looseObject;

    #region Properties
    public float X
    {
        get
        {
            return Mathf.Lerp(_currentTile.X, _nextTile.X, _movementPercentage);
        }
    }

    public float Y
    {
        get
        {
            return Mathf.Lerp(_currentTile.Y, _nextTile.Y, _movementPercentage);
        }
    }

    public Tile CurrentTile
    {
        get
        {
            return _currentTile;
        }
    }

    public Tile DestinationTile
    {
        get
        {
            return _destinationTile;
        }

        set
        {
            if(_destinationTile != value)
            {
                _destinationTile = value;
                _pathAStar = null;
            }
        }
    }

    public LooseObject LooseObject
    {
        get
        {
            return _looseObject;
        }

        set
        {
            _looseObject = value;
        }
    }
    #endregion

    public Character(Tile tile)
    {
        _currentTile = DestinationTile = _nextTile = tile;
    }

    void GetNewJob()
    {
        _myJob = _currentTile.World.JobQueue.Dequeue();

        if(_myJob == null)
        {
            return;
        }

        DestinationTile = _myJob.Tile;

        _myJob.RegisterJobCompletedCallback(RegisterOnJobEnded);
        _myJob.RegisterJobCancelledCallback(RegisterOnJobEnded);

        _pathAStar = new Path_AStar(WorldController.Instance.World, _currentTile, _destinationTile);
        if (_pathAStar.Length() == 0)
        {
            Debug.LogError("Path_AStart returned no path to current job tile!");
            AbandonJob();
        }
    }

    void Update_Job(float deltaTime)
    {
        if (_myJob == null)
        {
            GetNewJob();

            if (_myJob == null)
            {
                DestinationTile = _currentTile;
                return;
            }
        }

        if (!_myJob.HasAllMaterials())
        {
            if (_looseObject != null)
            {
                if (_myJob.RequiredLooseObjectAmount(_looseObject) > 0)
                {
                    if (_currentTile == _myJob.Tile)
                    {
                        WorldController.Instance.World.LooseObjectManager.PlaceLooseObject(_myJob, _looseObject);

                        _myJob.WorkOnJob(0);

                        if (_looseObject.StackSize == 0)
                        {
                            _looseObject = null;
                        }
                        else
                        {
                            Debug.LogError("Character is still carrying looseObject!");
                            _looseObject = null;
                        }
                    }
                    else
                    {
                        DestinationTile = _myJob.Tile;
                        return;
                    }
                }
                else
                {
                    if (!WorldController.Instance.World.LooseObjectManager.PlaceLooseObject(_currentTile, _looseObject))
                    {
                        Debug.LogError("Character tried to dump looseObject into an invalid tile!");
                        _looseObject = null;
                    }
                }
            }
            else
            {
                if(_currentTile.LooseObject != null 
                    && (_myJob.CanFetchFromStockpile || _currentTile.InstalledObject == null || !_currentTile.InstalledObject.IsStockpileJob()) 
                    && _myJob.RequiredLooseObjectAmount(_currentTile.LooseObject) > 0)
                {
                    WorldController.Instance.World.LooseObjectManager.PlaceLooseObject(this, _currentTile.LooseObject, _myJob.RequiredLooseObjectAmount(_currentTile.LooseObject));
                }
                else
                {
                    LooseObject required = _myJob.GetFirstRequiredLooseObject();
                    if (required != null)
                    {
                        LooseObject supplier = WorldController.Instance.World.LooseObjectManager.GetClosestLooseObjectOfType(required.ObjectType, _currentTile, required.MaxStackSize - required.StackSize, _myJob.CanFetchFromStockpile);

                        if (supplier == null)
                        {
                            //Debug.Log("No tile contains object of type '" + required.ObjectType + "'!");
                            AbandonJob();
                            return;
                        }

                        DestinationTile = supplier.Tile;
                        return;
                    }
                    return;
                }
            }          
            return;
        }

        DestinationTile = _myJob.Tile;

        if (_currentTile == _myJob.Tile)
        {
            _myJob.WorkOnJob(deltaTime);
        }
    }

    void Update_Movement(float deltaTime)
    {
        if(_currentTile == _destinationTile)
        {
            _pathAStar = null;
            return;
        }

        if(_nextTile == null || _nextTile == _currentTile)
        {
            if(_pathAStar == null || _pathAStar.Length() == 0)
            {
                _pathAStar = new Path_AStar(WorldController.Instance.World, _currentTile, _destinationTile);
                if(_pathAStar.Length() == 0)
                {
                    Debug.LogError("Path_AStart returned no path to destination!");
                    AbandonJob();
                    return;
                }

                _nextTile = _pathAStar.GetNextTile();
            }

            _nextTile = _pathAStar.GetNextTile();
            if(_nextTile == _currentTile)
            {
                Debug.LogError("Update_Movement -- nextTile is currentTile?");               
            }
        }

        //float distanceToTravel = Mathf.Sqrt(Mathf.Pow(_currentTile.X - _nextTile.X, 2) + Mathf.Pow(_currentTile.Y - _nextTile.Y, 2));

        /*if(_nextTile.MovementCost == 0)
        {
            Debug.LogError("A character was trying to enter an unwalkable tile!");
            _nextTile = null;
            _pathAStar = null;
            return;
        }*/

        switch (_nextTile.CheckEnterableState())
        {
            case EnterableState.No:
                Debug.LogError("A character was trying to enter an unwalkable tile!");
                _nextTile = null;
                _pathAStar = null;
                return;
            case EnterableState.Soon:
                return;
            case EnterableState.Yes:
                break;
            default:
                break;
        }

        float distanceToTravel = Vector2.Distance(new Vector2(_currentTile.X, _currentTile.Y), new Vector2(_nextTile.X, _nextTile.Y));

        float distanceThisFrame = (_speed / _nextTile.MovementCost) * deltaTime;

        float percentageThisFrame = distanceToTravel <= 0 ? 1 : distanceThisFrame / distanceToTravel;

        _movementPercentage += percentageThisFrame;
        if (_movementPercentage >= 1)
        {
            _currentTile = _nextTile;
            _movementPercentage = 0f;
        }
    }

    public void Update(float deltaTime)
    {
        Update_Job(deltaTime);
        Update_Movement(deltaTime);

        if (_cbCharacterMoved != null)
        {
            _cbCharacterMoved(this);
        }
    }

    void AbandonJob()
    {
        _nextTile = DestinationTile = _currentTile;
        _currentTile.World.JobQueue.Enqueue(_myJob);
        _myJob = null;
    }

    public void SetDestination(Tile tile)
    {
        if (!_currentTile.IsNeighbour(tile, true))
        {
            Debug.Log("Character::SetDestination -- Our destination tile is not actually a neighbour!");
        }

        DestinationTile = tile;
    }

    #region Callbacks
    public void RegisterCharacterMovedCallback(Action<Character> callback)
    {
        _cbCharacterMoved += callback;
    }

    public void UnregisterCharacterMovedCallback(Action<Character> callback)
    {
        _cbCharacterMoved -= callback;
    }

    void RegisterOnJobEnded(Job job)
    {
        job.UnregisterJobCancelledCallback(RegisterOnJobEnded);
        job.UnregisterJobCompletedCallback(RegisterOnJobEnded);

        if(job != _myJob)
        {
            Debug.LogError("Character being told about job that isn't his!");
            return;
        }
        _myJob = null;
    }
    #endregion

    #region Saving & Loading
    public Character()
    {

    }

    public XmlSchema GetSchema()
    {
        return null;
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteAttributeString("X", CurrentTile.X.ToString());
        writer.WriteAttributeString("Y", CurrentTile.Y.ToString());
    }

    public void ReadXml(XmlReader reader)
    {

    }
    #endregion
}
