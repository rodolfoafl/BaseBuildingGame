using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character {

    float _x;
    float _y;
    float _movementPercentage;
    float _speed = 2f;

    Tile _currentTile;
    Tile _destinationTile;

    Action<Character> _cbCharacterMoved;

    Job _myJob;

    #region Properties
    public float X
    {
        get
        {
            return Mathf.Lerp(_currentTile.X, _destinationTile.X, _movementPercentage);
        }
    }

    public float Y
    {
        get
        {
            return Mathf.Lerp(_currentTile.Y, _destinationTile.Y, _movementPercentage);
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
        _currentTile = _destinationTile = tile;
    }

    public void Update(float deltaTime)
    {
        if(_myJob == null)
        {        
            _myJob = _currentTile.World.JobQueue.Dequeue();

            if(_myJob != null)
            {
                _destinationTile = _myJob.Tile;
                _myJob.RegisterJobCompletedCallback(RegisterOnJobEnded);
                _myJob.RegisterJobCancelledCallback(RegisterOnJobEnded);
            }
        }

        //Reached destination yet?
        if(_currentTile == _destinationTile)
        {
            if(_myJob != null)
            {
                _myJob.WorkOnJob(deltaTime);
            }

            return;
        }

        //The distance from point A to point B
        float distanceToTravel = Vector2.Distance(new Vector2(_currentTile.X, _currentTile.Y), new Vector2(_destinationTile.X, _destinationTile.Y));

        //Distance traveled this frame
        float distanceThisFrame = _speed * deltaTime;

        //Percentage to our destination
        float percentageThisFrame = distanceToTravel <= 0 ? 1 : distanceThisFrame / distanceToTravel;

        //Add to overral percentage travelled
        _movementPercentage += percentageThisFrame;
        if(_movementPercentage >= 1)
        {
            //Reached the destination
            _currentTile = _destinationTile;
            _movementPercentage = 0f;
        }

        if(_cbCharacterMoved != null)
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
