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
    private ShipInPort currentShipIcon; //current ship icon a player has selected

    [SerializeField]
    private StockSlot[] stockSlots; //town's warehouse slot

    [SerializeField]
    private CargoSlot[] cargoSlots; //ship's cargo hold
    public CargoSlot[] CargoSlots { get { return cargoSlots; } }

    [SerializeField]
    private List<GameObject> allShipIcons;
    public List<GameObject> AllShipIcons { get { return allShipIcons; } set { allShipIcons = value; } }

    [SerializeField]
    private GameObject shipInPortPrefab;

    [SerializeField]
    private GameObject portShipsParent;

    [SerializeField]
    private GameObject stockDragPrefab; //icon dragged from town's stock
    public GameObject StockDragPrefab { get { return stockDragPrefab; } }

    [SerializeField]
    private GameObject cargoDragPrefab; //icon dragged from ship's cargo

    [SerializeField]
    private TMP_Text moneyText;

    [SerializeField]
    private TMP_Text moneyEuropeText;

    [Header("Europe")]
    [SerializeField]
    private GameObject europePanel;

    [SerializeField]
    private GameObject toEuropeShipsParent;

    [SerializeField]
    private GameObject fromEuropeShipsParent;

    [SerializeField]
    private List<GameObject> allShipToEuropeIcons;

    [SerializeField]
    private List<GameObject> allShipFromEuropeIcons;

    [SerializeField]
    private List<GameObject> allShipInEuropeIcons;
    public List<GameObject> AllShipInEuropeIcons { get { return allShipInEuropeIcons; } }

    [SerializeField]
    private GameObject europePortShipsParent;

    [SerializeField]
    private CargoSlot[] cargoSlotsEurope; //ship's cargo hold (Europe)
    public CargoSlot[] CargoSlotsEurope { get { return cargoSlotsEurope; } }

    [SerializeField]
    private StockSlot[] stockSlotsEurope; //Europe's port slot

    [SerializeField]
    private bool inEurope;
    public bool InEurope { get { return inEurope; } }

    [SerializeField]
    private GameObject shipDragPrefab; //ship icon drag
    public GameObject ShipDragPrefab { get { return shipDragPrefab; } }

    [SerializeField]
    private GameObject purchaseShipPanel;
    public GameObject PurchaseShipPanel { get { return purchaseShipPanel; } }

    [SerializeField]
    private LandUnit curLandUnit;
    public LandUnit CurLandUnit { get { return curLandUnit; } set { curLandUnit = value; } }

    [SerializeField]
    private GameObject orderPanel;
    public GameObject OrderPanel { get { return orderPanel; } set { orderPanel = value; } }

    [SerializeField]
    private GameObject trainUnitPanel;
    public GameObject TrainUnitPanel { get { return trainUnitPanel; } }

    [SerializeField]
    private GameObject europePortUnitParent;

    [SerializeField]
    private List<GameObject> waitingLandUnitsInEuropeIcons; //icons of people waiting at the port
    public List<GameObject> WaitingLandUnitsInEuropeIcons { get { return waitingLandUnitsInEuropeIcons; } }

    [SerializeField]
    private UnitDrag curUnitIcon; //current land Unit icon a player has selected
    public UnitDrag CurUnitIcon { get { return curUnitIcon; } set { curUnitIcon = value; } }

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

    public void DestroyOldUnitDrag()
    {
        if (inEurope)
        {
            foreach (GameObject obj in waitingLandUnitsInEuropeIcons)
            {
                Destroy(obj);
            }
            waitingLandUnitsInEuropeIcons.Clear();
        }
        else
        {
            foreach (GameObject obj in allUnitDrags)
            {
                Destroy(obj);
            }
            allUnitDrags.Clear();
        }
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
        CheckShipArriveWithPassenger(curHex); //passengers walk ashore before the town icons are built
        UpdateNamePanel(curHex.Town);
        SetupHexSlots(curHex, aroundHexes);
        SetupUnitDragOutsideTown(curHex);
        SetupUnitDragWorkingInTerrain();
        SetupYieldInTerrain();
        UpdateTotalFoodIcons();
        SetupStockSlots(curHex);
        SetupShipsInPort(curHex);
        SetupShipsCargoSlot(curHex);
        UpdateMoneyText();
    }

    public void SetupStockSlots(Hex hex)
    {
        for (int i = 0; i < stockSlots.Length; i++)
        {
            stockSlots[i].stockInit(i, hex.Town, inEurope);
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

        if (europePanel.activeInHierarchy)
            ToggleEuropePanel(false);
    }

    public void ToggleTownPanel(bool show)
    {
        if (show == false)
        {
            DestroyOldUnitDrag();
            RemoveAllYieldIcons();
            HideUnitWorkInTown(GameManager.instance.CurTown.CurHex);
            DeleteAllCargoDragsInSlots(cargoSlots);
            DestroyAllShipIcons();
            DisableAllCargoSlots(cargoSlots);
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

    // ---------------- 39 : ships and cargo in the town's port ----------------
    private void SetupShipsInPort(Hex hex)
    {
        foreach (Unit unit in hex.UnitsInHex)
        {
            if (unit.UnitType == UnitType.Naval)
            {
                GameObject unitObj = Instantiate(shipInPortPrefab, portShipsParent.transform);
                allShipIcons.Add(unitObj);

                ShipInPort shipIcon = unitObj.GetComponent<ShipInPort>();
                shipIcon.UnitInit((NavalUnit)unit, false);
            }
        }
    }

    public void SetupCargoSlot(NavalUnit ship, CargoSlot[] cargoSlots)
    {
        for (int i = 0; i < cargoSlots.Length; i++)
        {
            if (i < ship.CargoHoldNum)
            {
                cargoSlots[i].gameObject.SetActive(true);
                cargoSlots[i].SlotInit(ship, i);
            }
            else
                cargoSlots[i].gameObject.SetActive(false);
        }
    }

    private void SetupShipsCargoSlot(Hex hex)
    {
        foreach (Unit unit in hex.UnitsInHex)
        {
            if (unit.UnitType == UnitType.Naval)
            {
                //first ship
                NavalUnit firstShip = unit.gameObject.GetComponent<NavalUnit>();
                SetupCargoSlot(firstShip, cargoSlots);
                UpdateCargoSlots(firstShip, cargoSlots);

                break;
            }
        }
    }

    private void DestroyAllShipIcons()
    {
        foreach (GameObject obj in allShipIcons)
        {
            Destroy(obj);
        }
        allShipIcons.Clear();
    }

    private void DisableAllCargoSlots(CargoSlot[] cargoSlots)
    {
        foreach (CargoSlot slot in cargoSlots)
        {
            slot.gameObject.SetActive(false);
        }
    }

    public void UpdateMoneyText()
    {
        moneyText.text = $"{GameManager.instance.PlayerFaction.Money}";
    }

    public void UpdateMoneyEuropeText()
    {
        moneyEuropeText.text = $"{GameManager.instance.PlayerFaction.Money}";
    }

    public void ToggleStockDragRaycast(bool flag)
    {
        foreach (StockSlot slot in stockSlots)
        {
            slot.ToggleRayCastStockDrag(flag);
        }
    }

    private void DeleteAllCargoDragsInSlots(CargoSlot[] cargoSlots)
    {
        foreach (CargoSlot slot in cargoSlots)
        {
            if (slot.CargoDrag != null)
            {
                Destroy(slot.CargoDrag.gameObject);
                slot.CargoDrag = null;
            }
        }
    }

    public void UpdateCargoSlots(NavalUnit ship, CargoSlot[] cargoSlots) //Update icon in ship's hold
    {
        DeleteAllCargoDragsInSlots(cargoSlots);

        for (int i = 0; i < ship.CargoList.Count; i++)
        {
            GameObject cargoDragObj =
                Instantiate(cargoDragPrefab, cargoSlots[i].transform.position, Quaternion.identity, cargoSlots[i].transform);

            CargoDrag cargoDrag = cargoDragObj.GetComponent<CargoDrag>();
            cargoSlots[i].CargoDrag = cargoDrag;

            int productId = ship.CargoList[i].ProductID;
            Sprite icon = GameManager.instance.ProductData[productId].icons[0];

            cargoDrag.CargoDragInit(ship, icon, i);
        }
    }

    // ---------------- 40 : Europe ----------------
    public void ToggleEuropePanel(bool flag)
    {
        if (flag == false)
        {
            DestroyAllShipsEuropeIcons();
            DisableAllCargoSlots(cargoSlotsEurope);

            europePanel.SetActive(false);
            inEurope = false;
        }
        else
        {
            townPanel.SetActive(false);
            europePanel.SetActive(true);
            inEurope = true;

            SetupShipsToEurope(EuropeManager.instance.ShipsToEurope);
            SetupShipsFromEurope(EuropeManager.instance.ShipsFromEurope);
            SetupShipsInEurope(EuropeManager.instance.ShipsInEurope);
            SetupShipsCargoSlotEurope(EuropeManager.instance.ShipsInEurope);
            DestroyOldUnitDrag();
            SetupUnitDragInEuropePort();
            SetupStockSlotsEurope();
            UpdateMoneyEuropeText();
        }
    }

    public void CheckEuropePanel()
    {
        if (europePanel.activeInHierarchy)
            ToggleEuropePanel(false);
        else
            ToggleEuropePanel(true);
    }

    private void SetupShipsToEurope(List<ShipInTransit> shipsToEU)
    {
        foreach (ShipInTransit shipInTransit in shipsToEU)
        {
            GameObject unitObj = Instantiate(shipInPortPrefab, toEuropeShipsParent.transform);
            allShipToEuropeIcons.Add(unitObj);

            ShipInPort shipIcon = unitObj.GetComponent<ShipInPort>();
            shipIcon.UnitInit(shipInTransit.Ship, false);
            shipIcon.UpdateTurnText(shipInTransit.TurnLeft);
        }
    }

    private void SetupShipsFromEurope(List<ShipInTransit> shipsToEU)
    {
        foreach (ShipInTransit shipInTransit in shipsToEU)
        {
            GameObject unitObj = Instantiate(shipInPortPrefab, fromEuropeShipsParent.transform);
            allShipFromEuropeIcons.Add(unitObj);

            ShipInPort shipIcon = unitObj.GetComponent<ShipInPort>();
            shipIcon.UnitInit(shipInTransit.Ship, false);
            shipIcon.UpdateTurnText(shipInTransit.TurnLeft);
        }
    }

    public void UpdateIconsFromEuropeToNewWorld()
    {
        DestroyShipIcons(allShipFromEuropeIcons);
        SetupShipsFromEurope(EuropeManager.instance.ShipsFromEurope);

        DestroyShipIcons(AllShipInEuropeIcons);
        SetupShipsInEurope(EuropeManager.instance.ShipsInEurope);

        DeleteAllCargoDragsInSlots(cargoSlotsEurope);
        DisableAllCargoSlots(cargoSlotsEurope);
        SetupShipsCargoSlotEurope(EuropeManager.instance.ShipsInEurope);
    }

    public void DestroyShipIcons(List<GameObject> shipList)
    {
        foreach (GameObject obj in shipList)
        {
            Destroy(obj);
        }
        shipList.Clear();
    }

    private void DestroyAllShipsEuropeIcons()
    {
        DestroyShipIcons(allShipToEuropeIcons);
        DestroyShipIcons(allShipFromEuropeIcons);
        DestroyShipIcons(allShipInEuropeIcons);
    }

    public void SetupShipsInEurope(List<NavalUnit> ships)
    {
        foreach (Unit unit in ships)
        {
            if (unit.UnitType == UnitType.Naval)
            {
                GameObject unitObj = Instantiate(shipInPortPrefab, europePortShipsParent.transform);
                allShipInEuropeIcons.Add(unitObj);

                ShipInPort shipIcon = unitObj.GetComponent<ShipInPort>();
                shipIcon.UnitInit((NavalUnit)unit, true);
            }
        }
    }

    private void SetupShipsCargoSlotEurope(List<NavalUnit> ships)
    {
        foreach (Unit unit in ships)
        {
            if (unit.UnitType == UnitType.Naval)
            {
                //first ship
                NavalUnit firstShip = unit.gameObject.GetComponent<NavalUnit>();
                SetupCargoSlot(firstShip, cargoSlotsEurope);
                UpdateCargoSlots(firstShip, cargoSlotsEurope);

                break;
            }
        }
    }

    public void SetupStockSlotsEurope()
    {
        for (int i = 0; i < stockSlotsEurope.Length; i++)
        {
            stockSlotsEurope[i].stockInitEurope(i);
        }
    }

    //Setup selected ship's cargo slot in New World
    public void SetupSelectedShipsCargoSlot(NavalUnit ship)
    {
        SetupCargoSlot(ship, cargoSlots);
        UpdateCargoSlots(ship, cargoSlots);
    }

    //Setup selected ship's cargo slot in Europe
    public void SetupSelectedShipsCargoSlotEurope(NavalUnit ship)
    {
        SetupCargoSlot(ship, cargoSlotsEurope);
        UpdateCargoSlots(ship, cargoSlotsEurope);
    }

    // ---------------- 45 : buying ships (the manual names these two but gives no code) ----------------
    public void TogglePurchasePanel(bool flag)
    {
        purchaseShipPanel.SetActive(flag);
    }

    public void AddNewShipInEurope(int i) //Map with Purchase Each Ship Button
    {
        if (!GameManager.instance.CheckShipPriceAndMoney(i))
            return;

        NavalUnit ship = GameManager.instance.GenerateHiddenShip(i);
        EuropeManager.instance.ShipsInEurope.Add(ship);

        //Hide ship's game object in map01
        ship.gameObject.SetActive(false);

        DestroyShipIcons(allShipInEuropeIcons);
        SetupShipsInEurope(EuropeManager.instance.ShipsInEurope);
        SetupShipsCargoSlotEurope(EuropeManager.instance.ShipsInEurope);

        GameManager.instance.PayShipPrice(i);
        UpdateMoneyEuropeText();
        TogglePurchasePanel(false);
    }

    // ---------------- 46 : training land units ----------------
    public void CheckShipArriveWithPassenger(Hex hex)
    {
        foreach (Unit unit in hex.UnitsInHex)
        {
            if (unit.UnitType == UnitType.Naval)
            {
                NavalUnit ship = (NavalUnit)unit;
                GameManager.instance.CheckPassengerArriving(ship, hex, ship.Faction);
            }
        }
    }

    public void ToggleOrderPanel(bool flag)
    {
        blockImage.SetActive(flag);
        orderPanel.SetActive(flag);
    }

    private void SwapUnitMemberInTownToTheFront(List<Unit> unitList)
    {
        if (curLandUnit == null)
            return;

        int i = unitList.IndexOf(curLandUnit);

        if (i == -1)
            return;

        Unit unit = unitList[i];

        unitList.RemoveAt(i);
        unitList.Insert(0, unit);
    }

    private void SwapUnitMemberInEuropeToTheFront(List<LandUnit> landUnitList)
    {
        if (curLandUnit == null)
            return;

        int i = landUnitList.IndexOf(curLandUnit);

        if (i == -1)
            return;

        LandUnit landUnit = landUnitList[i];

        landUnitList.RemoveAt(i);
        landUnitList.Insert(0, landUnit);
    }

    private void MoveToTheFront()
    {
        if (inEurope)
            SwapUnitMemberInEuropeToTheFront(EuropeManager.instance.LandUnitsInEurope);
        else
            SwapUnitMemberInTownToTheFront(GameManager.instance.CurTown.CurHex.UnitsInHex);
    }

    public void SetOrderForLandUnit(int i) //Map with Order Button
    {
        if (curLandUnit == null)
            return;

        switch (i)
        {
            case 0:
                curLandUnit.UnitStatus = UnitStatus.None;
                curUnitIcon.CheckStatusIcon();
                break;
            case 1:
                curLandUnit.UnitStatus = UnitStatus.ToBoard;
                curUnitIcon.CheckStatusIcon();
                break;
            case 2:
                MoveToTheFront();
                DestroyOldUnitDrag();

                if (inEurope)
                    SetupUnitDragInEuropePort();
                else
                {
                    SetupUnitDragOutsideTown(GameManager.instance.CurTown.CurHex);
                    SetupUnitDragWorkingInTerrain();
                }
                break;
        }
        ToggleOrderPanel(false);
    }

    public void ToggleTrainPanel(bool flag)
    {
        trainUnitPanel.SetActive(flag);
    }

    public void CreateUnitDragInEuropePort(int i) //Map with Train Each Unit Button
    {
        if (!GameManager.instance.CheckLandUnitPriceAndMoney(i))
            return;

        LandUnit landUnit = GameManager.instance.GenerateHiddenLandUnit(i);
        EuropeManager.instance.LandUnitsInEurope.Add(landUnit);

        //Hide unit's game object in map01
        landUnit.gameObject.SetActive(false);

        GameObject unitObj = Instantiate(unitDragPrefab, europePortUnitParent.transform);
        waitingLandUnitsInEuropeIcons.Add(unitObj);

        UnitDrag unitDrag = unitObj.GetComponent<UnitDrag>();
        unitDrag.UnitInit(landUnit);

        GameManager.instance.PayUnitPrice(i);
        UpdateMoneyEuropeText();
        ToggleTrainPanel(false);
    }

    public void SetupUnitDragInEuropePort()
    {
        foreach (LandUnit unit in EuropeManager.instance.LandUnitsInEurope)
        {
            if (unit.UnitType == UnitType.Land)
            {
                if (unit.UnitStatus == UnitStatus.None || unit.UnitStatus == UnitStatus.ToBoard
                    || unit.UnitStatus == UnitStatus.Hidden)
                {
                    GameObject unitObj = Instantiate(unitDragPrefab, europePortUnitParent.transform);
                    waitingLandUnitsInEuropeIcons.Add(unitObj);

                    UnitDrag unitDrag = unitObj.GetComponent<UnitDrag>();
                    unitDrag.UnitInit(unit);
                    unitDrag.CheckStatusIcon();
                }
            }
        }
    }

    // ---------------- 44.12 : colony advisor warnings ----------------
    public void ShowColonyClearLandText()
    {
        DialogManager.instance.ShowColonyClearLandText();
    }

    public void ShowColonyNotEnoughToolsText(int n)
    {
        DialogManager.instance.ShowColonyNotEnoughToolsText(n);
    }

    public void ShowColonyHasOtherTownAroundText()
    {
        DialogManager.instance.ShowColonyHasOtherTownAround();
    }
}
