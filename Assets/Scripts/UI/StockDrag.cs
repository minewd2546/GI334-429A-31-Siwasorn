using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class StockDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    private StockSlot stockSlot;

    [SerializeField]
    private Image image;
    public Image Image { get { return image; } set { image = value; } }

    [SerializeField]
    private Cargo cargo;
    public Cargo Cargo { get { return cargo; } set { cargo = value; } }

    [SerializeField]
    private Transform iconParent;
    public Transform IconParent { get { return iconParent; } set { iconParent = value; } }

    public void StockDragInit(StockSlot slot)
    {
        stockSlot = slot;
        image.sprite = stockSlot.Image.sprite;
    }

    public void StockDragSetQuantity()
    {
        int qnty = 0;

        if (stockSlot.Quantity >= 100)
            qnty = 100;
        else
            qnty = stockSlot.Quantity;

        stockSlot.UpdateQuantityStock(-qnty);

        cargo = new Cargo(stockSlot.ProductId, qnty);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("Begin Drag");
        iconParent = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;

        StockDragSetQuantity();
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
        transform.position = stockSlot.Image.transform.position;

        stockSlot.UpdateQuantityStock(cargo.Quantity);
    }
}
