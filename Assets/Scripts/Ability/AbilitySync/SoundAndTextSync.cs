using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System;

public class SoundAndTextSync : MonoBehaviourPunCallbacks
{
    public static SoundAndTextSync Instance { get; private set; }
    /* Sharp Shield */
    [SerializeField] AudioClip shieldCameSound, pistolShotSound, silencedPistolShotSound, noAmmoSound, cursedByImamSound;
    AudioSource audioSource;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    private void Start()
    {
        audioSource = PlayerController.Instance.GetAudioSource();
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        //Sync on all players

        //Fire sync (Murderer gun)
        FireSync((string)changedProps["fireType"], targetPlayer);


        //Sync on selected player

        //Sharp shield sync (Watchman R)
        SharpShieldSync(changedProps, targetPlayer);

        //cursed sync (Imam R)
        cursedSync(changedProps, targetPlayer);

    }

    private void cursedSync(Hashtable changedProps, Player targetPlayer)
    {
        if (changedProps.ContainsKey("cursed") && ChooseRole.pickedRole != "Imam" && targetPlayer == PhotonNetwork.LocalPlayer)
        {
            if ((bool)changedProps["cursed"])
            {
                if ((string)targetPlayer.CustomProperties["side"] == "mafia")
                    SetText("You cursed by the " + Roles.Imam.ToString() + ". If you don't get any spell protection, you will die within " + Math.Round((float)changedProps["cursedDuration"]) + " seconds.", Color.red);

                else
                    SetText("You cursed by the " + Roles.Imam.ToString() + ". If you don't get any spell protection, your next " + Math.Round((float)changedProps["cursedDuration"]) + " seconds is will be painful.", Color.magenta);
                audioSource.PlayOneShot(cursedByImamSound);
            }
        }
    }

    private void SharpShieldSync(Hashtable changedProps, Player targetPlayer)
    {
        if (changedProps.ContainsKey("sharpShield") && ChooseRole.pickedRole != "Watchman" && targetPlayer == PhotonNetwork.LocalPlayer)
            if ((bool)changedProps["sharpShield"])
            {
                SetText(Roles.Watchman.ToString() + " sent you the \"Sharp Shield\" buff. You are protected for " + Math.Round((float)changedProps["sharpShieldDuration"]) + " seconds.", Color.blue);
                audioSource.PlayOneShot(shieldCameSound);
            }
    }

    private void FireSync(string fireType, Player targetPlayer)
    {
        switch (fireType)
        {
            case "normal":
                FindAndPlayPlayerAudioSource(targetPlayer, pistolShotSound, 250f);
                break;
            case "silenced":
                FindAndPlayPlayerAudioSource(targetPlayer, silencedPistolShotSound, 30f);
                break;
            case "noammo":
                FindAndPlayPlayerAudioSource(targetPlayer, noAmmoSound, 5f);
                break;
        }
    }
    private void FindAndPlayPlayerAudioSource(Player targetPlayer, AudioClip sound, float maxDistance)
    {
        foreach (PlayerController controller in FindObjectsOfType<PlayerController>())
        {
            if (controller.GetComponent<PhotonView>().Owner == targetPlayer)
            {
                AudioSource asource = controller.GetAudioSource();
                asource.maxDistance = maxDistance;
                asource.PlayOneShot(sound);
            }
        }
    }
    private void SetText(string text, Color c)
    {
        AbilityUsageText.Instance.SetAbilityUsageText(text, c);
        ChatMessaging.Instance.AddLogText(text, c);
    }
}
