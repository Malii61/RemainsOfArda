using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class AbilityDetailsUI : MonoBehaviour
{
    [SerializeField] GameObject detailsPanel;
    [SerializeField] TextMeshProUGUI passiveField;
    [SerializeField] TextMeshProUGUI Q_Field;
    [SerializeField] TextMeshProUGUI R_Field;
    void Start()
    {
        AllAbilityDetails details = new AllAbilityDetails();

        var passive = details.abilityDetails[ChooseRole.pickedRole].Item1;
        var q_detail = details.abilityDetails[ChooseRole.pickedRole].Item2;
        var r_detail = details.abilityDetails[ChooseRole.pickedRole].Item3;
        passiveField.text = passive;
        Q_Field.text = q_detail;
        R_Field.text = r_detail;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameInput.Instance.GetInputActions().Interactions.AbilityDetails.IsPressed())
            detailsPanel.SetActive(true);
        else
            detailsPanel.SetActive(false);
    }
}
