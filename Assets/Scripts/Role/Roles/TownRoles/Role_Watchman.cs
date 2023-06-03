using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System;
using System.Collections;
using Photon.Realtime;

public class Role_Watchman : Role
{
    Hashtable hash;
    //Remaining Times
    private float remainingTime = 0f;
    //ability sounds
    private AudioClip WatchmanQSound;
    private AudioClip WatchmanRSound;
    ChoosePlayerManager choosePlayer;
    private void Start()
    {
        choosePlayer = CreatePlayersCanvas.Create(AbilityType.shield, "Choose a player to give sharp shield with you", playersStatus.alive);
        choosePlayer.OnPlayerChoosed += ChoosePlayerManager_OnPlayerChoosed;
        choosePlayer.OnCanceled += ChoosePlayerManager_OnCanceled;
        SetMoneyAndPrices(_money: 150, R_price: 125);
        SetSounds();
        SetRole(Roles.Watchman);
        hash = new Hashtable();
        hash.Add("sharpShield", false);
        StartCoroutine(SetLocalSkillDurations());
        GameInput.Instance.OnAbility1Performed += GameInput_OnAbility1Performed;
        GameInput.Instance.OnAbility2Performed += GameInput_OnAbility2Performed;
    }

    private void GameInput_OnAbility1Performed(object sender, EventArgs e)
    {
        if (IsQ_Available())
        {
            SetAbilityUsageText("You have a sharp shield for " + Math.Round(Q_Duration, 1) + " seconds. (Sharp Shield: Ready against all physical attacks. If enemy attacks, he dies.)", Color.blue);
            SetAbilityIcons("WatchmanQ", Q_Duration, Color.blue);
            audioSource.PlayOneShot(WatchmanQSound);
            hash["sharpShield"] = true;
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            remainingTime += Q_Duration;
            OnQ_Performed();
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

    private void ChoosePlayerManager_OnCanceled(object sender, EventArgs e)
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
    private void FixedUpdate()
    {
        CheckSharpShield();
    }

    private void CheckSharpShield()
    {
        if (remainingTime > 0)
        {
            remainingTime -= Time.fixedDeltaTime;
        }
        if (remainingTime < 0)
        {
            remainingTime = 0;
            hash["sharpShield"] = false;
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }
    public override void SetSounds()
    {
        WatchmanRSound = (AudioClip)Resources.Load("Sounds/AbilitySounds/Watchman/WatchmanR_sharpShield");
        WatchmanQSound = (AudioClip)Resources.Load("Sounds/AbilitySounds/Watchman/WatchmanQ");
    }

    private void ApplyCastOnChoosedPlayer(Player pl)
    {

        //if choosed player is us than cast ability on ourself
        if (pl == PhotonNetwork.LocalPlayer)
        {
            //shielded local player for double time
            SetAbilityIcons("WatchmanQ", R_Duration, Color.blue);
            SetAbilityUsageText("You have a sharp shield for " + Math.Round(R_Duration * 2, 1) + " seconds (doubled).", Color.blue);
            audioSource.PlayOneShot(WatchmanQSound);
            hash["sharpShield"] = true;
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            remainingTime += R_Duration * 2;
        }
        //if not than cast abiltiy on choosed player
        else
        {
            //shielded local player
            remainingTime += Q_Duration;
            audioSource.PlayOneShot(WatchmanQSound);
            remainingTime += R_Duration;
            hash["sharpShield"] = true;
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

            //shielded selected player
            pl.SetCustomProperties(new Hashtable { { "sharpShield", true }, { "sharpShieldDuration", R_Duration } });
            StartCoroutine(CheckRTimerForChoosedPlayer(pl));
        }
        Money -= R_Cost;
        //close canvas after choose
        PlayersCanvas.Instance.CanvasStatus(false);
        OnR_Performed();
    }

    private IEnumerator CheckRTimerForChoosedPlayer(Player pl)
    {
        yield return new WaitForSeconds(R_Duration);

        pl.SetCustomProperties(new Hashtable { { "sharpShield", false } });
    }


    public override IEnumerator SetLocalSkillDurations()
    {
        yield return TimeSystem.nightTime != 0;
        Q_Duration = TimeSystem.nightTime / 8 * 60;
        R_Duration = TimeSystem.nightTime / 4 * 60;
    }
}
