using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Role_Imam : Role
{
    //sounds
    AudioClip player_is_from_town;
    AudioClip player_is_sus;
    AudioClip imam_cursed;
    //mosque
    MosqueFieldTrigger mosqueFieldTrigger;
    //ability bools
    private bool seePlayersHeart;
    private bool useAbilityOnce = false;
    //duration
    private string R_Effect = "cursed";
    ChoosePlayerManager choosePlayer;
    void Start()
    {
        choosePlayer = CreatePlayersCanvas.Create(AbilityType.magic, "Choose a player to curse", playersStatus.alive);
        choosePlayer.OnPlayerChoosed += ChoosePlayerManager_OnPlayerChoosed;
        choosePlayer.OnCanceled += ChoosePlayerManager_OnCanceled;
        SetMoneyAndPrices(_money: 200, R_price: 150);
        SetSounds();
        SetRole(Roles.Imam);
        mosqueFieldTrigger = MosqueFieldTrigger.Instance;
        StartCoroutine(SetLocalSkillDurations());
        GameInput.Instance.OnAbility1Performed += GameInput_OnAbility1Performed;
        GameInput.Instance.OnAbility2Performed += GameInput_OnAbility2Performed;
    }

    private void GameInput_OnAbility1Performed(object sender, EventArgs e)
    {
        if (IsQ_Available())
        {
            seePlayersHeart = true;

        }
    }
    private void GameInput_OnAbility2Performed(object sender, EventArgs e)
    {
        if (IsR_Available())
        {
            PlayersCanvas.Instance.CanvasStatus(true);
        }
    }

    private void ChoosePlayerManager_OnPlayerChoosed(object sender, ChoosePlayerManager.OnPlayerChoosedEventArgs e)
    {
        ApplyCastOnChoosedPlayer(e.choosedPlayer);
    }

    private void ChoosePlayerManager_OnCanceled(object sender, System.EventArgs e)
    {
        abilitySlots.SetImageFillAmount(AbilitySlots.Image.R_AbilityCD, 0);
    }

    private void OnDestroy()
    {
        choosePlayer.OnPlayerChoosed -= ChoosePlayerManager_OnPlayerChoosed;
        choosePlayer.OnCanceled -= ChoosePlayerManager_OnCanceled;
        Destroy(choosePlayer != null ? choosePlayer.transform.parent.gameObject : null);
        GameInput.Instance.OnAbility1Performed -= GameInput_OnAbility1Performed;
        GameInput.Instance.OnAbility2Performed -= GameInput_OnAbility2Performed;
    }
    void FixedUpdate()
    {
        if (TimeStage.Instance.currentStage == Stage.ready)
            useAbilityOnce = false;
        CheckAbilityHashtable(R_Effect);
        if (TimeStage.Instance.currentStage != Stage.action)
            return;
        CheckQCast();
    }
    private void ApplyCastOnChoosedPlayer(Player pl)
    {
        pl.SetCustomProperties(new Hashtable { { R_Effect, true }, { "cursedDuration", R_Duration } });
        PlayersCanvas.Instance.CanvasStatus(false);
        audioSource.PlayOneShot(imam_cursed);
        Money -= R_Cost;
        //if choosed player is us than cast ability on ourself
        if (pl == PhotonNetwork.LocalPlayer)
            SetAbilityUsageText("You cursed yourself! Your next " + R_Duration + " seconds is will be painful..", Color.magenta);
        //if not than cast ability on choosed player
        else
            SetAbilityUsageText("You cursed " + pl.NickName.ToUpperInvariant() + "! He may die after  " + R_Duration + " seconds", Color.magenta);

        OnR_Performed();
        //if duration has ended and the choosed player is on the mafia side than he will suicide
        StartCoroutine(CheckSuicideTimer(pl));
    }
    private IEnumerator CheckSuicideTimer(Player pl)
    {
        yield return new WaitForSeconds(R_Duration);

        // check if selected player has a spell protection
        if (!DieChecker.DieCheck(pl, ProtectionProperties.SpellProtections) || TimeStage.Instance.currentStage != Stage.action)
        {
            pl.SetCustomProperties(new Hashtable { { R_Effect, false } });
        }

        if ((string)pl.CustomProperties["side"] == Side.mafia && (bool)pl.CustomProperties[R_Effect])
        {
            pl.SetCustomProperties(new Hashtable { { "alive", false } });
        }
    }
    private void CheckQCast()
    {
        if (Physics.Raycast(ray, out RaycastHit hit, playerActivateDistance) && hit.transform.tag == "Player" && mosqueFieldTrigger.isInside && !useAbilityOnce)
        {
            abilitySlots.SetImageFillAmount(AbilitySlots.Image.Q_AbilityCD, 0);
            if (seePlayersHeart)
            {
                Player pl = hit.transform.gameObject.GetComponent<PhotonView>().Owner;
                if (pl == PhotonNetwork.LocalPlayer)
                {
                    return;
                }
                string plName = pl.NickName;
                string side = "";
                //if someone change target's role than imam will see the fake role and side of target
                if (pl.CustomProperties.TryGetValue("fakeSide", out object value))
                {
                    if ((string)value != "")
                        side = (string)value;
                }
                if (side == "")
                    side = (string)pl.CustomProperties["side"];
                bool isFromTown = side == "town" ? true : false;
                if (isFromTown)
                {
                    SetAbilityUsageText(plName.ToUpperInvariant() + " is on the " + Side.town.ToUpperInvariant() + "'s side", Color.green);
                    audioSource.PlayOneShot(player_is_from_town);
                }
                else
                {
                    SetAbilityUsageText(plName.ToUpperInvariant() + " is not on the" + Side.town.ToUpperInvariant() + "'s side", Color.magenta);
                    audioSource.PlayOneShot(player_is_sus);
                }

                OnQ_Performed();
                seePlayersHeart = false;
                useAbilityOnce = true;
            }
        }
        else
        {
            abilitySlots.SetImageFillAmount(AbilitySlots.Image.Q_AbilityCD, 1);
        }
    }
    public override void SetSounds()
    {
        player_is_from_town = (AudioClip)Resources.Load("Sounds/AbilitySounds/Imam/good-person");
        player_is_sus = (AudioClip)Resources.Load("Sounds/AbilitySounds/Imam/mystery-person");
        imam_cursed = (AudioClip)Resources.Load("Sounds/AbilitySounds/Imam/imam-cursed");
    }

    public override IEnumerator SetLocalSkillDurations()
    {
        yield return TimeSystem.nightTime != 0;
        R_Duration = TimeSystem.nightTime / 3 * 60;
    }
}
