using UnityEngine;

public class Knife : Item
{
    internal bool checkTarget;
    [SerializeField] StabbingEnemyChecker stabbingChecker;

    public override void Use()
    {
        SetStabbingChecker(true);
    }
    public void SetStabbingChecker(bool isCheckable)
    {
        stabbingChecker.checkable = isCheckable;
    }

}
