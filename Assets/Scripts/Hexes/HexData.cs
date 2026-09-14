using UnityEngine;

[CreateAssetMenu(fileName = "HexData", menuName = "Scriptable Objects/HexData")]
public class HexData : ScriptableObject
{
    public string hexName;
    public HexType type;
    public Sprite[] terrainSprites;
    public Sprite[] forestSprites;
    //0-food, 1-sugar, 2-tobacco 3-cotton 4-fur 5-lumber 6-ore 7-silver
    public int[] resourceYield = new int[8];
    public int moveCost;
}
