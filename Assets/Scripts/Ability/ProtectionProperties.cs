using UnityEngine;

public enum Property
{
    alive,

}
public enum Protection
{
    sharpShield,
    luckShield,
    luckAegis,

}
public class ProtectionProperties : MonoBehaviour
{

    public static string[] physicalProtections =
    {
        Protection.sharpShield.ToString(),  //watchman_R
        Protection.luckShield.ToString(),    //lucky_passive/Q

    };
    public static string[] SpellProtections =
    {
        Protection.luckAegis.ToString(),    //lucky_Q
    };
    public static string[] spells =
    {
        "cursed" //imam_R
    };

}
