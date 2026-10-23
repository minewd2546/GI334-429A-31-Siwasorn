using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField]
    private GameObject townPanel;
    public GameObject TownPanel { get { return townPanel; } set { townPanel = value; } }

    [SerializeField]
    private TerrainSlot centerSlot;

    [SerializeField]
    private TerrainSlot[] areaSlots;

    [SerializeField]
    private GameObject unitDragPrefab;

    [SerializeField]
    private GameObject outsideTownParent;

    [SerializeField]
    private GameObject yieldIconPrefab;
    public GameObject YieldIconPrefab { get { return yieldIconPrefab; } }

    [SerializeField]
    private List<GameObject> allUnitDrags;
    public List<GameObject> AllUnitDrags { get { return allUnitDrags; } set { allUnitDrags = value; } }

    [SerializeField]
    private TerrainSlot currentSlot;

    [SerializeField]
    private GameObject blockImage;

    [SerializeField]
    private GameObject professionPanel;

    [SerializeField]
    private TMP_Text labelQuestionText;

    [SerializeField]
    private TMP_Text[] btnYieldTexts;

    [SerializeField]
    private Transform foodParent;

    [SerializeField]
    private TMP_Text foodText;

    [SerializeField]
    private List<GameObject> foodIconList = new List<GameObject>();

    [SerializeField]
    private StockSlot[] stockSlots;

    [Header("Name Panel")]
    [SerializeField]
    private TMP_Text townNameText;

    [SerializeField]
    private TMP_Text yearText;

    [SerializeField]
    private TMP_Text goldText;

    void Awake()
    {
        instance = this;
    }

    public void SetupHexSlots(Hex centerHex, Hex[] aroundHexes)
    {
        centerSlot.HexSlotInit(centerHex);

        //setup auto Food production on CenterSlot
        if (centerSlot.Hex.YieldID == -1)
            centerSlot.SelectYield(0); //Change to Food
        else
            centerSlot.AdjustActualYield(); //Already Food

        for (int i = 0; i < areaSlots.Length; i++)
        {
            areaSlots[i].HexSlotInit(aroundHexes[i]);
        }
    }

    private void DestroyOldUnitDrag()
    {
        foreach (GameObject obj in allUnitDrags)
        {
            Destroy(obj);
        }
        allUnitDrags.Clear();
    }

    private void HideUnitWorkInTown(Hex hex)
    {
        foreach (Unit unit in hex.UnitsInHex)
        {
            if (unit.UnitStatus == UnitStatus.WorkInField)
                unit.gameObject.SetActive(false);
        }
    }

    public void SetupUnitDragOutsideTown(Hex hex)
    {
        foreach (Unit unit in hex.UnitsInHex)
        {
            if (unit.UnitType == UnitType.Land && unit.UnitStatus == UnitStatus.None)
            {
                GameObject unitObj = Instantiate(unitDragPrefab, outsideTownParent.transform);
                allUnitDrags.Add(unitObj);

                UnitDrag unitDrag = unitObj.GetComponent<UnitDrag>();
                unitDrag.UnitInit((LandUnit)unit);
            }
        }
    }

    public void SetupUnitDragWorkingInTerrain()
    {
        foreach (TerrainSlot terrainSlot in areaSlots)
        {
            if (terrainSlot.Hex == null)
                continue;

            if (terrainSlot.Hex.Labor != null)
            {
                GameObject unitObj = Instantiate(unitDragPrefab, terrainSlot.LaborParent);
                allUnitDrags.Add(unitObj);

                UnitDrag unitDrag = unitObj.GetComponent<UnitDrag>();
                unitDrag.UnitInit((LandUnit)terrainSlot.Hex.Labor);

                unitDrag.IconParent = terrainSlot.LaborParent;
                unitDrag.WorkAtNewTerrainSlot(terrainSlot);
            }
        }
    }

    public void SetupCurrentTown(Hex curHex, Hex[] aroundHexes)
    {
        UpdateNamePanel(curHex.Town);
        SetupHexSlots(curHex, aroundHexes);
        SetupUnitDragOutsideTown(curHex);
        SetupUnitDragWorkingInTerrain();
        SetupYieldInTerrain();
        UpdateTotalFoodIcons();
        SetupStockSlots(curHex);
    }

    public void SetupStockSlots(Hex hex)
    {
        for (int i = 0; i < stockSlots.Length; i++)
        {
            stockSlots[i].stockInit(i, hex.Town);
        }
    }

    //town name, year and the nation's treasury along the top strip
    public void UpdateNamePanel(Town town)
    {
        if (townNameText != null)
            townNameText.text = (town != null) ? town.TownName : string.Empty;

        if (yearText != null)
            yearText.text = GameManager.instance.GameYear.ToString();

        if (goldText != null)
        {
            Faction faction = (town != null) ? town.Faction : GameManager.instance.PlayerFaction;
            goldText.text = (faction != null) ? faction.Gold + " Gold" : string.Empty;
        }
    }

    // 38.21 : ending the turn with the town open should close it
    public void CheckActiveUIPanel()
    {
        if (townPanel.activeInHierarchy)
            ToggleTownPanel(false);
    }

    public void ToggleTownPanel(bool show)
    {
        if (show == false)
        {
            DestroyOldUnitDrag();
            RemoveAllYieldIcons();
            HideUnitWorkInTown(GameManager.instance.CurTown.CurHex);
        }
        townPanel.SetActive(show);
    }

    private void SetupYieldInTerrain()
    {
        foreach (TerrainSlot terrainSlot in areaSlots)
        {
            if (terrainSlot.Hex == null)
                continue;

            if (terrainSlot.Hex.Labor != null && terrainSlot.Hex.YieldID != -1)
            {
                //re-opening the town only redraws the icons; the yield was already
                //accumulated when the worker was placed (38.27)
                terrainSlot.AdjustActualYield();
            }
        }
    }

    public void RemoveAllYieldIcons()
    {
        centerSlot.RemoveYieldIcons();

        foreach (TerrainSlot terrainSlot in areaSlots)
        {
            if (terrainSlot == null)
                continue;

            terrainSlot.RemoveYieldIcons();
        }
    }

    // ---------------- 34.17 : the "pick a profession" dialog ----------------
    public void UpdateLabelQuestionText()
    {
        if (currentSlot == null)
            return;

        string s = string.Format("Select a profession for {0}", currentSlot.Hex.Labor.UnitName);
        labelQuestionText.text = s;
    }

    public void UpdateButtonTextsYield()
    {
        if (currentSlot == null)
            return;

        for (int i = 0; i < btnYieldTexts.Length; i++)
        {
            string s = string.Format("{0} {1}",
                currentSlot.NormalYield[i],
                GameManager.instance.ProductData[i].productName);

            btnYieldTexts[i].text = s;
        }
    }

    public void SelectProfession(TerrainSlot slot)
    {
        currentSlot = slot;

        UpdateLabelQuestionText();
        UpdateButtonTextsYield();

        blockImage.SetActive(true);
        professionPanel.SetActive(true);
    }

    public void SelectYield(int i)//Link to Select Profession Button on UI
    {
        Debug.Log($"Select: {i}");

        if (currentSlot != null)
            currentSlot.SelectYield(i);

        blockImage.SetActive(false);
        professionPanel.SetActive(false);

        if (currentSlot.Hex.YieldID == 0)
            UpdateTotalFoodIcons();
    }

    // ---------------- 35.6 : the town's total food for this turn ----------------
    public void SetupParentSpacing(int n, Transform parent, int iconWidth, int parentWidth)
    {
        if (n <= 1)
            return;

        HorizontalLayoutGroup layout = parent.GetComponent<HorizontalLayoutGroup>();

        int totalWidth = iconWidth * n;
        int excessWidth = totalWidth - parentWidth;

        if (excessWidth <= 0)
            return;

        int result = excessWidth / (n - 1);
        layout.spacing = -result;
    }

    public GameObject GenerateFoodIcon()
    {
        GameObject foodObj = Instantiate(yieldIconPrefab, foodParent);
        Image iconImg = foodObj.GetComponent<Image>();

        iconImg.sprite = GameManager.instance.ProductData[0].icons[0];

        return foodObj;
    }

    public void UpdateTotalFoodIcons()
    {
        foreach (GameObject obj in foodIconList)
            Destroy(obj);

        foodIconList.Clear();

        int total = GameManager.instance.CurTown.TotalYieldThisTurn[0];

        foodText.text = total.ToString();
        foodText.gameObject.SetActive(true);

        for (int i = 0; i < total; i++)
        {
            GameObject iconobj = GenerateFoodIcon();
            foodIconList.Add(iconobj);
        }
        SetupParentSpacing(total, foodParent, 64, 300);
    }
}
