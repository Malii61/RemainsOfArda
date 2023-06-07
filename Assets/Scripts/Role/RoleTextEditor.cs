using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class RoleTextEditor : MonoBehaviour
{
    public static RoleTextEditor Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI roleText;
    
    void Awake()
    {
            if (Instance == null)
                Instance = this;
    }
    public void SetRoleText(string _roleText)
    {
        roleText.text = _roleText;
        SetRoleTextColor();
    }
    private void SetRoleTextColor()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("side", out object side))
        {
            if (side.ToString() == Side.mafia)
            {
                roleText.color = Color.red;
            }
            else if (side.ToString() == Side.town)
            {
                roleText.color = Color.green;
            }
            else if (side.ToString() == Side.other)
            {
                Color c = new Color(0.8f, 0.6f, 0.0f);
                roleText.color = c;
            }
        }
    }
}
