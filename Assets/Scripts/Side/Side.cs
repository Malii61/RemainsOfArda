using UnityEngine;
public class Side : ScriptableObject
{
    //Get side of role 
    public static string town = "town";
    public static string mafia = "mafia";
    public static string other = "other";
    internal string GetSide(string role)
    {
        AllRoles r = new AllRoles();
        if (r.mafiaRoles.ContainsKey(role))
            return mafia;
        else if (r.townRoles.ContainsKey(role))
            return town;
        else
            return other;
    }
    
    //internal string GetRole(Dictionary<string, string> side, Color c)
     //{
     //    foreach (var role in side)
     //    {
     //        if (roleText.text == role.Key)
     //        {
     //            roleText.color = c;
     //            break;
     //        }
     //    }
     //}

    //internal bool IsOnSide(string role, string side)
    //{

    //}


}
