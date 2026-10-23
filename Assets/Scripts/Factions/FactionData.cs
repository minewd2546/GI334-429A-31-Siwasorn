using UnityEngine;

[CreateAssetMenu(fileName = "FactionData", menuName = "Scriptable Objects/FactionData")]
public class FactionData : ScriptableObject
{
    public Nation nation;
    public bool isEuropean;
    public Sprite flagIcon;
    public Sprite shieldIcon;
    public Sprite unitIcon;
    public Sprite townIcon;
    public int startingGold;
    public string[] townNames; //historical colony names, used in founding order
}
