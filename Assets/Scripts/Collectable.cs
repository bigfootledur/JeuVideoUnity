using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Collectable : RTSGameObject {

    private float quantity;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public float Quantity
    {
        get
        {
            return quantity;
        }

        set
        {
            quantity = value;
        }
    }
}
