using Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System;
using TMPro;
using Photon.Pun;

public class Role_Greedy : Role
{
    //layer
    LayerMask layer;
    //skill bools
    private bool stealBag = false;
    private bool earnDouble = true;
    //prefabs
    GameObject goldBag;
    private GameObject goldIndicatorCanvas;
    private GameObject goldIndicator;
    //lists
    private Dictionary<Player, GameObject> Indicators = new Dictionary<Player, GameObject>();
    Dictionary<GameObject, int> goldBags = new Dictionary<GameObject, int>();
    //sound
    private AudioClip goldBagSound;
    private AudioClip bonusCashSound;

    void Start()
    {
        SetMoneyAndPrices(_money: 125, R_price: 100);
        SetSounds();
        SetRole(Roles.Greedy);
        //set layer
        layer = LayerMask.GetMask("GoldBag");
        // initialize prefabs
        InitPrefabs();
        StartCoroutine(SetLocalSkillDurations());
        GameInput.Instance.OnAbility1Performed += GameInput_OnAbility1Performed;
        GameInput.Instance.OnAbility2Performed += GameInput_OnAbility2Performed;
    }

    private void GameInput_OnAbility1Performed(object sender, EventArgs e)
    {
        if (IsQ_Available())
        {
            stealBag = true;
            audioSource.PlayOneShot(goldBagSound);
            OnQ_Performed();
        }
    }
    private void GameInput_OnAbility2Performed(object sender, EventArgs e)
    {
        if (IsR_Available())
        {
            SetAbilityUsageText("Your gold earnings are doubled for " + R_Duration + " seconds", Color.green);
            SetAbilityIcons("GreedyR", R_Duration, Color.yellow);
            audioSource.PlayOneShot(bonusCashSound);
            earnDouble = true;
            Money -= R_Cost;
            OnR_Performed();
        }
    }
    private void OnDestroy()
    {
        GameInput.Instance.OnAbility1Performed -= GameInput_OnAbility1Performed;
        GameInput.Instance.OnAbility2Performed -= GameInput_OnAbility2Performed;
    }
    private void InitPrefabs()
    {
        goldBag = (GameObject)Resources.Load("RoleSpecificPrefabs/Greedy/GoldBag/GoldBag");
        goldIndicatorCanvas = (GameObject)Instantiate(Resources.Load("CommonPrefabs/Indicator"));
        goldIndicatorCanvas.GetComponent<Canvas>().worldCamera = cam;
        goldIndicatorCanvas.GetComponent<Canvas>().planeDistance = 1;
        goldIndicator = goldIndicatorCanvas.transform.GetChild(0).gameObject;
    }

    private void FixedUpdate()
    {
        CheckQCast();
        CheckRTime();
        CheckIndicators();
    }
    private void LateUpdate()
    {
        if (isNight)
            RotateIndicator();
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("diePosition"))
            if ((Vector3)changedProps["diePosition"] != new Vector3(0, 0, 0))
            {
                GameObject bag = Instantiate(goldBag, (Vector3)changedProps["diePosition"], Quaternion.identity);
                int money = (int)targetPlayer.CustomProperties["money"];
                CreateIndicator(targetPlayer);
                goldBags.Add(bag, money);
            }

        CheckPropertyChanges(targetPlayer, changedProps);
    }
    private void CheckIndicators()
    {
        for (int i = 0; i < Indicators.Count; i++)
        {
            var indicator = Indicators.ElementAt(i);
            if (TimeStage.Instance.currentStage != Stage.action)
            {
                indicator.Value.SetActive(false);
                goldBags.ElementAt(i).Key.SetActive(false);
            }
            else
            {
                indicator.Value.SetActive(true);
                goldBags.ElementAt(i).Key.SetActive(true);
            }

            if (indicator.Key.CustomProperties.TryGetValue("alive", out object value))
            {
                if ((bool)value)
                {
                    Indicators.Remove(indicator.Key);
                    goldBags.Remove(goldBags.ElementAt(i).Key);
                    Destroy(indicator.Value.gameObject);
                    Destroy(goldBags.ElementAt(i).Key.gameObject);
                    i--;
                }
            }
        }
    }
    private void CreateIndicator(Player pl)
    {
        GameObject Indicator = Instantiate(goldIndicator, goldIndicatorCanvas.transform);
        Indicator.SetActive(true);
        Indicators.Add(pl, Indicator);
    }
    private void RotateIndicator()
    {
        goldIndicatorCanvas.GetComponent<Canvas>().worldCamera = cam;
        for (int i = 0; i < goldBags.Count; i++)
        {
            KeyValuePair<GameObject, int> bag = goldBags.ElementAt(i);
            AdjustDistance(bag.Key, Indicators.ElementAt(i).Value);
            Vector3 relativePos = bag.Key.transform.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(relativePos);
            rotation.z = -rotation.y;
            rotation.x = 0;
            rotation.y = 0;
            Vector3 northDirection = new Vector3(0, 0, transform.eulerAngles.y);
            Indicators.ElementAt(i).Value.transform.localRotation = rotation * Quaternion.Euler(northDirection);
        }
    }

    private void AdjustDistance(GameObject bag, GameObject indicator)
    {
        double dist = Math.Round(Vector3.Distance(bag.transform.position, transform.position), 1);
        TextMeshProUGUI distText = indicator.transform.GetComponentInChildren<TextMeshProUGUI>();
        distText.text = dist + " m";
        distText.transform.LookAt(cam.transform);
        distText.transform.Rotate(Vector3.up * 180);
    }

    private void CheckRTime()
    {
        if (TimeStage.Instance.currentStage == Stage.gathering)
            earnDouble = false;
    }

    private void CheckQCast()
    {
        if (Physics.Raycast(ray, out RaycastHit hit, playerActivateDistance, layer))
        {
            abilitySlots.SetImageFillAmount(AbilitySlots.Image.Q_AbilityCD, 0);
            if (stealBag)
            {
                for (int i = 0; i < goldBags.Count; i++)
                {
                    KeyValuePair<GameObject, int> bag = goldBags.ElementAt(i);
                    if (bag.Key == hit.transform.gameObject)
                    {
                        if (earnDouble)
                        {
                            Money += bag.Value * 2;
                            SetAbilityUsageText("You have stolen " + bag.Value * 2 + " gold", Color.yellow);
                        }
                        else
                        {
                            Money += bag.Value;
                            SetAbilityUsageText("You have stolen " + bag.Value + " gold", Color.yellow);
                        }
                        Destroy(bag.Key.gameObject);
                        goldBags.Remove(bag.Key);

                        Destroy(Indicators.ElementAt(i).Value);
                        Indicators.ElementAt(i).Key.SetCustomProperties(new Hashtable { { "money", 0 } });
                        Indicators.Remove(Indicators.ElementAt(i).Key);
                    }
                }
                stealBag = false;
            }
        }
        else
        {
            abilitySlots.SetImageFillAmount(AbilitySlots.Image.Q_AbilityCD, 1);
        }
    }
    public override void SetSounds()
    {
        goldBagSound = (AudioClip)Resources.Load("Sounds/AbilitySounds/Greedy/gold-bag");
        bonusCashSound = (AudioClip)Resources.Load("Sounds/AbilitySounds/Greedy/bonus-cash");
    }

    public override IEnumerator SetLocalSkillDurations()
    {
        yield return TimeSystem.nightTime != 0;
        R_Duration = TimeSystem.nightTime / 3 * 60;
    }
}
