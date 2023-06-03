using UnityEngine;
using TMPro;
using System;

public class Mining : Job
{
    [SerializeField] MiningAreaTrigger areaTrigger;
    [SerializeField] TextMeshProUGUI miningTimerText;
    [SerializeField] GameObject pickaxe;
    float miningTime = 10;
    float rotateAmount;
    public override void Initialize()
    {
        rotateAmount = Time.fixedDeltaTime * 300;
        miningTimerText.text = Math.Round(miningTime, 1) + " s";
        InitializeJob(JobType._3D);

    }
    void FixedUpdate()
    {
        miningTime -= Time.fixedDeltaTime;
        miningTimerText.text = Math.Round(miningTime, 1).ToString() + " s";
        RotatePickaxe();
        CheckArea();
        if (miningTime <= 0)
            JobDone();
        if (leaveJob)
            ResetProp();
    }

    private void RotatePickaxe()
    {
        if (!areaTrigger.inArea)
            return;
        var rot = pickaxe.transform.rotation.x;
        if (rot <= 0)
            rotateAmount = Time.fixedDeltaTime * 300;
        else if (rot >= 0.6f)
            rotateAmount = -Time.fixedDeltaTime * 100;
        pickaxe.transform.Rotate(rotateAmount, 0f, 0f);
    }

    private void CheckArea()
    {
        if (!areaTrigger.inArea)
            miningTime = 10;
    }

    public override void ResetProp()
    {
        miningTime = 10;
        QuitJob();
    }
}
