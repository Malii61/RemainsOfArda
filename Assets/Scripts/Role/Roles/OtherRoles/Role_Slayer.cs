using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Hashtable = ExitGames.Client.Photon.Hashtable;

/* Slayer (Katil) - Other:

    Pasif: Geceleri kullanabileceði bir býçaðý vardýr. Birini katlettiðinde hareket hýzý kazanýr.
         Birini býçakladýðýnda býçak ölen kiþide kalýr. Öldürürse Q yu tekrar kullanabilir.

    Q: Gecenin 4 te 1 i boyunca görünmez olur. Saldýrmak istediði zaman yetenek iptal olur.

    R(150 gold): Gecenin 3 te 1 i boyunca seçilen oyuncunun nickiyle Slayer'in nicki yer deðiþtirir.
*/
public class Role_Slayer : Role
{
    //Adding knife to scene
    GameObject knife;
    private float invisibleTimerMax;
    private float replaceNicknameTimerMax;
    ChoosePlayerManager choosePlayer;
    AnimationClip stabbingAnim;
    AnimatorOverrider animOverrider;
    //private float
    private void Start()
    {
        choosePlayer = CreatePlayersCanvas.Create(AbilityType.magic, "Choose a player to replace your nickname", playersStatus.alive);
        choosePlayer.OnPlayerChoosed += ChoosePlayerManager_OnPlayerChoosed;
        choosePlayer.OnCanceled += ChoosePlayerManager_OnCanceled;
        GameInput.Instance.OnAttack += GameInput_OnAttack;
        //init role
        SetSounds();
        SetRole(Roles.Slayer);
        SetMoneyAndPrices(_money: 200, R_price: 150);
        //set animation override
        animOverrider = AnimatorOverrider.Instance;
        animOverrider.SetAnimatorOverride(AnimatorOverrider.overrides.knife);
        stabbingAnim = animOverrider.GetAnimation(animOverrider.GetCurrentAnimatorOverride(), AnimatorOverrider.animName.Stabbing);
        //initialize variables
        Transform knifeHandPoint = AnimationRiggingManager.Instance.knifeHoldPoint;
        knife = PhotonNetwork.Instantiate("ItemPrefabs/Melees/Knife", knifeHandPoint.transform.position, knifeHandPoint.transform.rotation, 0, new object[] { PV.ViewID });
        knife.transform.parent = knifeHandPoint;
        StartCoroutine(SetLocalSkillDurations());
        TimeStage.Instance.OnMorning += TimeStage_OnMorning;
        TimeStage.Instance.OnNight += TimeStage_OnNight;
        GameInput.Instance.OnAbility1Performed += GameInput_OnAbility1Performed;
        GameInput.Instance.OnAbility2Performed += GameInput_OnAbility2Performed;
    }
    private void GameInput_OnAbility1Performed(object sender, EventArgs e)
    {
        if (IsQ_Available())
        {
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "invisible", true } });
            Debug.Log("görünmez oldun");
            StartCoroutine(CheckInvisibleTime());
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
    private void TimeStage_OnMorning(object sender, EventArgs e)
    {
        RemoveKnifeFromInventory();
    }
    private void TimeStage_OnNight(object sender, EventArgs e)
    {
        AddKnifeToInventory();
    }

    private void AddKnifeToInventory()
    {
        ItemManager.Instance.AddItem(knife.GetComponent<Knife>());
    }
    private void RemoveKnifeFromInventory()
    {
        ItemManager.Instance.RemoveItem(knife.GetComponent<Knife>());
    }
    private void GameInput_OnAttack(object sender, EventArgs e)
    {
        Stabbing();
    }
    private void Stabbing()
    {
        animator.SetBool("isMelee", true);
        if (ItemManager.Instance.Item() == knife.GetComponent<Knife>()
             && !EventSystem.current.IsPointerOverGameObject()
             && !Job.IsJobCanvasActive
             && !animator.GetBool("isAttacking"))
        {
            animator.SetBool("isAttacking", true);
            knife.GetComponent<Knife>().Use();
            StartCoroutine(OnCompleteAttackAnimation());
        }
    }
    IEnumerator OnCompleteAttackAnimation()
    {
        yield return new WaitForSeconds(stabbingAnim.length);
        animator.SetBool("isAttacking", false);
        knife.GetComponent<Knife>().SetStabbingChecker(false);
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
        TimeStage.Instance.OnMorning -= TimeStage_OnMorning;
        TimeStage.Instance.OnNight -= TimeStage_OnNight;
        choosePlayer.OnPlayerChoosed -= ChoosePlayerManager_OnPlayerChoosed;
        choosePlayer.OnCanceled -= ChoosePlayerManager_OnCanceled;
        Destroy(choosePlayer != null ? choosePlayer.transform.parent.gameObject : null);
        GameInput.Instance.OnAbility1Performed -= GameInput_OnAbility1Performed;
        GameInput.Instance.OnAbility2Performed -= GameInput_OnAbility2Performed;
    }
    private void ApplyCastOnChoosedPlayer(Player choosedPlayer)
    {
        choosedPlayer.SetCustomProperties(new Hashtable { { "replacedNickname", PhotonNetwork.LocalPlayer.NickName } });
        Money -= R_Cost;
        PlayersCanvas.Instance.CanvasStatus(false);
        StartCoroutine(CheckReplacedNicknameTimer(choosedPlayer));
    }

    private IEnumerator CheckReplacedNicknameTimer(Player choosedPlayer)
    {
        yield return new WaitForSeconds(replaceNicknameTimerMax);
        choosedPlayer.SetCustomProperties(new Hashtable { { "replacedNickname", "" } });
    }

    private IEnumerator CheckInvisibleTime()
    {
        yield return new WaitForSeconds(invisibleTimerMax);
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "invisible", false } });
        Debug.Log("görünür oldun");
    }

    public override IEnumerator SetLocalSkillDurations()
    {
        yield return TimeSystem.nightTime != 0;
        invisibleTimerMax = TimeSystem.nightTime / 4 * 60;
        replaceNicknameTimerMax = TimeSystem.nightTime / 3 * 60;
    }

    public override void SetSounds()
    {
    }
}
