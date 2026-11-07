using UnityEngine;
using UnityEngine.EventSystems;

public class ToNewWorldSlot : MonoBehaviour, IDropHandler
{
    [SerializeField]
    private EuropeManager EUMgr;

    [SerializeField]
    private UIManager UIMgr;

    void Awake()
    {
        EUMgr = EuropeManager.instance;
        UIMgr = UIManager.instance;
    }

    public void OnDrop(PointerEventData eventData)
    {
        //Debug.Log("On Drop - Out Town");

        GameObject unitObj = eventData.pointerDrag;
        ShipDrag shipDrag = unitObj.GetComponent<ShipDrag>();
        if (shipDrag == null)
            return;

        EUMgr.CheckIfPassengerInEuropeReadyToBoardShip(shipDrag.Ship);
        EUMgr.AllowToGoToNewWorld(shipDrag.Ship); //go to New World

        UIMgr.DestroyOldUnitDrag();
        UIMgr.SetupUnitDragInEuropePort();
    }
}
