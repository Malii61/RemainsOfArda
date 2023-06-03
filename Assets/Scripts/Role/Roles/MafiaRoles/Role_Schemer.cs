using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Role_Schemer : Role
{
    //sounds
    Player Q_choosedPl;
    Player R_choosedPl;
    //diary variables
    private string diaryContent;
    private bool changeOnce;
    //select random role prefab
    GameObject randomRolesCanvas;
    CheckRoleSelecting checkSelect;
    //q bools
    private bool choosed;
    private bool changeDiary;
    private bool useAbilityOnce;

    ChoosePlayerManager choosePlayer;
    void Start()
    {
        choosePlayer = CreatePlayersCanvas.Create(AbilityType.magic, "Choose a player to change the shown role", playersStatus.alive);
        SetMoneyAndPrices(250, 50);
        SetSounds();
        SetRole(Roles.Schemer);


        choosePlayer.OnPlayerChoosed += ChoosePlayerManager_OnPlayerChoosed;
        choosePlayer.OnCanceled += ChoosePlayerManager_OnCanceled;
        GameInput.Instance.OnAbility1Performed += GameInput_OnAbility1Performed;
        GameInput.Instance.OnAbility2Performed += GameInput_OnAbility2Performed;
    }

    private void GameInput_OnAbility1Performed(object sender, EventArgs e)
    {
        if (IsQ_Available())
        {
            choosed = true;
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
    void FixedUpdate()
    {
        CheckQCast();
        CheckFakeRole();
    }
    private void LateUpdate()
    {
        ReplaceDiary();
    }
    private void ApplyCastOnChoosedPlayer(Player pl)
    {
        R_choosedPl = pl;
        PlayersCanvas.Instance.CanvasStatus(false);
        randomRolesCanvas = (GameObject)Instantiate(Resources.Load("CommonPrefabs/RandomRolesCanvas"));
        Debug.Log(randomRolesCanvas == null);
        randomRolesCanvas.GetComponent<SetRandomRoles>().Initialize(R_choosedPl);
        checkSelect = randomRolesCanvas.GetComponent<CheckRoleSelecting>();
        Money -= R_Cost;
        OnR_Performed();
    }
    private void CheckFakeRole()
    {
        if (checkSelect && checkSelect.role != null)
        {
            if (checkSelect.role != "unselected")
            {
                Side side = new Side();
                R_choosedPl.SetCustomProperties(new Hashtable { { "fakeRole", checkSelect.role }, { "fakeSide", side.GetSide(checkSelect.role) } });
                SetAbilityUsageText("You'll show " + R_choosedPl.NickName.ToUpperInvariant() + "'s role as " + checkSelect.role.ToUpperInvariant() + " until morning", Color.cyan);
            }
            Cursor.lockState = CursorLockMode.Locked;
            Destroy(randomRolesCanvas?.gameObject);
        }
        if (TimeStage.Instance.currentStage == Stage.gathering && R_choosedPl != null)
        {
            PlayersCanvas.Instance.CanvasStatus(false);
            Destroy(randomRolesCanvas?.gameObject);
            R_choosedPl.SetCustomProperties(new Hashtable { { "fakeRole", "" }, { "fakeSide", "" } });
            R_choosedPl = null;
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (targetPlayer == Q_choosedPl && changedProps.ContainsKey("diary"))
        {
            if (!(bool)targetPlayer.CustomProperties["alive"])
            {
                changeDiary = true;
                diaryContent = (string)changedProps["diary"];
            }
        }
        else if (targetPlayer == Q_choosedPl && changedProps.ContainsKey("alive") && (bool)changedProps["alive"])
            changeDiary = false;

        CheckPropertyChanges(targetPlayer, changedProps);
    }
    private void ReplaceDiary()
    {
        if (TimeStage.Instance.currentStage == Stage.gathering && changeDiary && changeOnce)
        {
            //change choosed player's diary with schemer's
            Q_choosedPl.SetCustomProperties(new Hashtable { { "diary", DiarySystem.Instance.GetDiaryContent() } });
            DiarySystem.Instance.SetDiaryContent(diaryContent);
            diaryContent = "";
            changeOnce = false;

        }
        else if (TimeStage.Instance.currentStage == Stage.ready)
            changeOnce = true;
    }
    private void CheckQCast()
    {
        if (Physics.Raycast(ray, out RaycastHit hit, playerActivateDistance) && hit.transform.tag == "Player" && !useAbilityOnce)
        {
            if (hit.transform.GetComponent<PhotonView>().Owner == PhotonNetwork.LocalPlayer)
                return;
            abilitySlots.SetImageFillAmount(AbilitySlots.Image.Q_AbilityCD, 0);
            if (choosed)
            {
                Q_choosedPl = hit.transform.gameObject.GetComponent<PhotonView>().Owner;
                string plName = Q_choosedPl.NickName;
                SetAbilityUsageText("You choosed " + plName.ToUpperInvariant(), Color.magenta);
                //audioSource.PlayOneShot(player_is_sus);
                OnQ_Performed();
                choosed = false;
                useAbilityOnce = true;
            }
        }
        else
        {
            abilitySlots.SetImageFillAmount(AbilitySlots.Image.Q_AbilityCD, 1);
        }
    }
    public override IEnumerator SetLocalSkillDurations()
    {
        yield return TimeSystem.nightTime != 0;
    }

    public override void SetSounds()
    {
    }
}
