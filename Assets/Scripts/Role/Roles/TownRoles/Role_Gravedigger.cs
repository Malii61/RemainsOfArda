using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Role_Gravedigger : Role
{
    //subscribe check
    private float Q_Timer = 0;
    bool subscribed = false;
    //prefabs
    GameObject gravePrfb;
    private GameObject graveIndicatorCanvas;
    private GameObject graveIndicator;
    //lists
    private bool revive;
    private Dictionary<Player, GameObject> Indicators = new Dictionary<Player, GameObject>();
    List<GameObject> graves = new List<GameObject>();
    void Start()
    {
        SetMoneyAndPrices(_money: 200, R_price: 150);
        SetSounds();
        SetRole(Roles.Gravedigger);
        StartCoroutine(SetLocalSkillDurations());
        //set layer
        InitPrefabs();
        GameInput.Instance.OnAbility1Performed += GameInput_OnAbility1Performed;
        GameInput.Instance.OnAbility2Performed += GameInput_OnAbility2Performed;
    }

    private void GameInput_OnAbility1Performed(object sender, EventArgs e)
    {
        if (IsQ_Available())
        {
            ChatChannel.Instance.SubOrUnsubChannel(Channel.dead, SubType.subscribe);
            subscribed = true;
            SetAbilityUsageText("You can chat with dead players for " + Q_Duration + " seconds. Type \"/d \" at the beginning of the message to write to the dead chat", Color.green);
            OnQ_Performed();
        }
    }
    private void GameInput_OnAbility2Performed(object sender, EventArgs e)
    {
        if (IsR_Available())
        {
            revive = true;
        }
    }

    private void OnDestroy()
    {
        GameInput.Instance.OnAbility1Performed -= GameInput_OnAbility1Performed;
        GameInput.Instance.OnAbility2Performed -= GameInput_OnAbility2Performed;
    }

    private void InitPrefabs()
    {
        gravePrfb = (GameObject)Resources.Load("RoleSpecificPrefabs/Gravedigger/Grave");
        graveIndicatorCanvas = (GameObject)Instantiate(Resources.Load("CommonPrefabs/Indicator"));
        graveIndicatorCanvas.GetComponent<Canvas>().worldCamera = cam;
        graveIndicatorCanvas.GetComponent<Canvas>().planeDistance = 1;
        graveIndicatorCanvas.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = abilitySlots.GetImage(AbilitySlots.Image.R_Ability).sprite;
        graveIndicator = graveIndicatorCanvas.transform.GetChild(0).gameObject;
    }

    void FixedUpdate()
    {
        CheckQTime();
        CheckIndicators();
        CheckRCast();
    }
    private void LateUpdate()
    {
        if (isNight)
            RotateIndicator();
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("diePosition"))
            if ((Vector3)changedProps["diePosition"] != new Vector3(0, 0, 0) && !Indicators.ContainsKey(targetPlayer))
            {
                Debug.Log("diePosition: " + (Vector3)changedProps["diePosition"]);
                GameObject grave = Instantiate(gravePrfb, (Vector3)changedProps["diePosition"], Quaternion.identity);
                CreateIndicator(targetPlayer);
                graves.Add(grave);
            }

        CheckPropertyChanges(targetPlayer, changedProps);
    }
    private void CheckRCast()
    {
        ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        if (Physics.Raycast(ray, out RaycastHit hit, playerActivateDistance))
        {
            if (hit.transform.tag != "Grave")
                return;
            abilitySlots.SetImageFillAmount(AbilitySlots.Image.Q_AbilityCD, 0);
            if (revive)
            {
                for (int i = 0; i < graves.Count; i++)
                {
                    var grave = graves[i];
                    if (grave == hit.transform.gameObject)
                    {
                        Destroy(grave.gameObject);
                        graves.Remove(grave);
                        Destroy(Indicators.ElementAt(i).Value);
                        var pl = Indicators.ElementAt(i).Key;
                        pl.SetCustomProperties(new Hashtable { { "alive", true } });
                        SetAbilityUsageText("You revived " + pl.NickName.ToUpperInvariant() + "!", Color.green);
                        Indicators.Remove(Indicators.ElementAt(i).Key);
                    }
                }
                Money -= R_Cost;
                revive = false;
            }
        }
        else
        {
            abilitySlots.SetImageFillAmount(AbilitySlots.Image.Q_AbilityCD, 1);
        }
    }
    private void CreateIndicator(Player pl)
    {
        GameObject Indicator = Instantiate(graveIndicator, graveIndicatorCanvas.transform);
        Indicator.SetActive(true);
        Indicators.Add(pl, Indicator);
    }
    private void RotateIndicator()
    {
        for (int i = 0; i < graves.Count; i++)
        {
            Vector3 relativePos = graves[i].transform.position - transform.position;
            AdjustDistance(graves[i], Indicators.ElementAt(i).Value);
            Quaternion rotation = Quaternion.LookRotation(relativePos);
            rotation.z = -rotation.y;
            rotation.x = 0;
            rotation.y = 0;
            Vector3 northDirection = new Vector3(0, 0, transform.eulerAngles.y);
            Indicators.ElementAt(i).Value.transform.localRotation = rotation * Quaternion.Euler(northDirection);
        }
    }
    private void AdjustDistance(GameObject obj, GameObject indicator)
    {
        double dist = Math.Round(Vector3.Distance(obj.transform.position, transform.position), 1);
        TextMeshProUGUI distText = indicator.transform.GetComponentInChildren<TextMeshProUGUI>();
        distText.text = dist + " m";
        distText.transform.LookAt(cam.transform);
        distText.transform.Rotate(Vector3.up * 180);
    }
    private void CheckIndicators()
    {
        for (int i = 0; i < Indicators.Count; i++)
        {
            var indicator = Indicators.ElementAt(i);
            if (TimeStage.Instance.currentStage != Stage.action)
            {
                indicator.Value.SetActive(false);
                graves[i].SetActive(false);
            }
            else
            {
                indicator.Value.SetActive(true);
                graves[i].SetActive(true);
            }

            if (indicator.Key.CustomProperties.TryGetValue("alive", out object value))
            {
                if ((bool)value)
                {
                    Indicators.Remove(indicator.Key);
                    graves.Remove(graves[i]);
                    Destroy(indicator.Value.gameObject);
                    Destroy(graves[i].gameObject);
                    i--;
                }
            }
        }
    }
    private void CheckQTime()
    {
        if (subscribed)
        {
            Q_Timer += Time.fixedDeltaTime;
            if (Q_Timer >= Q_Duration)
            {
                ChatChannel.Instance.SubOrUnsubChannel(Channel.dead, SubType.unsubscribe);
                Q_Timer = 0;
                subscribed = false;
            }
        }
    } // Update is called once per frame
    public override IEnumerator SetLocalSkillDurations()
    {
        yield return TimeSystem.nightTime != 0;
        Q_Duration = TimeSystem.nightTime / 6 * 60;
    }

    public override void SetSounds()
    {

    }


}
