using Photon.Realtime;
using UnityEngine;

public class DieChecker : MonoBehaviour
{
    public static bool DieCheck(Player target, string[] protections)
    {
        foreach (string key in protections)
            if (target.CustomProperties.ContainsKey(key))
            {
                if ((bool)target.CustomProperties[key])
                    return false;
            }
        return true;
    }
    public static Player DieCheckWithPlayers(Player killer, Player target, string[] protections)
    {
        foreach (string key in protections)
            if (target.CustomProperties.ContainsKey(key))
            {
                if (key == "sharpShield")
                {
                    return CheckDieWithSharpShield(killer, target);
                }
            }
        return target;
    }


    private static Player CheckDieWithSharpShield(Player killer, Player target)
    {
        bool targetSafety = false;
        bool killerSafety = false;
        if ((bool)target.CustomProperties["sharpShield"])
        {
            targetSafety = true;
            if (killer.CustomProperties.ContainsKey("sharpShield"))
            {
                if ((bool)killer.CustomProperties["sharpShield"])
                    killerSafety = true;
            }
        }
        if (!targetSafety)
        {
            return target;

        }
        else if (!killerSafety)
        {
            return killer;
        }
        else
        {
            return null;
        }
    }
}
