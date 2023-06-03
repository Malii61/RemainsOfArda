using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Voice;
using Photon.Pun;
using Photon.Voice.PUN;

public class MicCheck : MonoBehaviour
{
    [SerializeField]
    PhotonVoiceView PVV;
    PhotonView PV;
    [SerializeField]
    GameObject micSignal;

    private void Awake()
    {
		PV = GetComponent<PhotonView>();
    }
    private void Update()
    {
        if (!PV.IsMine)
            return;
        PV.RPC(nameof(showMicSignal),RpcTarget.Others, PVV.IsRecording);
    }
    [PunRPC]
    void showMicSignal(bool isRecording)
    {
        micSignal.SetActive(isRecording);
    }


}
