using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
public class Role_Belladonna : Role
{
    /*Belladonna - other:

        Pasif 1: Geceleri kullanabileceði üfürük silahý vardýr. Belladonna'nýn fýrlattýðý zehirli dart, isabet ettiði oyuncuyu büyü korumasý olup olmamasý farketmeksizin zehirler. Zehri aktif etmediði sürece seçilen oyuncuya bir þey olmaz.
        Pasif 2: Belladonna herkesi öldürürse kazanýr.

        Q: Zehirlenmiþ tüm oyuncularý ve oyuncularýn o anda büyü korumalarýnýn olup olmadýðý bilgisini görür.

        R(x gold -> x: zehirlenmiþ oyuncu sayýsý çarpý 75): Býrakýlan tüm zehirleri aktif eder ve üzerinde zehir olan korumasýz oyuncular ölür. 
    */

    //Adding blowgun to scene
    GameObject blowgun;

    private bool zooming;
    private bool x;

    void Start()
    {
        GameInput.Instance.OnAttack += GameInput_OnShoot;
        SetSounds();
        SetRole(Roles.Belladonna);
        SetMoneyAndPrices(_money: 300, R_price: 100);

        //set animation override
        AnimatorOverrider.Instance.SetAnimatorOverride(AnimatorOverrider.overrides.blowgun);

        //initialize variables
        Transform blowgunHandPoint = AnimationRiggingManager.Instance.blowgunHandPoint;
        blowgun = PhotonNetwork.Instantiate("ItemPrefabs/Guns/Blowgun", blowgunHandPoint.transform.position, blowgunHandPoint.transform.rotation, 0, new object[] { PV.ViewID });
        blowgun.transform.parent = blowgunHandPoint;
        StartCoroutine(SetLocalSkillDurations());

        GameInput.Instance.GetInputActions().Interactions.Ability1.started += Ability1_started;
        GameInput.Instance.GetInputActions().Interactions.Ability1.canceled += Ability1_canceled;
        GameInput.Instance.OnAbility2Performed += GameInput_OnAbility2Performed;
        TimeStage.Instance.OnMorning += TimeStage_OnMorning;
        TimeStage.Instance.OnNight += TimeStage_OnNight;

    }



    private void TimeStage_OnMorning(object sender, EventArgs e)
    {
        RemoveBlowgunFromInventory();
    }


    private void TimeStage_OnNight(object sender, EventArgs e)
    {
        AddBlowgunToInventory();
    }


    private void GameInput_OnShoot(object sender, EventArgs e)
    {
        Shoot();
    }

    private void Ability1_started(InputAction.CallbackContext obj)
    {
        if (IsQ_Available())
        {
            ShowPoisonedPlayers();
        }
        //open canvas
    }

    private void Ability1_canceled(InputAction.CallbackContext obj)
    {
        HidePoisonedPlayers();
    }

    private void GameInput_OnAbility2Performed(object sender, EventArgs e)
    {
        //Zehirlenenleri öldür.
        OnR_Performed();
    }
    private void OnDestroy()
    {
        GameInput.Instance.OnAttack -= GameInput_OnShoot;
        GameInput.Instance.GetInputActions().Interactions.Ability1.started -= Ability1_started;
        GameInput.Instance.GetInputActions().Interactions.Ability1.canceled -= Ability1_canceled;
        GameInput.Instance.OnAbility2Performed += GameInput_OnAbility2Performed;
        TimeStage.Instance.OnMorning -= TimeStage_OnMorning;
        TimeStage.Instance.OnNight -= TimeStage_OnNight;
    }
    private void LateUpdate()
    {
        if (!x)
        {
            ItemManager.Instance.AddItem(blowgun.GetComponent<Blowgun>());
            x = true;
        }
        Zoom();
    }
    private void Zoom()
    {
        if (GameInput.Instance.GetInputActions().Player.Zoom.IsPressed()
            && ItemManager.Instance.Item() == blowgun.GetComponent<Blowgun>()
            && !EventSystem.current.IsPointerOverGameObject()
            && !Job.IsJobCanvasActive)
        {
            animator.SetBool("isZooming", true);
            AnimationRiggingManager.Instance.SetRigWeight(AnimationRiggingManager.RigEnum.blowgunAim, 1);
            zooming = true;
        }
        else if (zooming)
        {
            animator.SetBool("isZooming", false);
            AnimationRiggingManager.Instance.SetRigWeight(AnimationRiggingManager.RigEnum.blowgunAim, 0);
            zooming = false;
        }
    }


    private void AddBlowgunToInventory()
    {
        ItemManager.Instance.AddItem(blowgun.GetComponent<Blowgun>());
    }
    private void RemoveBlowgunFromInventory()
    {
        ItemManager.Instance.RemoveItem(blowgun.GetComponent<Blowgun>());
    }
    private void Shoot()
    {
        if (zooming)
        {
            animator.SetBool("isAttacking", true);
            blowgun.GetComponent<Blowgun>().Use();
        }
    }

    private void ShowPoisonedPlayers()
    {
        foreach (Player pl in PhotonNetwork.PlayerList)
        {
            if (pl.CustomProperties.TryGetValue("dart_poison", out object value))
            {
                if ((bool)value)
                    Debug.Log(pl.NickName + " zehirlenmiþ");
            }
        }
    }
    private void HidePoisonedPlayers()
    {
        throw new NotImplementedException();
    }

    public override IEnumerator SetLocalSkillDurations()
    {
        yield return TimeSystem.nightTime != 0;
    }

    public override void SetSounds()
    {
    }


}
