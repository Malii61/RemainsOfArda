using Photon.Pun;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class AnimationRiggingManager : MonoBehaviour, IPunObservable
{
    public static AnimationRiggingManager Instance { get; private set; }

    [SerializeField] private Rig upperAim, middleAim, gunAim, blowgunAim;
    [SerializeField] internal Transform leftHand, rightHand, pistolHandPoint, blowgunHandPoint, knifeHoldPoint;
    private float targetMiddleAimWeight, targetGunAimWeight, targetblowgunAimWeight;
    [SerializeField] PhotonView PV;
    public enum RigEnum
    {
        upper,
        middle,
        gunAim,
        blowgunAim
    }
    private void Awake()
    {
        if (PV.IsMine)
        {
            Instance = this;
        }
    }
    public void SetRigWeight(RigEnum rig, float weight)
    {
        switch (rig)
        {
            case RigEnum.upper:
                upperAim.weight = weight;
                break;
            case RigEnum.middle:
                targetMiddleAimWeight = weight;
                break;
            case RigEnum.gunAim:
                targetGunAimWeight = weight;
                break;
            case RigEnum.blowgunAim:
                targetblowgunAimWeight = weight;
                break;
        }
    }
    private void Update()
    {
        CheckWeightChanges(new Rig[] { middleAim, gunAim, blowgunAim }, new float[] { targetMiddleAimWeight, targetGunAimWeight, targetblowgunAimWeight });
    }
    private void LateUpdate()
    {
        if (PV.IsMine)
            CheckBodyAimRig();
    }
    private void CheckWeightChanges(Rig[] rigs, float[] targets)
    {
        for (int i = 0; i < rigs.Length; i++)
        {
            if (targets[i] == rigs[i].weight)
                continue;
            rigs[i].weight = Mathf.Lerp(rigs[i].weight, targets[i], Time.deltaTime * 20);
        }
    }
    private void CheckBodyAimRig()
    {
        if (GameInput.Instance.GetMovementVectorNormalized() == Vector2.zero)
            SetRigWeight(RigEnum.middle, 0);
        else
            SetRigWeight(RigEnum.middle, 1);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(targetMiddleAimWeight);
        }
        else if (stream.IsReading)
        {
            targetMiddleAimWeight = (float)stream.ReceiveNext();
        }
    }
}
