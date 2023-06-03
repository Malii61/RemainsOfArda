using UnityEngine;
using Photon.Pun;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System;

public class Role_Murderer : Role
{
    //Adding pistol to scene
    GameObject pistol;
    GameObject pistolObject;
    //Role Attributes
    private int bulletCount;
    private bool isSupressed = false;
    //Sound
    private AudioClip pickupAmmoSound;
    private AudioClip attachSuppresorSound;
    //Text
    private float disableTextTimer;
    //hash
    Hashtable hash = new Hashtable();
    //bullets
    GameObject bulletCanvas;
    private List<Image> bulletImages;
    Transform container;
    Image firstBullet;
    private bool zooming;

    void Start()
    {

        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("bulletCount", out object bullet))
            bulletCount = (int)bullet;
        else
        {
            bulletCount = 1;
            hash.Add("bulletCount", 1);
        }
        bulletImages = new List<Image>();
        SetMoneyAndPrices(_money: 125, R_price: 100);
        SetSounds();
        SetRole(Roles.Murderer);
        hash.Add("fireType", "");
        PhotonNetwork.SetPlayerCustomProperties(hash);

        //set animation override
        AnimatorOverrider.Instance.SetAnimatorOverride(AnimatorOverrider.overrides.pistol);

        //initialize variables
        Transform pistolHandPoint = AnimationRiggingManager.Instance.pistolHandPoint;
        pistol = PhotonNetwork.Instantiate("ItemPrefabs/Guns/Pistol", pistolHandPoint.position, pistolHandPoint.rotation, 0, new object[] { PV.ViewID });
        pistol.transform.parent = pistolHandPoint;
        StartCoroutine(SetLocalSkillDurations());

        GameInput.Instance.OnAttack += GameInput_OnShoot;
        TimeStage.Instance.OnMorning += TimeStage_OnMorning;
        TimeStage.Instance.OnNight += TimeStage_OnNight;
        GameInput.Instance.OnAbility1Performed += GameInput_OnAbility1Performed;
        GameInput.Instance.OnAbility2Performed += GameInput_OnAbility2Performed;
    }

    private void GameInput_OnAbility1Performed(object sender, EventArgs e)
    {
        if (IsQ_Available())
        {
            audioSource.PlayOneShot(attachSuppresorSound);
            SetAbilityUsageText("Your gun is supressed for " + Q_Duration + " seconds", Color.blue);
            SetAbilityIcons("MurdererQ", Q_Duration, Color.black);
            EnableSupress();
            Invoke(nameof(DisableSupress), Q_Duration);
            OnQ_Performed();
        }
    }

    private void GameInput_OnAbility2Performed(object sender, EventArgs e)
    {
        if (IsR_Available())
        {
            audioSource.PlayOneShot(pickupAmmoSound);
            bulletCount++;
            Money -= R_Cost;
            AddBulletToCanvas();
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "bulletCount", bulletCount } });
        }
    }
    private void TimeStage_OnMorning(object sender, EventArgs e)
    {
        RemovePistolFromInventory();
    }
    private void TimeStage_OnNight(object sender, EventArgs e)
    {
        AddPistolToInventory();
    }

    private void GameInput_OnShoot(object sender, EventArgs e)
    {
        Fire();
    }
    private void OnDestroy()
    {
        GameInput.Instance.OnAttack -= GameInput_OnShoot;
        GameInput.Instance.OnAbility2Performed -= GameInput_OnAbility2Performed;
        GameInput.Instance.OnAbility2Performed -= GameInput_OnAbility2Performed;
        TimeStage.Instance.OnMorning -= TimeStage_OnMorning;
        TimeStage.Instance.OnNight -= TimeStage_OnNight;
    }
    private void LateUpdate()
    {
        Zoom();
        CheckBalanceForAbility2();
    }

    private void CheckBalanceForAbility2()
    {
        if (!IsR_Available())
        {
            abilitySlots.SetImageFillAmount(AbilitySlots.Image.R_AbilityCD, 1);
        }
        else
        {
            abilitySlots.SetImageFillAmount(AbilitySlots.Image.R_AbilityCD, 0);
        }
    }

    private void Zoom()
    {
        if (GameInput.Instance.GetInputActions().Player.Zoom.IsPressed()
            && !EventSystem.current.IsPointerOverGameObject()
            && !Job.IsJobCanvasActive
            && ItemManager.Instance.Item() == pistol.GetComponent<Pistol>())
        {
            animator.SetBool("isZooming", true);
            AnimationRiggingManager.Instance.SetRigWeight(AnimationRiggingManager.RigEnum.gunAim, 1);
            zooming = true;
        }
        else if (zooming)
        {
            animator.SetBool("isZooming", false);
            AnimationRiggingManager.Instance.SetRigWeight(AnimationRiggingManager.RigEnum.gunAim, 0);
            zooming = false;
        }
    }
    private void Fire()
    {
        if (ItemManager.Instance.Item() == pistol.GetComponent<Pistol>() && zooming)
        {
            if (EventSystem.current.IsPointerOverGameObject() || Job.IsJobCanvasActive)
            {
                return;
            }
            animator.SetBool("isAttacking", true);
            if (bulletCount != 0)
            {
                pistol.GetComponent<Pistol>().Use();
                if (isSupressed)
                {
                    hash["fireType"] = "silenced";
                }
                else
                {
                    hash["fireType"] = "normal";
                }
                bulletCount--;
                RemoveBulletFromCanvas();
                hash["bulletCount"] = bulletCount;
            }
            else
            {
                hash["fireType"] = "noammo";
            }
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        }
    }
    private void AddPistolToInventory()
    {
        ItemManager.Instance.AddItem(pistol.GetComponent<Pistol>());
        ShowBulletCanvas(true);
    }
    private void RemovePistolFromInventory()
    {
        ItemManager.Instance.RemoveItem(pistol.GetComponent<Pistol>());
        ShowBulletCanvas(false);
    }
    private void EnableSupress()
    {
        isSupressed = true;
        pistol.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
    }
    private void DisableSupress()
    {
        isSupressed = false;
        pistol.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
    }

    public override void SetSounds()
    {
        pickupAmmoSound = (AudioClip)Resources.Load("Sounds/AbilitySounds/Murderer/pickup-ammo");
        attachSuppresorSound = (AudioClip)Resources.Load("Sounds/AbilitySounds/Murderer/attach-suppresor");
    }
    private void ShowBulletCanvas(bool activity)
    {
        if (!bulletCanvas)
        {
            bulletCanvas = (GameObject)Instantiate(Resources.Load("RoleSpecificPrefabs/Murderer/BulletsCanvas"));
            container = bulletCanvas.GetComponent<RectTransform>();
            firstBullet = bulletCanvas.transform.GetChild(1).GetComponent<Image>();
            for (int i = 0; i < bulletCount; i++)
            {
                AddBulletToCanvas();
            }
        }
        bulletCanvas.SetActive(activity);
    }
    private void AddBulletToCanvas()
    {
        Image bullet = Instantiate(firstBullet, container);
        bullet.gameObject.SetActive(true);
        bulletImages.Add(bullet);
    }
    private void RemoveBulletFromCanvas()
    {
        Destroy(bulletImages[bulletImages.Count - 1].gameObject);
        bulletImages.Remove(bulletImages[bulletImages.Count - 1]);
    }
    public override IEnumerator SetLocalSkillDurations()
    {
        yield return TimeSystem.nightTime != 0;
        Q_Duration = TimeSystem.nightTime / 6 * 60;
    }
}
