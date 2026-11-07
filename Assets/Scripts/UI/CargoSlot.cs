using UnityEngine;
using UnityEngine.EventSystems;

public class CargoSlot : MonoBehaviour, IDropHandler
{
    [SerializeField]
    private NavalUnit ship;

    [SerializeField]
    private int holdId;

    [SerializeField]
    private UIManager uiMgr;

    [SerializeField]
    private GameManager gameMgr;

    [SerializeField]
    private EuropeManager EUMgr;

    [SerializeField]
    private CargoDrag cargoDrag;
    public CargoDrag CargoDrag { get { return cargoDrag; } set { cargoDrag = value; } }

    void Awake()
    {
        uiMgr = UIManager.instance;
        gameMgr = GameManager.instance;
        EUMgr = EuropeManager.instance;
    }

    public void SlotInit(NavalUnit ship, int holdId)
    {
        this.ship = ship;
        this.holdId = holdId;
    }

    public void OnDrop(PointerEventData eventData)
    {
        //Debug.Log($"Drop on Slot:{holdId}");

        if (ship.CargoList.Count > holdId)//there's something in this hold
        {
            if (ship.CargoList[holdId].Quantity >= 100)
                return;
        }
        GameObject obj = eventData.pointerDrag;
        StockDrag stockDrag = obj.GetComponent<StockDrag>();

        if (stockDrag == null)
            return;

        /*cargoDrag.IconParent = transform;
        cargoObj.transform.position = transform.position;*/

        int cargoLeft = ship.AddCargo(holdId, stockDrag.Cargo);

        if (uiMgr.InEurope)
        {
            uiMgr.UpdateCargoSlots(ship, uiMgr.CargoSlotsEurope);
            //Buy from market
            gameMgr.PlayerFaction.Money -=
                stockDrag.Cargo.Quantity * EUMgr.EuropeStocks[stockDrag.Cargo.ProductID].AskPrice;

            uiMgr.UpdateMoneyEuropeText();
        }
        else
            uiMgr.UpdateCargoSlots(ship, uiMgr.CargoSlots);

        stockDrag.Cargo.Quantity = cargoLeft;
    }
}
