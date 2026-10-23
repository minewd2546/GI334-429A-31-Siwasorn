using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TerrainSlot : MonoBehaviour, IDropHandler
{
    [SerializeField]
    private Hex hex;
    public Hex Hex { get { return hex; } }

    [SerializeField]
    private Image terrainImage;

    [SerializeField]
    private Image forestImage;

    [SerializeField]
    private Image townImage;

    [SerializeField]
    private Transform yieldParent;

    [SerializeField]
    private Transform laborParent;
    public Transform LaborParent { get { return laborParent; } }

    //0-food, 1-sugar, 2-tobacco 3-cotton 4-fur 5-lumber 6-ore 7-silver
    [SerializeField]
    private int[] normalYield = new int[8];
    public int[] NormalYield { get { return normalYield; } set { normalYield = value; } }

    [SerializeField]
    private int[] actualYield = new int[8]; //Actual Yield labor is currently working
    public int[] ActualYield { get { return actualYield; } set { actualYield = value; } }

    [SerializeField]
    private bool centerHex;

    [SerializeField]
    private TMP_Text yieldText;

    [SerializeField]
    private List<GameObject> yieldIconList = new List<GameObject>();
    public List<GameObject> YieldIconList { get { return yieldIconList; } set { yieldIconList = value; } }

    [SerializeField]
    private GameManager gameMgr;
    [SerializeField]
    private UIManager uiMgr;

    void Awake()
    {
        gameMgr = GameManager.instance;
        uiMgr = UIManager.instance;
    }


    // The terrain art comes in two shapes: the new set is 256x256, the older pack
    // 256x384. Stretching both into one fixed rect makes them different sizes and
    // heights. Sprite pivot + pixelsPerUnit already mark the hex face (every hex is
    // HEX_WORLD_W units across, pivot at the face centre), so size and offset each
    // image from those instead.
    const float HEX_WORLD_W = 2.74f;   // must match the world map
    const float FACE_PX = 150f;        // how wide a hex face should be in this panel

    private static void FitHexImage(Image img, Sprite sprite)
    {
        if (img == null || sprite == null)
            return;

        float faceInSprite = HEX_WORLD_W * sprite.pixelsPerUnit;   // hex face, in sprite pixels
        float scale = FACE_PX / faceInSprite;

        RectTransform rt = img.rectTransform;
        rt.sizeDelta = sprite.rect.size * scale;

        // slide the sprite so its pivot (the hex face centre) lands on the slot centre
        Vector2 middle = sprite.rect.size * 0.5f;
        rt.anchoredPosition = (middle - sprite.pivot) * scale;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (centerHex || hex.Labor != null)
            return;

        GameObject unitObj = eventData.pointerDrag;
        UnitDrag unitDrag = unitObj.GetComponent<UnitDrag>();

        unitDrag.QuitOldTerrainSlot(); //old slot remove this labor
        unitDrag.WorkAtNewTerrainSlot(this); //this slot is remembered in this labor

        unitDrag.IconParent = laborParent;
        unitObj.transform.position = laborParent.position;

        unitDrag.LandUnit.UnitStatus = UnitStatus.WorkInField;
        hex.Labor = unitDrag.LandUnit;

        uiMgr.SelectProfession(this);
    }

    public void HexSlotInit(Hex hexData)
    {
        if (hexData != null)
        {
            hex = hexData;
            terrainImage.sprite = hex.TerrainSprite.sprite;
            FitHexImage(terrainImage, terrainImage.sprite);
            SetupNormalYield();

            if (hex.HasForest)
            {
                forestImage.sprite = hex.ForestSprite.sprite;
                FitHexImage(forestImage, forestImage.sprite);
                forestImage.gameObject.SetActive(true);
            }
            else
                forestImage.gameObject.SetActive(false);

            if (hex.HasTown)
            {
                townImage.sprite = hex.Town.TownSprite.sprite;
                FitHexImage(townImage, townImage.sprite);
                townImage.gameObject.SetActive(true);
            }
            else
                townImage.gameObject.SetActive(false);
        }
    }

    // 34.20 : fur and lumber only exist while the hex is forested; farming loses 2 to the canopy
    private void SetupNormalYield()
    {
        for (int i = 0; i < 8; i++)
        {
            normalYield[i] = hex.ResourceYield[i];

            if (i == 4 || i == 5) //Fur or Lumber
            {
                if (!hex.HasForest)
                    normalYield[i] = 0;
            }
            else //Agriculture
            {
                if (hex.HasForest)
                    normalYield[i] -= 2; //Reduce agriculture
            }

            if (hex.HasTown)
                normalYield[i] = Mathf.CeilToInt(normalYield[i] / 2f);

            if (normalYield[i] < 0)
                normalYield[i] = 0;
        }
    }

    public GameObject GenerateYieldIcon(int i)
    {
        GameObject yieldObj = Instantiate(uiMgr.YieldIconPrefab, yieldParent);
        Image iconImg = yieldObj.GetComponent<Image>();

        iconImg.sprite = gameMgr.ProductData[i].icons[0];

        if (i == 0)
        {
            if (hex.HexType == HexType.Ocean)
                iconImg.sprite = gameMgr.ProductData[0].icons[1];
            else
                iconImg.sprite = gameMgr.ProductData[0].icons[0];
        }

        return yieldObj;
    }

    public void GenerateYieldIcons(int id)
    {
        yieldText.text = actualYield[id].ToString();
        yieldText.gameObject.SetActive(true);

        for (int i = 0; i < actualYield[id]; i++)
        {
            GameObject iconobj = GenerateYieldIcon(id);
            yieldIconList.Add(iconobj);
        }
        uiMgr.SetupParentSpacing(actualYield[id], yieldParent, 64, 250);
    }

    public void ConvertNormalToActualYield(int id)
    {
        if (hex.YieldID == -1)
            return;

        //Formula to Adjust NormalYield *

        //convert normal Yield to actual yield
        actualYield[id] = normalYield[id];
    }

    public void AdjustActualYield()
    {
        //Formula to Adjust NormalYield *

        ConvertNormalToActualYield(hex.YieldID);

        //generate all yield icons in one hex
        GenerateYieldIcons(hex.YieldID);
    }

    private void Accumulate()
    {
        gameMgr.CurTown.TotalYieldThisTurn[hex.YieldID] += actualYield[hex.YieldID];
    }

    public void ReduceTownYield()
    {
        if (hex.YieldID != -1)
        {
            gameMgr.CurTown.TotalYieldThisTurn[hex.YieldID] -= actualYield[hex.YieldID];
            actualYield[hex.YieldID] = 0;
        }
    }

    public void RemoveYieldIcons()
    {
        foreach (GameObject obj in yieldIconList)
        {
            Destroy(obj);
        }
        yieldIconList.Clear();
        yieldText.gameObject.SetActive(false);

        if (hex != null && hex.YieldID == 0)
            uiMgr.UpdateTotalFoodIcons();
    }

    // 34.18 : chosen from the profession dialog
    public void SelectYield(int i)
    {
        hex.YieldID = i;
        AdjustActualYield();
        Accumulate();
    }
}
