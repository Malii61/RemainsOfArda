using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public enum energy
{
    increase,
    decrease,
    hideAndIncrease
}
public class EnergyBar : MonoBehaviourPunCallbacks
{
    public static EnergyBar Instance { get; private set; }
    [SerializeField] private Slider slider;
    [SerializeField] private Image fill;
    public Gradient gradient;
    private float currentEnergy = 100;
    private float energyLoss;
    internal bool playerTired = false;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        SetMaxEnergy(currentEnergy);
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "energyLoss", 12f } });
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
    private void SetMaxEnergy(float energy)
    {
        slider.maxValue = energy;
        slider.value = energy;
        fill.color = gradient.Evaluate(1f);
    }

    public void ChangeEnergy(energy energy)
    {
        slider.gameObject.SetActive(true);
        switch (energy)
        {
            //the energy is increasing while player is not running
            case energy.increase:
                slider.value = currentEnergy + 22 * Time.fixedDeltaTime;
                break;
            //the energy is decreasing while player is running
            case energy.decrease:
                slider.value = currentEnergy - energyLoss * Time.fixedDeltaTime;
                break;
            //this case executes when the player is in the mission
            case energy.hideAndIncrease:
                slider.value = currentEnergy + 15 * Time.fixedDeltaTime;
                slider.gameObject.SetActive(false);
                break;
        }
        currentEnergy = slider.value;
        //the bar turns red when the player is tired
        if (playerTired)
            fill.color = gradient.Evaluate(0f);
        else
            fill.color = gradient.Evaluate(slider.normalizedValue);

        if (slider.value >= slider.maxValue)
        {
            slider.gameObject.SetActive(false);
            playerTired = false;
        }
        else if (energy != energy.hideAndIncrease)
        {
            slider.gameObject.SetActive(true);
            if (slider.value <= 5)
                playerTired = true;
        }
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (targetPlayer == PhotonNetwork.LocalPlayer && changedProps.ContainsKey("energyLoss"))
        {
            energyLoss = (float)changedProps["energyLoss"];
        }
    }
}
