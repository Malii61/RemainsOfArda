using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class MoneyManager : MonoBehaviourPunCallbacks
{
    public static MoneyManager Instance { get; private set; }
    int currentBalance;
    [SerializeField] private TextMeshProUGUI balanceChangeText;
    [SerializeField] private TextMeshProUGUI moneyText;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (targetPlayer == PhotonNetwork.LocalPlayer && changedProps.ContainsKey("alive"))
        {
            if ((bool)changedProps["alive"])
            {
                if (changedProps.ContainsKey("money"))
                    currentBalance = (int)changedProps["money"];
            }
        }
        else if (targetPlayer == PhotonNetwork.LocalPlayer && changedProps.ContainsKey("money"))
        {
            ShowChange((int)changedProps["money"]);
        }
    }

    private void ShowChange(int newBalance)
    {
        int change = newBalance - currentBalance;
        Color c;
        c = change < 0 ? Color.red : Color.green;
        if (change != 0)
        {
            StartCoroutine(SetText(change, c));
        }
        currentBalance = newBalance;
    }

    private IEnumerator SetText(int change, Color c)
    {
        balanceChangeText.enabled = true;
        balanceChangeText.color = c;
        string ch = change > 0 ? "+" + change : change.ToString();
        balanceChangeText.text = ch;
        yield return new WaitForSeconds(3);
        balanceChangeText.enabled = false;
    }
    public string GetMoneyText()
    {
        return moneyText.text;
    }
    public void SetMoneyText(string _money)
    {
        moneyText.text = _money;
    }
}
