using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilitySlots : MonoBehaviour
{
    public static AbilitySlots Instance { get; private set; }
    public enum Image
    {
        Q_Ability,
        R_Ability,
        Q_AbilityCD,
        R_AbilityCD
    }
    [SerializeField] UnityEngine.UI.Image Q_AbilityImage, R_AbilityImage, Q_AbilityCD_Image, R_AbilityCD_Image;
    [SerializeField] TextMeshProUGUI Q_AbilityTextDisplay, R_AbilityTextDisplay, R_CostText;
    private void Awake()
    {
            if (Instance == null)
                Instance = this;
    }
    private void Start()
    {
        Q_AbilityTextDisplay.text = GameInput.Instance.GetInputActions().Interactions.Ability1.bindings[0].ToDisplayString();
        R_AbilityTextDisplay.text = GameInput.Instance.GetInputActions().Interactions.Ability2.bindings[0].ToDisplayString();
        SetImageFillAmount(Image.Q_AbilityCD, 1);
        SetImageFillAmount(Image.R_AbilityCD, 1);
        PlayerController.Instance.OnPlayerSpawned += PlayerController_OnPlayerSpawned;
        PlayerController.Instance.OnPlayerDie += PlayerController_OnPlayerDie;
    }

    private void PlayerController_OnPlayerSpawned(object sender, System.EventArgs e)
    {
        gameObject.SetActive(true);
    }

    private void PlayerController_OnPlayerDie(object sender, System.EventArgs e)
    {
        gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        PlayerController.Instance.OnPlayerSpawned -= PlayerController_OnPlayerSpawned;
        PlayerController.Instance.OnPlayerDie -= PlayerController_OnPlayerDie;
    }
    public void SetAbilities(string role)
    {
        Q_AbilityImage.sprite = Resources.Load<Sprite>("AbilityImages/" + role + "/" + role + "Q");
        R_AbilityImage.sprite = Resources.Load<Sprite>("AbilityImages/" + role + "/" + role + "R");
    }
    public void SetImageFillAmount(Image img, float fillAmount)
    {
        UnityEngine.UI.Image image = ReturnImageFromEnum(img);
        image.fillAmount = fillAmount;
    }
    public void SetR_CostDisplayText(int cost)
    {
        R_CostText.text = cost.ToString();
    }
    public UnityEngine.UI.Image GetImage(Image img)
    {
        return ReturnImageFromEnum(img);
    }
    private UnityEngine.UI.Image ReturnImageFromEnum(Image img)
    {
        switch (img)
        {
            default:
            case Image.Q_Ability:
                return Q_AbilityImage;
            case Image.R_Ability:
                return R_AbilityImage;
            case Image.Q_AbilityCD:
                return Q_AbilityCD_Image;
            case Image.R_AbilityCD:
                return R_AbilityCD_Image;
        }
    }
}
