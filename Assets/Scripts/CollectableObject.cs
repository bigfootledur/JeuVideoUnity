using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class CollectableObject : RTSGameObject {

    [SerializeField] private Sprite imageDisplay;
    [SerializeField] private ParticleSystem pSystem;
    [SerializeField] private int moneyCost;
    private bool objectTaken = false;
    [SerializeField] private bool canBeTaken = true;
    private WalkingUnit owner;

    public Buff buffObject;

    protected override void Awake()
    {
        base.Awake();
        pSystem = GetComponentInChildren<ParticleSystem>();
    }

    public void TakeObject()
    {
        this.gameObject.SetActive(false);
        this.gameObject.transform.position = new Vector3(0, 0, 0);
    }
    public void TakeObjectAnimation()
    {
        Animator.SetTrigger("Taken");
        pSystem.Stop();
    }

    public Sprite ImageDisplay
    {
        get { return imageDisplay; }
        set { imageDisplay = value; }
    }

    public int MoneyCost
    {
        get
        {
            return moneyCost;
        }

        set
        {
            moneyCost = value;
        }
    }

    public bool ObjectTaken
    {
        get
        {
            return objectTaken;
        }

        set
        {
            objectTaken = value;
        }
    }

    public bool CanBeTaken
    {
        get
        {
            return canBeTaken;
        }

        set
        {
            canBeTaken = value;
        }
    }

    public Buff BuffObject
    {
        get
        {
            return buffObject;
        }

        set
        {
            print(buffObject);
            buffObject = value;
            print(buffObject);
        }
    }
}
