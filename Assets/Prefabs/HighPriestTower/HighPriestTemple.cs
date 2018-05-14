
public class HighPriestTemple : CreationBuilding {

    protected override void Awake()
    {
        base.Awake();
        addAction(OrderName.Create0);
    }

    // Use this for initialization
    protected override void Start ()
    {
        base.Start();
    }
	
	// Update is called once per frame
	protected override void Update ()
    {
        base.Update();
    }
}