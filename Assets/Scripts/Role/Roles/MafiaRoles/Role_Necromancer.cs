using Photon.Realtime;
using System;
using System.Collections;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Role_Necromancer : Role
{
    //duration
    private float Q_duration;

    ChoosePlayerManager choosePlayer;
    void Start()
    {
        choosePlayer = CreatePlayersCanvas.Create(AbilityType.heal, "Choose a player to revive", playersStatus.dead);
        choosePlayer.OnPlayerChoosed += ChoosePlayerManager_OnPlayerChoosed;
        choosePlayer.OnCanceled += ChoosePlayerManager_OnCanceled;
        SetMoneyAndPrices(_money: 250, R_price: 200);
        SetSounds();
        SetRole(Roles.Necromancer);
        StartCoroutine(SetLocalSkillDurations());
        GameInput.Instance.OnAbility1Performed += GameInput_OnAbility1Performed;
        GameInput.Instance.OnAbility2Performed += GameInput_OnAbility2Performed;
    }

    private void GameInput_OnAbility1Performed(object sender, EventArgs e)
    {
        if (IsQ_Available())
        {
            //eklenecek

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
    private void ApplyCastOnChoosedPlayer(Player pl)
    {
        pl.SetCustomProperties(new Hashtable { { "alive", true } });
        PlayersCanvas.Instance.CanvasStatus(false);
        //audioSource.PlayOneShot(song);
        Money -= R_Cost;
        SetAbilityUsageText("You revived " + pl.NickName + " !", Color.green);
    }

    public override IEnumerator SetLocalSkillDurations()
    {
        yield return TimeSystem.nightTime != 0;
        Q_duration = TimeSystem.nightTime / 3 * 60;
    }

    public override void SetSounds()
    {

    }


}
