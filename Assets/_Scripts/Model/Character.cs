using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character {

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
    #endregion

    public Character(Tile tile)
    {
        _currentTile = _destinationTile = _nextTile = tile;
    }

    void Update_Job(float deltaTime)
    {
        if (_myJob == null)
        {
            _myJob = _currentTile.World.JobQueue.Dequeue();

            if (_myJob != null)
            {
                _destinationTile = _myJob.Tile;
                _myJob.RegisterJobCompletedCallback(RegisterOnJobEnded);
                _myJob.RegisterJobCancelledCallback(RegisterOnJobEnded);
            }
        }

        if (_currentTile == _destinationTile)
        {
            if (_myJob != null)
            {
                _myJob.WorkOnJob(deltaTime);
            }
        }
    }

    void AbandonJob()
    {
        _nextTile = _destinationTile = _currentTile;
        _pathAStar = null;
        _currentTile.World.JobQueue.Enqueue(_myJob);
        _myJob = null;
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
           }
            _nextTile = _pathAStar.GetNextTile();
            if(_nextTile == _currentTile)
            {
                Debug.LogError("Update_Movement -- nextTile is currentTile?");
            }
        }

        //float distanceToTravel = Mathf.Sqrt(Mathf.Pow(_currentTile.X - _nextTile.X, 2) + Mathf.Pow(_currentTile.Y - _nextTile.Y, 2));

        float distanceToTravel = Vector2.Distance(new Vector2(_currentTile.X, _currentTile.Y), new Vector2(_nextTile.X, _nextTile.Y));

        float distanceThisFrame = _speed * deltaTime;

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

    public void SetDestination(Tile tile)
    {
        if (!_currentTile.IsNeighbour(tile, true))
        {
            Debug.Log("Character::SetDestination -- Our destination tile is not actually a neighbour!");
        }

        _destinationTile = tile;
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
        if(job != _myJob)
        {
            Debug.LogError("Character being told about job that isn't his!");
            return;
        }
        _myJob = null;
    }
    #endregion
}
