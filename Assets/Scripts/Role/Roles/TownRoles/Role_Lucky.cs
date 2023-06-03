using System.Collections;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun;
using System;
/*
Lucky(Þanslý) - Town:

    Pasif: Gece boyunca %25 þansla tüm olumsuz etkilerden sýyrýlabilir (Gecenin her 1/12 sinde þansý kontrol edilir).

    Q: Gecenin 3 te 1 i boyunca tüm olumsuz etkilerden kurtulma þansýný 2 ye katlar.

    R(200 gold): Bu skillin aktif olduðu gecenin sabahýndaki oylamada idam edilmek için seçilirse þanslý olduðunu kanýtlar. Giyotin arýza yapar ve ölmez.
*/
public class Role_Lucky : Role
{
    private bool doubleLuck;
    private float Q_timer;
    private bool isGuillotineBroken;
    private float checkLuckTimer = TimeSystem.nightTime / 12 * 60;
    void Start()
    {
        SetMoneyAndPrices(_money: 200, R_price: 150);
        SetSounds();
        SetRole(Roles.Lucky);
        StartCoroutine(SetLocalSkillDurations());
        GameInput.Instance.OnAbility1Performed += GameInput_OnAbility1Performed;
        GameInput.Instance.OnAbility2Performed += GameInput_OnAbility2Performed;
    }

    private void GameInput_OnAbility1Performed(object sender, EventArgs e)
    {
        if (IsQ_Available())
        {
            doubleLuck = true;
            checkLuckTimer = 0;
            OnQ_Performed();
        }
    }
    private void GameInput_OnAbility2Performed(object sender, EventArgs e)
    {
        if (IsR_Available())
        {
            isGuillotineBroken = true;
            GuillotineState(true);
            OnR_Performed();
        }
    }
    private void OnDestroy()
    {
        GameInput.Instance.OnAbility1Performed -= GameInput_OnAbility1Performed;
        GameInput.Instance.OnAbility2Performed -= GameInput_OnAbility2Performed;
    }
    void FixedUpdate()
    {
        CheckLuckProtections();
        CheckGuillotine();
    }

    private void CheckGuillotine()
    {
        if (isGuillotineBroken && TimeStage.Instance.currentStage == Stage.ready)
        {
            isGuillotineBroken = false;
            GuillotineState(false);
        }
    }

    private void GuillotineState(bool state)
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "brokenGuillotine", state } });
    }

    private void CheckLuckProtections()
    {
        if (doubleLuck)
        {
            ShieldState(CheckShieldChance(0, 2));

            if (Q_timer >= Q_Duration)
            {
                doubleLuck = false;
                Q_timer = 0;
            }
            Q_timer += Time.fixedDeltaTime;
        }
        else
        {
            ShieldState(CheckShieldChance(0, 4));
        }
    }

    private void ShieldState(bool state)
    {
        checkLuckTimer -= Time.fixedDeltaTime;
        if (checkLuckTimer > 0)
            return;

        checkLuckTimer = TimeSystem.nightTime / 12 * 60;
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "luckShield", state }, { "luckAegis", state } });
    }

    private bool CheckShieldChance(int expectedInt, int percent)
    {
        int res = UnityEngine.Random.Range(0, percent);
        if (res == expectedInt)
            return true;
        return false;
    }

    public override IEnumerator SetLocalSkillDurations()
    {
        yield return TimeSystem.nightTime != 0;
        Q_Duration = TimeSystem.nightTime / 3 * 60;
    }

    public override void SetSounds()
    {
    }
}
