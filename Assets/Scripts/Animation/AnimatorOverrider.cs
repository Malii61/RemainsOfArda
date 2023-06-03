using Photon.Pun;
using UnityEngine;

public class AnimatorOverrider : MonoBehaviour
{
    public static AnimatorOverrider Instance { get; private set; }
    PhotonView PV;
    AnimatorOverrideController animOverride = null;
    public enum overrides
    {
        blowgun,
        pistol,
        knife
    }
    public enum animName
    {
        PistolShooting,
        Stabbing
    }
    [SerializeField] private AnimatorOverrideController[] animatorOverrides;
    private Animator animator;
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        if (PV.IsMine && Instance == null)
            Instance = this;

        animator = GetComponent<Animator>();
    }
    public void SetAnimatorOverride(overrides animationOverride, bool synchronizable = true)
    {
        switch (animationOverride)
        {
            case overrides.blowgun:
                animOverride = animatorOverrides[0];
                break;
            case overrides.pistol:
                animOverride = animatorOverrides[1];
                break;
            case overrides.knife:
                animOverride = animatorOverrides[2];
                break;
        }
        if (animOverride != null)
        {
            if (synchronizable)
                PV.RPC(nameof(SetAnimationsOnEveryPC), RpcTarget.All, animOverride.name);
            else
                animator.runtimeAnimatorController = animOverride;
        }
    }
    public AnimatorOverrideController GetCurrentAnimatorOverride()
    {
        return animOverride;

    }
    public AnimationClip GetAnimation(AnimatorOverrideController animOverride, animName anim)
    {
        foreach (var clip in animOverride.animationClips)
        {
            if (anim.ToString() == clip.name)
                return clip;
        }
        return null;
    }
    [PunRPC]
    private void SetAnimationsOnEveryPC(string animOverrideName)
    {
        foreach (AnimatorOverrideController animatorOverride in animatorOverrides)
        {
            if (animatorOverride.name == animOverrideName)
            {
                animator.runtimeAnimatorController = animatorOverride;
                break;
            }
        }
    }
}
