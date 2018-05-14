using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitMove : UnitInteraction
{
    //private Vector3 destination; // A destination to reach
    private NavMeshAgent navigation;

    // Move
    private bool _isMoving;
    //[SerializeField] private float _moveSpeed = 10;
    //[SerializeField] private float _rotationSpeed = 10;
    private bool _destMoveReached; // If the moving object reached his destination

    // Patrol
    private Vector3 _startPosition;

    void Awake()
    {
        Navigation = GetComponent<NavMeshAgent>();
        _isMoving = false;
        _destMoveReached = false;
    }

    public void Move(Vector3 dest)
    {
        Navigation.velocity = new Vector3(0, 0, 0);
        Navigation.SetDestination(dest);
    }

    public void Patrol(Vector3 startpos, Vector3 dest)
    {
        print("fuck you in particular");
    }

    public void StopAction()
    {
        Navigation.velocity = new Vector3(0, 0, 0);
        Navigation.SetDestination(transform.position);
    }

    public float RemainingDistance()
    {
        return Navigation.remainingDistance;
    }

    #region Getters/Setters
    public override bool isInAction()
    {
        return !DestMoveReached;
    }

    public bool DestMoveReached
    {
        get { return Navigation.remainingDistance < 1f; }        
    }

    public NavMeshAgent Navigation
    {
        get
        {
            return navigation;
        }

        set
        {
            navigation = value;
        }
    }
    #endregion
}
