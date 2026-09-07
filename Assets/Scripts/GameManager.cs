using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Grid grid;

    [SerializeField]
    private Transform hexParent;

    [SerializeField]
    private GameObject hexPrefab;

    public const int WIDTH = 50; //no. of Column in this map
    public const int HEIGHT = 60; //no. of Row in this map

    public const int ARCTICNORTH = HEIGHT - 2; //58
    public const int ARCTICSOUTH = 2;

    public const int TUNDRANORTH = HEIGHT - 6; //54
    public const int TUNDRASOUTH = 6;

    public const int GRASSNORTH = HEIGHT - 12; //48
    public const int GRASSSOUTH = 12;

    public const int PRAIRIENORTH = HEIGHT - 20; //40
    public const int PRAIRIESOUTH = 20;

    public const int SAVANNANORTH = HEIGHT - 24; //36
    public const int SAVANNASOUTH = 24;

    public const int TROPICALNORTH = HEIGHT - 30; //30
    public const int TROPICALSOUTH = 30;

    [SerializeField]
    private Hex[,] allHexes = new Hex[WIDTH, HEIGHT];
    public Hex[,] AllHexes { get { return allHexes; } }

    [SerializeField]
    private HexData[] hexData;
    public HexData[] HexData { get { return hexData; } }

    [SerializeField]
    private bool showingText;

    [SerializeField]
    private int oceanEdgeIndex;

    [SerializeField]
    private Faction playerFaction;
    public Faction PlayerFaction { get { return playerFaction; } }

    [SerializeField]
    private Faction[] factions; //England, France, Spain, Netherland, Portugal
    public Faction[] Factions { get { return factions; } }

    [SerializeField]
    private FactionData[] factionData;
    public FactionData[] FactionData { get { return factionData; } }

    [SerializeField]
    private GameObject landUnitPrefab;

    [SerializeField]
    private GameObject navalUnitPrefab;

    [SerializeField]
    private GameObject townPrefab;

    [SerializeField]
    private Unit curUnit;
    public Unit CurUnit { get { return curUnit; } set { curUnit = value; } }

    [SerializeField]
    private Unit curAiUnit;
    public Unit CurAiUnit { get { return curAiUnit; } set { curAiUnit = value; } }

    [SerializeField]
    private LandUnitData[] landUnitData;
    public LandUnitData[] LandUnitData { get { return landUnitData; } }

    [SerializeField]
    private NavalUnitData[] navalUnitData;
    public NavalUnitData[] NavalUnitData { get { return navalUnitData; } }

    public static GameManager instance;

    void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetUpFaction();
        SelectPlayerFaction();

        DetermineOcean();
        GenerateAllHexes();

        GenerateAllEuropeanShips();
        GenerateAllEuropeanExplorerUnits();

        FocusStartingUnit();
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.iKey.wasPressedThisFrame)
            ToggleHexText();

        if (Keyboard.current.tabKey.wasPressedThisFrame)
            SelectNextPlayerUnit();
    }

    private void DetermineOcean()
    {
        oceanEdgeIndex = WIDTH - Random.Range(7, 10);
        //Debug.Log($"min:{oceanEdgeIndex}");
    }

    private void GenerateAllHexes()
    {
        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                Vector3 hexPos = grid.GetCellCenterWorld(new Vector3Int(x, y));
                //Debug.Log(hexPos);

                GameObject hexObj = Instantiate(hexPrefab, hexPos, Quaternion.identity, hexParent);
                Hex hex = hexObj.GetComponent<Hex>();

                int n = Random.Range(oceanEdgeIndex - 3, oceanEdgeIndex + 4);

                if (x >= n)
                    hex.HexInit(x, y, hexPos, this, 0);//Ocean
                else
                    GenerateAllBiomes(x, y, hex, hexPos);//Land

                //Debug.Log($"{x}:{y}");
                allHexes[x, y] = hex;
            }
        }
    }

    private void GenerateBiome(int x, int y, Hex hex, Vector3 hexPos, int defaultTerrain, List<int> otherTerrain)
    {
        int n = Random.Range(1, 101);

        if (n <= 50)
            hex.HexInit(x, y, hexPos, this, defaultTerrain);//Main Biome Land
        else
        {
            int i = Random.Range(0, otherTerrain.Count);
            hex.HexInit(x, y, hexPos, this, otherTerrain[i]);//Other Biome Land
        }
    }

    private void GenerateAllBiomes(int x, int y, Hex hex, Vector3 hexPos)
    {
        int n = Random.Range(1, 101);

        //Arctic
        if ((y >= 0 && y < ARCTICSOUTH) || (y >= ARCTICNORTH && y < HEIGHT))
        {
            GenerateBiome(x, y, hex, hexPos, 8, new List<int> { 5 });//Tundra
        }

        //Tundra
        else if ((y >= ARCTICSOUTH && y < TUNDRASOUTH) || (y >= TUNDRANORTH && y < ARCTICNORTH))
        {
            GenerateBiome(x, y, hex, hexPos, 5, new List<int> { 1 });//Grassland
        }

        //Grassland
        else if ((y >= TUNDRASOUTH && y < GRASSSOUTH) || (y >= GRASSNORTH && y < TUNDRANORTH))
        {
            GenerateBiome(x, y, hex, hexPos, 1, new List<int> { 2, 5 });//Prairie, Tundra
        }

        //Prairie
        else if ((y >= GRASSSOUTH && y < PRAIRIESOUTH) || (y >= PRAIRIENORTH && y < GRASSNORTH))
        {
            GenerateBiome(x, y, hex, hexPos, 2, new List<int> { 1, 3, 4 });//Grassland, Savanna, Plain
        }

        //Savanna
        else if ((y >= PRAIRIESOUTH && y < SAVANNASOUTH) || (y >= SAVANNANORTH && y < PRAIRIENORTH))
        {
            GenerateBiome(x, y, hex, hexPos, 3, new List<int> { 1, 2, 4, 6 });//Grassland, Prairie, Plain, Desert
        }

        //Tropical
        else if ((y >= SAVANNASOUTH && y < TROPICALSOUTH) || (y >= 30 && y < SAVANNANORTH))
        {
            GenerateBiome(x, y, hex, hexPos, 4, new List<int> { 1, 7 });//Plain, Swamp
        }

        //**Special Conditions**
        //Hills
        if (n > 80)
        {
            hex.ClearForest();
            GenerateBiome(x, y, hex, hexPos, 9, new List<int> { 10 });//Mountains
        }
    }

    private void SetUpFaction()
    {
        for (int i = 0; i < factions.Length; i++)
        {
            factions[i].FactionInit(factionData[i]);
        }
    }

    public void SelectPlayerFaction()
    {
        int i = 0; //England
        playerFaction = factions[i];
    }

    private void GenerateEuropeanShip(Faction faction)
    {
        int x = WIDTH - 1; //near right edge of a map
        int y = Random.Range(0, HEIGHT);
        Hex hex = allHexes[x, y];

        GameObject obj = Instantiate(navalUnitPrefab, hex.Pos, Quaternion.identity, faction.UnitParent);
        NavalUnit ship = obj.GetComponent<NavalUnit>();

        ship.UnitInit(this, faction, navalUnitData[0]); //Caravel
        ship.SetupPosition(hex);
        faction.Units.Add(ship); //First Unit of European nations is a ship
    }

    private void GenerateAllEuropeanShips()
    {
        for (int i = 0; i < 5; i++)
        {
            GenerateEuropeanShip(factions[i]);
        }
    }

    private void GeneratePassengerUnit(Faction faction, Hex hex, int unitId, bool show, NavalUnit ship)//ship passengers
    {
        GameObject obj = Instantiate(landUnitPrefab, hex.Pos, Quaternion.identity, ship.PassengerParent.transform);
        LandUnit unit = obj.GetComponent<LandUnit>();

        unit.UnitInit(this, faction, landUnitData[unitId]);
        unit.SetupPosition(hex);

        unit.UnitStatus = UnitStatus.OnBoard;
        obj.SetActive(false);

        faction.Units.Add(unit);
        ship.Passengers.Add(unit);
    }

    private void GenerateAllEuropeanExplorerUnits()
    {
        for (int i = 0; i < 5; i++)
        {
            NavalUnit firstShip = factions[i].Units[0].gameObject.GetComponent<NavalUnit>();

            GeneratePassengerUnit(factions[i], firstShip.CurHex, 1, false, firstShip); //Veteran Soldiers
            GeneratePassengerUnit(factions[i], firstShip.CurHex, 2, false, firstShip); //Hardy Pioneers
        }
    }

    public void ShowToggleBorder(Unit unit)
    {
        if (unit.Faction == playerFaction)
            unit.ToggleBorder(true, Color.green);
        else
            unit.ToggleBorder(true, Color.red);
    }

    public void ClearToggleBorder(Unit unit)
    {
        unit.ToggleBorder(false, Color.green);
    }

    public void FocusPlayerUnit(Unit unit)
    {
        ShowToggleBorder(unit);
    }

    public bool CheckIfHexIsAdjacent(Hex centerHex, Hex targetHex)
    {
        List<Hex> adjHexes = HexCalculator.GetHexAround(allHexes, centerHex);

        return (adjHexes.Contains(targetHex)) ? true : false;
    }

    public void ClearDarkFogAroundUnit(Unit unit)
    {
        unit.CurHex.DiscoverHex();

        List<Hex> adjHexes = HexCalculator.GetHexAround(allHexes, unit.CurHex);

        //Debug.Log(adjHexes.Count);

        foreach (Hex hex in adjHexes)
        {
            hex.DiscoverHex();
        }
    }

    public void LeaveSeenFogAroundUnit(Unit unit)
    {
        unit.CurHex.SeenHex();

        List<Hex> adjHexes = HexCalculator.GetHexAround(allHexes, unit.CurHex);

        //Debug.Log(adjHexes.Count);

        foreach (Hex hex in adjHexes)
        {
            hex.SeenHex();
        }
    }

    public void ClearDarkFogAroundEveryUnit(Faction faction)
    {
        foreach (Unit unit in faction.Units)
        {
            //Debug.Log($"{unit.UnitName} discovers:");
            ClearDarkFogAroundUnit(unit);
        }
    }

    public void SelectPlayerUnit(Unit unit)
    {
        if (curUnit != null)
        {
            ClearToggleBorder(curUnit);

            if (curUnit.UnitStatus == UnitStatus.OnBoard)
                curUnit.gameObject.SetActive(false);
        }

        unit.gameObject.SetActive(true);

        curUnit = unit;
        //UpdateCanGoHex();

        FocusPlayerUnit(curUnit);
        //Debug.Log(curUnit);
    }

    private void FocusStartingUnit()
    {
        if (playerFaction.Units.Count == 0)
            return;

        //open the fog the player's own ship is sitting under, so it is visible on turn 1
        ClearDarkFogAroundEveryUnit(playerFaction);

        SelectPlayerUnit(playerFaction.Units[0]);
        CameraController.instance.MoveCamera(curUnit.transform.position);
    }

    private int FindIndexOfCurUnit()
    {
        if (playerFaction.Units.Contains(curUnit))
        {
            for (int i = 0; i < playerFaction.Units.Count; i++)
            {
                if (curUnit == playerFaction.Units[i])
                    return i;
            }
            return -1;
        }
        else
            return -1;
    }

    private void SelectNextPlayerUnit()
    {
        int i = FindIndexOfCurUnit();
        i++;

        if (i >= playerFaction.Units.Count)
            i = 0;

        SelectPlayerUnit(playerFaction.Units[i]);
        CameraController.instance.MoveCamera(curUnit.transform.position);
    }

    private void ToggleHexText()
    {
        foreach (Hex hex in allHexes)
            hex.ToggleAllBasicText(!showingText);

        showingText = !showingText;
    }
}
