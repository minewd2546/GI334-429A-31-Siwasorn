using System.Collections.Generic;

public enum HexDirection
{
    TopLeft,
    TopRight,
    Right,
    BottomRight,
    BottomLeft,
    Left
}

public static class HexCalculator
{
    public static Hex FindHexByDir(Hex center, HexDirection dir, Hex[,] hexes)
    {
        int width = GameManager.WIDTH;
        int height = GameManager.HEIGHT;

        Hex hexResult = null;

        //Debug.Log($"1. Center is: {center.X}, {center.Y} finding {dir}");

        int x = center.X;
        int y = center.Y;

        switch (dir)
        {
            case HexDirection.TopLeft:
                if ((y % 2) == 0)
                    x -= 1;
                y += 1;
                break;
            case HexDirection.TopRight:
                if ((y % 2) != 0)
                    x += 1;
                y += 1;
                break;
            case HexDirection.Right:
                x += 1;
                break;
            case HexDirection.BottomRight:
                if ((y % 2) != 0)
                    x += 1;
                y -= 1;
                break;
            case HexDirection.BottomLeft:
                if ((y % 2) == 0)
                    x -= 1;
                y -= 1;
                break;
            case HexDirection.Left:
                x -= 1;
                break;
        }
        //Debug.Log($"2.After Cal: {x}, {y}");

        if ((x < 0 || x >= width) || (y < 0 || y >= height))
            return null;

        hexResult = hexes[x, y];

        //Debug.Log($"3.Result: {hexResult.X}, {hexResult.Y}");

        return hexResult;
    }

    public static List<Hex> GetHexAround(Hex[,] hexes, Hex center)
    {
        List<Hex> hexList = new List<Hex>();

        for (int i = 0; i < 6; i++)
        {
            Hex hex = FindHexByDir(center, (HexDirection)i, hexes);

            if (hex != null)
                hexList.Add(hex);
        }
        return hexList;
    }

    private static List<HexDirection> GetHexDirectionList(Hex currentHex, Hex destHex)
    {
        //Debug.Log("Get Direction");
        List<HexDirection> directions = new List<HexDirection>();
        //TopLeft
        if (destHex.X <= currentHex.X && destHex.Y > currentHex.Y)
        {
            //Debug.Log("TopLeft");
            directions.Add(HexDirection.TopLeft);
            directions.Add(HexDirection.TopRight);
            directions.Add(HexDirection.Left);
            return directions;
        }

        //TopRight
        if (destHex.X > currentHex.X && destHex.Y > currentHex.Y)
        {
            //Debug.Log("TopRight");
            directions.Add(HexDirection.TopRight);
            directions.Add(HexDirection.TopLeft);
            directions.Add(HexDirection.Right);
            return directions;
        }

        //Right
        if (destHex.X > currentHex.X && destHex.Y == currentHex.Y)
        {
            //Debug.Log("Right");
            directions.Add(HexDirection.Right);
            directions.Add(HexDirection.TopRight);
            directions.Add(HexDirection.BottomRight);
            return directions;
        }

        //BottomRight
        if (destHex.X >= currentHex.X && destHex.Y < currentHex.Y)
        {
            //Debug.Log("BottomRight");
            directions.Add(HexDirection.BottomRight);
            directions.Add(HexDirection.Right);
            directions.Add(HexDirection.BottomLeft);
            return directions;
        }

        //BottomLeft
        if (destHex.X <= currentHex.X && destHex.Y < currentHex.Y)
        {
            //Debug.Log("BottomLeft");
            directions.Add(HexDirection.BottomLeft);
            directions.Add(HexDirection.BottomRight);
            directions.Add(HexDirection.Left);
            return directions;
        }

        //Left
        if (destHex.X < currentHex.X && destHex.Y == currentHex.Y)
        {
            //Debug.Log("Left");
            directions.Add(HexDirection.Left);
            directions.Add(HexDirection.BottomRight);
            directions.Add(HexDirection.BottomLeft);
            return directions;
        }
        return null;
    }

    public static Hex FindNextHexToGo(Unit unit, Hex currentHex, Hex destinationHex, Hex[,] allHexes)
    {
        if (destinationHex == null)
            return null;

        if (destinationHex == currentHex)
            return null;

        List<HexDirection> directions = GetHexDirectionList(currentHex, destinationHex);

        List<Hex> hexesAround = new List<Hex>();

        foreach (HexDirection dir in directions)
        {
            Hex hex = FindHexByDir(currentHex, dir, allHexes);

            if (hex == null) //off the map edge
                continue;

            if (unit.UnitType == UnitType.Naval && hex.HexType == HexType.Ocean)
                hexesAround.Add(hex);

            if (unit.UnitType == UnitType.Land && hex.HexType != HexType.Ocean)
                hexesAround.Add(hex);
        }
        //Debug.Log($"CurHex:{currentHex.X},{currentHex.Y}");

        foreach (Hex hex in hexesAround)
        {
            if (hex.MoveCost < unit.MovePoint)
                return hex;
        }

        return null;
    }

    public static Faction CheckNeverMetFaction(Hex[,] hexes, Unit playerUnit)
    {
        List<Hex> aroundHexes = new List<Hex>();
        aroundHexes = GetHexAround(hexes, playerUnit.CurHex);

        foreach (Hex hex in aroundHexes)
        {
            foreach (Unit other in hex.UnitsInHex)
            {
                if (other.Faction != playerUnit.Faction)
                {
                    if (other.Faction.MetByPlayer == false)
                    {
                        return other.Faction;
                    }
                }
            }
        }
        return null;
    }

    public static bool CheckIfHexAroundHasTown(Hex[,] hexes, Hex center)
    {
        for (int i = 0; i < 6; i++)
        {
            Hex hex = FindHexByDir(center, (HexDirection)i, hexes);

            if (hex == null)
                continue;

            if (hex.HasTown)
                return true;
        }
        return false;
    }

    public static Hex[] GetHexAroundToArray(Hex[,] hexes, Hex center)
    {
        Hex[] hexArray = new Hex[6];

        for (int i = 0; i < 6; i++)
        {
            Hex hex = FindHexByDir(center, (HexDirection)i, hexes);
            hexArray[i] = hex;
        }
        return hexArray;
    }
}
