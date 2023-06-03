using UnityEngine;
using Photon.Pun;
using System.Collections;
using UnityEngine.EventSystems;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public abstract class Role : MonoBehaviourPunCallbacks
{
    #region init
    internal AbilitySlots abilitySlots;

    //ray
    internal const int playerActivateDistance = 5;
    internal Camera cam;
    internal Ray ray;
    internal PhotonView PV;
    internal bool isQ_Usable = false;
    internal bool isR_Usable = false;
    private string role;
    private int money;
    private int R_Price;
    private bool hashIsCheckable;
    internal bool isNight;
    private float r_duration;
    private float q_duration;
    internal Animator animator;
    internal AudioSource audioSource;
    #endregion
    private void Awake()
    {
        //initializing objects
        audioSource = GetComponent<PlayerController>().GetAudioSource();
        cam = GetComponent<PlayerController>().GetCamera();
        PV = GetComponent<PhotonView>();
        animator = GetComponent<PlayerController>().GetAnimator();
        abilitySlots = AbilitySlots.Instance;
        TimeStage.Instance.OnMorning += TimeStage_OnMorning;
        TimeStage.Instance.OnNight += TimeStage_OnNight;
    }
    private void TimeStage_OnMorning(object sender, System.EventArgs e)
    {
        isNight = false;
        isQ_Usable = false;
        isR_Usable = false;
        abilitySlots.SetImageFillAmount(AbilitySlots.Image.Q_AbilityCD, 1);
        abilitySlots.SetImageFillAmount(AbilitySlots.Image.R_AbilityCD, 1);
    }
    private void TimeStage_OnNight(object sender, System.EventArgs e)
    {
        isNight = true;
        isQ_Usable = true;
        isR_Usable = true;
        abilitySlots.SetImageFillAmount(AbilitySlots.Image.Q_AbilityCD, 0);
        abilitySlots.SetImageFillAmount(AbilitySlots.Image.R_AbilityCD, 0);
    }

    void Update()
    {
        ray.origin = cam.transform.position;
        ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        CheckAbilityDurations();
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        CheckPropertyChanges(targetPlayer, changedProps);
    }
    internal void CheckPropertyChanges(Player targetPlayer, Hashtable changedProps)
    {
        if (Is_Me(targetPlayer))
        {
            if (changedProps.ContainsKey("money"))
            {
                var v = (int)changedProps["money"];
                if (Money != v) Money = v;
            }
            if (changedProps.ContainsKey("r_duration"))
            {
                var v = (float)changedProps["r_duration"];
                if (R_Duration != v) R_Duration = v;
            }
            if (changedProps.ContainsKey("q_duration"))
            {
                var v = (float)changedProps["q_duration"];
                if (Q_Duration != v) Q_Duration = v;
            }
        }
    }
    private bool Is_Me(Player p)
    {
        if (p == PhotonNetwork.LocalPlayer && PV.IsMine)
            return true;
        return false;
    }
    public int R_Cost
    {
        get { return R_Price; }
        set
        {
            R_Price = value;
            abilitySlots.SetR_CostDisplayText(value);
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "r_cost", value } });
        }
    }
    public float Q_Duration
    {
        get { return q_duration; }
        set
        {
            q_duration = value;
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "q_duration", value } });
        }
    }
    public float R_Duration
    {
        get { return r_duration; }
        set
        {
            r_duration = value;
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "r_duration", value } });
        }
    }
    public int Money
    {
        get { return money; }
        set
        {
            if (money == value)
                return;
            money = value;
            MoneyManager.Instance.SetMoneyText(value.ToString());
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "money", value } });
        }
    }
    internal bool IsQ_Available()
    {
        if (!EventSystem.current.currentSelectedGameObject && isQ_Usable)
            return true;
        return false;
    }
    internal bool IsR_Available()
    {
        if (!EventSystem.current.currentSelectedGameObject && isR_Usable && money >= R_Price)
            return true;
        return false;
    }
    internal void OnQ_Performed()
    {
        isQ_Usable = false;
        abilitySlots.SetImageFillAmount(AbilitySlots.Image.Q_AbilityCD, 1);
    }
    internal void OnR_Performed()
    {
        isR_Usable = false;
        abilitySlots.SetImageFillAmount(AbilitySlots.Image.R_AbilityCD, 1);
    }
    public void SetRole(Roles _role)
    {
        role = _role.ToString();
        abilitySlots.SetAbilities(role);
        RoleTextEditor.Instance.SetRoleText(role);
        if ((string)PhotonNetwork.LocalPlayer.CustomProperties["side"] == Side.mafia)
        {
            ChatChannel.Instance.SubOrUnsubChannel(Channel.mafia, SubType.subscribe);
        }
    }
    internal void SetMoneyAndPrices(int _money, int R_price)
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("money", out object money))
            Money = (int)money;
        else
            Money = _money;
        R_Cost = R_price;
    }
    internal void SetAbilityIcons(string skill, float duration, Color skillTypeColor)
    {
        AbilityDurationsUI.Instance.SetAbilityIcons(skill, duration, skillTypeColor);
    }
    private void RemoveAbilityIcons(string skill)
    {
        AbilityDurationsUI.Instance.RemoveAbilityIcons(skill);
    }
    private void CheckAbilityDurations()
    {
        AbilityDurationsUI.Instance.CheckAbilityDurations();
    }
    internal void SetAbilityUsageText(string text, Color c)
    {
        AbilityUsageText.Instance.SetAbilityUsageText(text, c);
        ChatMessaging.Instance.AddLogText(text, c);
    }

    internal void CheckAbilityHashtable(string skill)
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(skill, out object b))
        {
            if ((bool)b)
            {
                hashIsCheckable = true;
            }
            else
            {
                RemoveAbilityIcons(skill);
                hashIsCheckable = false;
            }
        }
    }

    public abstract void SetSounds();
    public abstract IEnumerator SetLocalSkillDurations();

}
