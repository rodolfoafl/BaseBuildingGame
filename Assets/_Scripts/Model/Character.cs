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
        //Reached destination yet?
        if(_currentTile == _destinationTile)
        {
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
    }

    public void SetDestination(Tile tile)
    {
        if (!_currentTile.IsNeighbour(tile, true))
        {
            Debug.Log("Character::SetDestination -- Our destination tile is not actually a neighbour!");
        }

        _destinationTile = tile;
    }
}
