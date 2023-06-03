using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Collections;

public class BuyNewspaper : MonoBehaviour,I_Interactable
{
    //layer
    LayerMask layer;
    //txt
    [SerializeField] TextMeshProUGUI interactionText;
    //cost
    private int newspaperCost = 0;
    private bool costIsSettable = true;
    private bool bought = false;
    //newspaper
    GameObject newspaper;

    void Start()
    {
        //set layer
        layer = LayerMask.GetMask("Newspaper");
    }

    void FixedUpdate()
    {
        SetNewspaperCost();
    }
    private void LateUpdate()
    {
        DropNewspaperBeforeNight();
    }

    private void DropNewspaperBeforeNight()
    {
        if (TimeStage.Instance.currentStage == Stage.ready && bought)
        {
            ItemManager.Instance.DestroyItemPrefab(ItemEnum.Newspaper);
            bought = false;
        }
    }
    private void SetNewspaperCost()
    {
        if (TimeStage.Instance.currentStage == Stage.discussion && costIsSettable)
        {
            newspaperCost += TimeSystem.dayCount * 20;
            costIsSettable = false;
        }
        else if (TimeStage.Instance.currentStage == Stage.ready)
        {
            costIsSettable = true;
        }
    }

    public void OnFaced()
    {
        interactionText.enabled = true;
        TranslateOnRuntime.Translate(interactionText, "$" + newspaperCost + "\n[" + GameInput.Instance.GetInputActions().Interactions.Interact.bindings[0].ToDisplayString() + "]", textSize.onlyFirstLetterUpperCase);
    }

    public void Interact()
    {
        if (bought)
        {
            TranslateOnRuntime.Translate(interactionText, "Already bought today's newspaper..", textSize.onlyFirstLetterUpperCase);
        }
        else if (TimeStage.Instance.currentStage != Stage.discussion)
        {
            if (TimeStage.Instance.hour < 10)
                TranslateOnRuntime.Translate(interactionText, "The newspaper has not yet been published..", textSize.onlyFirstLetterUpperCase);
            if (TimeStage.Instance.hour > 16)
                TranslateOnRuntime.Translate(interactionText, "It's too late to buy the newspaper..", textSize.onlyFirstLetterUpperCase);
        }
        else if (PrepareDeathDiaries.GetDiaries().Count == 0)
        {
            TranslateOnRuntime.Translate(interactionText, "No newspaper for today, try tomorrow..", textSize.onlyFirstLetterUpperCase);
        }
        else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("money", out object value))
        {
            if ((int)value >= newspaperCost)
            {
                TranslateOnRuntime.Translate(interactionText, "You bought newspaper! Your new balance: " + ((int)value - newspaperCost), textSize.onlyFirstLetterUpperCase);
                newspaper = ItemManager.Instance.InstantiateItemPrefab("ItemPrefabs/Newspaper", ItemEnum.Newspaper);
                newspaper.GetComponent<Newspaper>().Use();
                PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "money", (int)value - newspaperCost } });
                bought = true;
            }
            else
            {
                TranslateOnRuntime.Translate(interactionText, "You don't have enough money!", textSize.onlyFirstLetterUpperCase);
            }
        }
    }
    public void OnInteractEnded()
    {
        interactionText.enabled = false;
    }
}
