using Photon.Pun;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour
{
    public static AnimationEventHandler Instance { get; private set; }
    private PlayerController playerController;
    private Animator animator;

    private void Awake()
    {
        if (GetComponent<PhotonView>().IsMine && Instance == null)
            Instance = this;
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        playerController = PlayerController.Instance;
    }
    public void SetAnimatorBool(AnimationEvent animationEvent)
    {
        bool active = false;
        if (animationEvent.intParameter == 1)
            active = true;
        animator.SetBool(animationEvent.stringParameter, active);
    }
    public void SetLayerWeight(AnimationEvent animationEvent)
    {
        int layer = animationEvent.intParameter;
        float weight = animationEvent.floatParameter;
        animator.SetLayerWeight(layer, weight);
    }
    public void OnJump()
    {
        //jump buffi alirsa çalýþsýn
        //playerController.rb.AddForce(transform.up * playerController.jumpForce);
    }
}
