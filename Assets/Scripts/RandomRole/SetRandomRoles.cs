using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using UnityEngine.EventSystems;

public class SetRandomRoles : MonoBehaviour
{
    AllRoles roles;
    List<string> randomRoles = new List<string>();
    [SerializeField] List<TextMeshProUGUI> roleTexts;
    public void Initialize(Player target)
    {
        roles = new AllRoles();
        string targetRole = (string)target.CustomProperties["role"];
        for (int i = 0; i < 3; i++)
        {
            bool added = false;
            while (!added)
            {
                string role = roles.allRoles.ElementAt(Random.Range(0, roles.allRoles.Count)).Key;
                if (!randomRoles.Contains(role) && targetRole != role)
                {
                    randomRoles.Add(role);
                    roleTexts[i].text = role;
                    roleTexts[i].color = SetRoleColor(roleTexts[i]);
                    added = true;
                }
            }
        }
        Cursor.lockState = CursorLockMode.None;
    }

    private Color SetRoleColor(TextMeshProUGUI roleText)
    {
        Side s = new Side();
        string side = s.GetSide(roleText.text);
        if (side == Side.mafia)
            return Color.red;
        else if (side == Side.town)
            return Color.green;
        else
            return Color.grey;
    }
}
