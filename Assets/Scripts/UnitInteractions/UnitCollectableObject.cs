using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitMove))]
public class UnitCollectableObject : UnitInteraction
{
    private UnitMove moveScript;
    private WalkingUnit self;
    private CollectableObject objectToTake;
    private CollectableObject objectToDrop;
    private Vector3 targetPosition;

    [SerializeField] private bool onTheMove = false;

    void Awake()
    {
        moveScript = GetComponent<UnitMove>();
        self = GetComponent<WalkingUnit>();
    }

	// Update is called once per frame
	void Update () {
        if (objectToTake)
        {
            if (moveScript.RemainingDistance() <= 15 && moveScript.RemainingDistance() > 0.01f)
            {
                self.TakeObject(objectToTake);
                StopAction();
            }
        }

        else if (targetPosition != Vector3.zero)
        {
            if (moveScript.RemainingDistance() <= 15 && moveScript.RemainingDistance() > 0.01f)
            {
                self.DropObject(objectToDrop, targetPosition);
                StopAction();
            }
        }
	}

    public void StopAction()
    {
        objectToTake = null;
        onTheMove = false;
        moveScript.StopAction();
        targetPosition = Vector3.zero;
        objectToDrop = null;
    }

    public void TakeObject(CollectableObject target)
    {        
        this.objectToTake = target;
        moveScript.Move(target.transform.position);
        onTheMove = true;
    }

    public void DropObject(CollectableObject target, Vector3 pos)
    {
        this.objectToDrop = target;
        targetPosition = pos;
        moveScript.Move(pos);
        onTheMove = true;
    }

    public override bool isInAction()
    {
        return onTheMove;
    }
}
