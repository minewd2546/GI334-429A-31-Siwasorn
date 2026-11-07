using UnityEngine;
using UnityEngine.InputSystem;

public enum LandUnitType
{
    FreeColonists,
    VeteranSoldiers,
    HardyPioneers,
    PlainIndians,
    TropicalIndians,
    SeasonedScouts,
    Farmers,
    Fishermen,
    Lumberjacks,
    Carpenters,
    OreMiners,
    Blacksmiths
}

public class LandUnit : Unit
{
    [SerializeField]
    private LandUnitType landUnitType;
    public LandUnitType LandUnitType { get { return landUnitType; } set { landUnitType = value; } }

    [SerializeField]
    private int toolsNum; //max 100
    public int ToolsNum { get { return toolsNum; } set { toolsNum = value; } }

    [SerializeField]
    private int musketsNum; //max 50
    public int MusketsNum { get { return musketsNum; } set { musketsNum = value; } }

    [SerializeField]
    private int horseNum; //max 50
    public int HorseNum { get { return horseNum; } set { horseNum = value; } }

    [SerializeField]
    private bool armed = false;
    public bool Armed { get { return armed; } set { armed = value; } }

    [SerializeField]
    private bool hasMusket = false;
    public bool HasMusket { get { return hasMusket; } set { hasMusket = value; } }

    [SerializeField]
    private bool hasHorse = false;
    public bool HasHorse { get { return hasHorse; } set { hasHorse = value; } }

    [SerializeField]
    private NavalUnit transportShip;
    public NavalUnit TransportShip { get { return transportShip; } set { transportShip = value; } }

    public void UnitInit(GameManager gameMgr, UIManager uiMgr, Faction fact, LandUnitData data)
    {
        base.gameMgr = gameMgr;
        base.uiMgr = uiMgr;
        faction = fact;
        flagSprite.sprite = fact.ShieldIcon;

        unitName = data.unitName;
        movePointMax = data.movePointMax;
        movePoint = data.movePointMax;
        strength = data.strength;
        visualRange = data.visualRange;
        unitSprite.sprite = data.unitIcon;
        unitStatus = UnitStatus.None;

        landUnitType = data.landUnitType;
        if (landUnitType == LandUnitType.HardyPioneers)
            toolsNum = 100;

        armed = data.armed;

        hasMusket = data.hasMusket;
        if (hasMusket)
            musketsNum = 50;

        hasHorse = data.hasHorse;
        if (hasHorse)
            horseNum = 50;
    }

    // this project is set to the new Input System only, so Keyboard.current
    // replaces the manual's Input.GetKeyDown
    protected override void Update()
    {
        base.Update();

        if (Keyboard.current.bKey.wasPressedThisFrame)
        {
            if (this == gameMgr.CurUnit)
                BuildSettlement();
        }

        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            if (this == gameMgr.CurUnit)
                ClearingLand();
        }
    }

    private void MakeLandfall()
    {
        //Make Landfall
        unitStatus = UnitStatus.None;
        gameObject.transform.parent = faction.UnitParent.transform;
        transportShip.Passengers.Remove(this);
        transportShip = null;
    }

    public void ClearingLand()
    {
        if (landUnitType != LandUnitType.HardyPioneers)
            return;

        if (curHex.HexType == HexType.Ocean || curHex.HexType == HexType.Mountains
            || curHex.HexType == HexType.Hills || !curHex.HasForest)
        {
            //warning has to be cleared land
            Debug.Log("Must be on Forest");
        }
        else if (toolsNum < 20)
        {
            //warning not enough tools
            Debug.Log("Not enough tools");
            uiMgr.ShowColonyNotEnoughToolsText(toolsNum);
        }
        else
        {
            unitStatus = UnitStatus.Clearing;
            toolsNum -= 20;
        }
    }

    public void BuildSettlement()
    {
        if (landUnitType != LandUnitType.HardyPioneers)
            return;

        Debug.Log("Build Settlement");

        if (curHex.HexType == HexType.Ocean || curHex.HexType == HexType.Mountains || curHex.HasForest)
        {
            //warning has to be cleared land
            Debug.Log("Must be on Cleared Land");
            uiMgr.ShowColonyClearLandText();
        }
        else if (toolsNum < 20)
        {
            //warning not enough tools
            Debug.Log("Not enough tools");
            uiMgr.ShowColonyNotEnoughToolsText(toolsNum);
        }
        else if (gameMgr.CheckIfAroundHexHasTown(curHex))
        {
            Debug.Log("Must not be next to another town or village");
            uiMgr.ShowColonyHasOtherTownAroundText();
        }
        else
        {
            unitStatus = UnitStatus.Building;
            toolsNum -= 20;
        }
    }

    public void BoardingShip(NavalUnit ship)
    {
        ship.Passengers.Add(this);
        transportShip = ship;
        gameObject.transform.parent = ship.PassengerParent.transform;
        unitStatus = UnitStatus.OnBoard;
    }

    protected override void StayOnHex(Hex hex)
    {
        base.StayOnHex(hex);

        if (hex.HexType == HexType.Ocean)
        {
            //Check again if this ship can be boarded
            NavalUnit ship = gameMgr.CheckIfHexHasOurShipToBoard(hex);

            if (ship != null)
                BoardingShip(ship);
        }

        if (destinationHex != null)
            CheckMoveToDestination();

        if (faction == gameMgr.PlayerFaction)
            gameMgr.CheckUnmetFaction(this);
    }

    public override void PrepareMoveToHex(Hex targetHex) //Begin to move by RC or AI auto movement
    {
        //Debug.Log($"{unitName}:walks");

        if (targetHex.HexType != HexType.Ocean)
        {
            base.PrepareMoveToHex(targetHex);

            if (unitStatus == UnitStatus.OnBoard)
                MakeLandfall();
        }
        else
        {
            StayOnHex(curHex);
        }
    }
}
