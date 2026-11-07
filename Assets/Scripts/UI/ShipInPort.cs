using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ShipInPort : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private Image unitImage;

    [SerializeField]
    private NavalUnit navalUnit;
    public NavalUnit NavalUnit { get { return navalUnit; } set { navalUnit = value; } }

    [SerializeField]
    private TMP_Text turnText;
    public TMP_Text TurnText { get { return turnText; } set { turnText = value; } }

    [SerializeField]
    private UIManager uiMgr;

    [SerializeField]
    private ShipDrag shipDrag;

    void Awake()
    {
        uiMgr = UIManager.instance;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Click at ship");

        if (uiMgr.InEurope)
            uiMgr.SetupSelectedShipsCargoSlotEurope(navalUnit);
        else
            uiMgr.SetupSelectedShipsCargoSlot(navalUnit);
    }

    public ShipDrag GenerateShipDrag(NavalUnit ship)
    {
        GameObject shipDragObj =
            Instantiate(uiMgr.ShipDragPrefab, transform.position, Quaternion.identity, transform);

        ShipDrag shipDrag = shipDragObj.GetComponent<ShipDrag>();
        shipDrag.Init(ship);

        return shipDrag;
    }

    public void UnitInit(NavalUnit navalUnit, bool genShipDrag)
    {
        unitImage.sprite = navalUnit.UnitSprite.sprite;
        this.navalUnit = navalUnit;

        if (genShipDrag)
        {
            shipDrag = GenerateShipDrag(navalUnit);
        }
    }

    public void UpdateTurnText(int turn)
    {
        turnText.gameObject.SetActive(true);
        turnText.text = ($"{turn}\nTurn(s)");
        turnText.color = Color.yellow;
    }
}
