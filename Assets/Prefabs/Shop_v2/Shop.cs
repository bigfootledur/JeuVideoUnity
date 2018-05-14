using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : Building {

    [SerializeField] private Button objectDisplayPrefab;
    [SerializeField] private CollectableObject[] objectsToSell;
    [SerializeField] private TriggerShop triggerShop; // You need to be close to the shop to buy items
    [SerializeField] private GameObject objectsToSellDisplay;

    protected override void Awake()
    {
        base.Awake();
        TriggerShop = GetComponentInChildren<TriggerShop>();
        UpdateObjectsToSellDisplay();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    public void SellObject(CollectableObject objectToSell)
    {
        WalkingUnit buyer = TriggerShop.GetNearestOne();
        if(buyer == null)
        {
            print("Nobody is near");
            return;
        }

        if (!buyer.faction.Equals(GameMaster.PlayerFaction))
            return;

        if (buyer)
        {
            if (buyer && buyer.HasInventory() &&
                (buyer.faction.Equals(Faction.Blue) && (gameMaster.BlueWool >= objectToSell.MoneyCost)) ||
                (buyer.faction.Equals(Faction.Red) && (gameMaster.RedWool >= objectToSell.MoneyCost)))
            {
                bool objectSold = false;
                for (int i = 0; i < buyer.Inventory.Length; i++)
                {
                    if (buyer.Inventory[i] == null)
                    {
                        if (buyer.faction.Equals(Faction.Blue) && (gameMaster.BlueWool >= objectToSell.MoneyCost))
                            gameMaster.BlueWool -= objectToSell.MoneyCost;
                        else if (buyer.faction.Equals(Faction.Red) && (gameMaster.RedWool >= objectToSell.MoneyCost))
                            gameMaster.RedWool -= objectToSell.MoneyCost;
                        else
                            Debug.LogError("Couldn't take money from red/blue wool when selling " + objectToSell + " to " + buyer.gameObject);

                        CollectableObject obj = Instantiate(objectToSell);
                        obj.gameObject.SetActive(false);
                        buyer.TakeObject(obj);
                        objectSold = true;
                        break;
                    }
                }
                if (!objectSold)
                    print("Inventory full");
            }
        }
    }

    public void UpdateObjectsToSellDisplay()
    {
        // Cute cleanup
        for (int i = 0; i < objectsToSellDisplay.GetComponents<Button>().Length; i++)
            Destroy(objectsToSellDisplay.GetComponents<Button>()[i].gameObject);

        for (int i = 0; i < objectsToSell.Length; i++)
        {
            Button btn = Instantiate(objectDisplayPrefab, objectsToSellDisplay.transform);
            btn.GetComponent<ShopObject>().ImageDisplay.sprite = objectsToSell[i].ImageDisplay;
            btn.GetComponent<ShopObject>().slotNumber = i;
        }
    }

    public override bool doingNothing()
    {
        throw new NotImplementedException();
    }

    public override void SendStopOrder(bool directOrder, bool postpone)
    {
        throw new NotImplementedException();
    }

    public override void SendMouse1Order(Vector3 position, bool directOrder, bool postpone)
    {
        throw new NotImplementedException();
    }

    public override void SendMouse1Order(RTSGameObject target, bool directOrder, bool postpone)
    {
        throw new NotImplementedException();
    }

    public override void InitialiseButtons()
    {
        throw new NotImplementedException();
    }

    public CollectableObject[] ObjectsToSell
    {
        get { return objectsToSell; }
        set { objectsToSell = value; }
    }

    public TriggerShop TriggerShop
    {
        get
        {
            return triggerShop;
        }

        set
        {
            triggerShop = value;
        }
    }
}
