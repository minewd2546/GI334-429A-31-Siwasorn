using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UnitDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [SerializeField]
    private Image unitImage;

    [SerializeField]
    private LandUnit landUnit;
    public LandUnit LandUnit { get { return landUnit; } set { landUnit = value; } }

    [SerializeField]
    private Transform iconParent;
    public Transform IconParent { get { return iconParent; } set { iconParent = value; } }

    [SerializeField]
    private TerrainSlot terrainSlot;

    [SerializeField]
    private Image statusIcon;

    [SerializeField]
    private UIManager uiMgr;

    void Awake()
    {
        uiMgr = UIManager.instance;
    }

    public void CheckStatusIcon()
    {
        if (statusIcon == null)
            return;

        switch (landUnit.UnitStatus)
        {
            case UnitStatus.ToBoard:
                statusIcon.gameObject.SetActive(true);
                statusIcon.sprite = landUnit.StatusSpritesList[2];
                break;
            default:
                statusIcon.gameObject.SetActive(false);
                break;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (landUnit.UnitStatus == UnitStatus.None || landUnit.UnitStatus == UnitStatus.ToBoard
            || landUnit.UnitStatus == UnitStatus.Hidden)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                uiMgr.CurLandUnit = landUnit;
                uiMgr.CurUnitIcon = this;
                uiMgr.ToggleOrderPanel(true);
            }
        }
    }

    public void UnitInit(LandUnit landUnit)
    {
        unitImage.sprite = landUnit.UnitSprite.sprite;
        this.landUnit = landUnit;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (uiMgr.InEurope)
            return;

        iconParent = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        unitImage.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (uiMgr.InEurope)
            return;

        // the manual uses Input.mousePosition; this project runs the new Input System
        transform.position = Mouse.current.position.ReadValue();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //OnBeginDrag never set iconParent in Europe, so the icon would lose its parent
        if (uiMgr.InEurope)
            return;

        transform.SetParent(iconParent);
        unitImage.raycastTarget = true;
    }

    public void QuitOldTerrainSlot()
    {
        if (terrainSlot != null)
        {
            terrainSlot.Hex.Labor = null;
            terrainSlot.ReduceTownYield();
            terrainSlot.RemoveYieldIcons();
            terrainSlot.Hex.YieldID = -1;//no yield for this hex
            terrainSlot = null;
        }
    }

    public void WorkAtNewTerrainSlot(TerrainSlot terrainSlot)
    {
        this.terrainSlot = terrainSlot;
    }
}
