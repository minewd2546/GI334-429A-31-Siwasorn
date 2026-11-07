using UnityEngine;

[System.Serializable]
public class ShipInTransit
{
    [SerializeField]
    private NavalUnit ship;
    public NavalUnit Ship { get { return ship; } set { ship = value; } }

    [SerializeField]
    private int turnLeft;
    public int TurnLeft { get { return turnLeft; } set { turnLeft = value; } }

    public ShipInTransit(NavalUnit s, int turn)
    {
        ship = s;
        turnLeft = turn;
    }
}
