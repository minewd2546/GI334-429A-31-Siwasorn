using UnityEngine;
using UnityEngine.InputSystem;

public class Town : MonoBehaviour
{
    [SerializeField]
    protected string townName;
    public string TownName { get { return townName; } set { townName = value; } }

    [SerializeField]
    protected Faction faction;
    public Faction Faction { get { return faction; } }

    [SerializeField]
    protected int visualRange;
    public int VisualRange { get { return visualRange; } }

    [SerializeField]
    private Vector2 curPos;
    public Vector2 CurPos { get { return curPos; } set { curPos = value; } }

    [SerializeField]
    protected Hex curHex;
    public Hex CurHex { get { return curHex; } set { curHex = value; } }


    [SerializeField]
    private int[] warehouse = new int[16];
    public int[] Warehouse { get { return warehouse; } set { warehouse = value; } }

    [SerializeField]
    private int[] totalYieldThisTurn = new int[16]; //no. of all resource production this turn
    public int[] TotalYieldThisTurn { get { return totalYieldThisTurn; } set { totalYieldThisTurn = value; } }

    [SerializeField]
    private int crossNum; //no. of crosses produced in this turn
    public int CrossNum { get { return crossNum; } set { crossNum = value; } }

    [SerializeField]
    private int bellNum; //no. of bells produced in this turn
    public int BellNum { get { return bellNum; } set { bellNum = value; } }

    [Header("Town")]
    [SerializeField]
    protected SpriteRenderer townSprite;
    public SpriteRenderer TownSprite { get { return townSprite; } }

    [Header("Border")]
    [SerializeField]
    protected SpriteRenderer borderSprite;

    [Header("Flag")]
    [SerializeField]
    protected SpriteRenderer flagSprite;

    [SerializeField]
    protected GameManager gameManager;

    private void OnMouseDown()
    {
        if (faction == gameManager.PlayerFaction)
            gameManager.SetupCurrentTown(this);
    }

    // the manual uses Input.GetMouseButtonDown(1); this project is on the new Input System
    public void OnMouseOver()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            //Debug.Log($"To Move Hex:{curHex.X}, {curHex.Y}");
            if (gameManager.CheckIfHexIsAdjacent(gameManager.CurUnit.CurHex, curHex))
            {
                if (faction == gameManager.PlayerFaction)//same side unit
                {
                    //Debug.Log("True");
                    gameManager.CurUnit.PrepareMoveToHex(curHex);
                }
                else//diff side unit
                {
                    //Debug.Log($"{gameManager.CurUnit} Attacks {townName}");
                }
            }
        }
    }

    public void AccumulateToWarehouse()
    {
        for (int i = 0; i < 16; i++)
        {
            warehouse[i] += totalYieldThisTurn[i];
        }
    }

    public void TownInit(GameManager gameMgr, Faction fact)
    {
        gameManager = gameMgr;
        faction = fact;
        flagSprite.sprite = fact.FlagIcon;
        townSprite.sprite = fact.TownIcon;
    }
}
