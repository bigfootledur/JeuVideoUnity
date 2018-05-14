using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGhost : MonoBehaviour {

    public bool canBeBuiltHere = true;
    [SerializeField] private int nbWorkersAssigned;

    private Color canBuildHere;
    private Color cantBuildHere;

    void Awake()
    {
        nbWorkersAssigned = 0;
        canBuildHere = new Color(0, 255, 0, 0.12f);
        cantBuildHere = new Color(255, 0, 0, 0.12f);
    }

    void Update()
    {
        //print(canBeBuiltHere);
        //print(nbWorkersAssigned);
        if (canBeBuiltHere)
            GetComponent<MeshRenderer>().materials[0].color = canBuildHere;
        else
            GetComponent<MeshRenderer>().materials[0].color = cantBuildHere;
    }

    public int NbWorkersAssigned
    {
        get { return nbWorkersAssigned; }
        set
        {
            nbWorkersAssigned = value;
            if (nbWorkersAssigned <= 0)
                Destroy(this.gameObject);
        }
    }

	void OnTriggerStay(Collider collider)
    {
        if (collider.GetComponent<Aggro>() || 
            collider.GetComponent<Projectile>() ||
            collider.GetComponent<TriggerXp>())
            return;

        if (!collider.GetComponent<RTSGameObject>())
            canBeBuiltHere = false;
        else if (collider.GetComponent<RTSGameObject>().faction != Faction.Red || collider.GetComponent<CreationBuilding>()
                                                                               || collider.GetComponent<BuildingGhost>())
            canBeBuiltHere = false;
    }
    void OnTriggerExit(Collider collider)
    {
        if (collider.GetComponent<Aggro>() ||
            collider.GetComponent<Projectile>())
            return;
        //print("exit");
        canBeBuiltHere = true;
    }
}
