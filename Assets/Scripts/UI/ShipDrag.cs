using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShipDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    private Image shipImage;

    [SerializeField]
    private NavalUnit ship;
    public NavalUnit Ship { get { return ship; } set { ship = value; } }

    [SerializeField]
    private Transform iconParent;
    public Transform IconParent { get { return iconParent; } set { iconParent = value; } }

    public void Init(NavalUnit ship)
    {
        shipImage.sprite = ship.UnitSprite.sprite;
        this.ship = ship;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("Begin Drag");
        iconParent = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        shipImage.raycastTarget = false;
    }

    // the manual uses Input.mousePosition; this project is on the new Input System
    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("On Drag");
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("End Drag");
        transform.SetParent(iconParent);
        shipImage.raycastTarget = true;
        transform.position = iconParent.position;
    }
}
