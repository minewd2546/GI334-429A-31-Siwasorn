using System.Collections.Generic;
using UnityEngine;

public enum NavalUnitType
{
    Caravel,
    Merchantman,
    Galleon,
    Privateer,
    Frigate,
    ManOWar
}

public class NavalUnit : Unit
{
    [SerializeField]
    private NavalUnitType navalUnitType;
    public NavalUnitType NavalUnitType { get { return navalUnitType; } set { navalUnitType = value; } }

    [SerializeField]
    private bool armed = false;

    [SerializeField]
    private int cargoHoldNum;
    public int CargoHoldNum { get { return cargoHoldNum; } }

    [SerializeField]
    private List<LandUnit> passengers = new List<LandUnit>();
    public List<LandUnit> Passengers { get { return passengers; } set { passengers = value; } }

    [SerializeField]
    private GameObject passengerParent;
    public GameObject PassengerParent { get { return passengerParent; } }

    [SerializeField]
    private List<Cargo> cargoList = new List<Cargo>();
    public List<Cargo> CargoList { get { return cargoList; } set { cargoList = value; } }

    public void UnitInit(GameManager gameMgr, Faction fact, NavalUnitData data)
    {
        base.gameMgr = gameMgr;
        faction = fact;
        flagSprite.sprite = fact.ShieldIcon;

        unitName = data.unitName;
        movePointMax = data.movePointMax;
        movePoint = data.movePointMax;
        strength = data.strength;
        visualRange = data.visualRange;
        unitSprite.sprite = data.shipIcon;

        navalUnitType = data.navalUnitType;
        armed = data.armed;
        cargoHoldNum = data.cargoHoldNum;
    }

    public int AddCargo(int id, Cargo newCargo)
    {
        //Debug.Log($"i:{id}, count:{cargoList.Count}");

        if (cargoList.Count < cargoHoldNum) //a space left
        {
            cargoList.Add(new Cargo(newCargo.ProductID, newCargo.Quantity));
            return 0;
        }
        //all cargo taken, check if there is space
        else if (cargoList[id].ProductID == newCargo.ProductID)
        {
            cargoList[id].Quantity += newCargo.Quantity;
            newCargo.Quantity = 0;

            if (cargoList[id].Quantity > 100)
            {
                int cargoLeft = cargoList[id].Quantity - 100;
                cargoList[id].Quantity = 100;
                return cargoLeft;
            }
        }
        //Debug.Log("Added Cargo");
        return newCargo.Quantity;
    }

    public override void PrepareMoveToHex(Hex targetHex) //Begin to move by RC or AI auto movement
    {
        base.PrepareMoveToHex(targetHex);

        if (targetHex.HexType != HexType.Ocean) //Land Hex
        {
            if (targetHex.HasTown) //If has town
            {
                //not our town
                if (targetHex.Town.Faction != gameMgr.PlayerFaction)
                    StayOnHex(curHex);
            }
            else //No town
                StayOnHex(curHex);
        }
    }

    protected override void StayOnHex(Hex hex)
    {
        base.StayOnHex(hex);

        foreach (LandUnit passenger in passengers)
        {
            //Remove passenger from old ocean hex
            passenger.CurHex.UnitsInHex.Remove(passenger);
            //Add passenger to new ocean hex
            hex.UnitsInHex.Add(passenger);

            passenger.CurHex = hex;
            passenger.CurPos = hex.Pos;
        }
        if (faction == gameMgr.PlayerFaction)
            gameMgr.CheckUnmetFaction(this);

        gameMgr.CheckShipToEurope();

        if (destinationHex != null)
            CheckMoveToDestination();
    }
}
