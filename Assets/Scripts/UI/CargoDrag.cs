using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class CargoDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    private Image image;
    public Image Image { get { return image; } set { image = value; } }

    [SerializeField]
    private Cargo cargo;
    public Cargo Cargo { get { return cargo; } set { cargo = value; } }

    [SerializeField]
    private Transform iconParent;
    public Transform IconParent { get { return iconParent; } set { iconParent = value; } }

    [SerializeField]
    private TMP_Text cargoText;

    [SerializeField]
    private NavalUnit ship;
    public NavalUnit Ship { get { return ship; } set { ship = value; } }

    [SerializeField]
    private int cargoListId;

    [SerializeField]
    private UIManager uiMgr;

    [SerializeField]
    private bool dropped; //already unloaded into a StockSlot

    void Awake()
    {
        uiMgr = UIManager.instance;
    }

    public void UpdateCargoText()
    {
        cargoText.text = cargo.Quantity.ToString();
    }

    public void CargoDragDeductQuantity()
    {
        ship.CargoList[cargoListId].Quantity = 0;
    }

    public void CargoDragInit(NavalUnit ship, Sprite sprite, int id)
    {
        this.ship = ship;
        image.sprite = sprite;

        Cargo c = ship.CargoList[id];
        cargo = new Cargo(c.ProductID, c.Quantity);
        cargoListId = id;

        UpdateCargoText();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("Begin Drag");
        iconParent = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;

        CargoDragDeductQuantity();
        //Debug.Log("CargoDrag - Begin Drag");
        uiMgr.ToggleStockDragRaycast(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("On Drag");
        // the manual uses Input.mousePosition; this project runs the new Input System
        transform.position = Mouse.current.position.ReadValue();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("End Drag");
        transform.SetParent(iconParent);
        image.raycastTarget = true;
        transform.position = iconParent.transform.position;

        //return
        // not in the manual: OnEndDrag still fires after StockSlot.OnDrop removed this
        // cargo, and cargoListId would then point at the next hold (or past the list)
        if (!dropped)
            ship.CargoList[cargoListId].Quantity += cargo.Quantity;

        //Debug.Log("CargoDrag - End Drag - Return");
        uiMgr.ToggleStockDragRaycast(true);
    }

    public void RemoveCargoListFromShip()
    {
        ship.CargoList.RemoveAt(cargoListId);
        dropped = true;
    }
}
