using UnityEngine;
public class Pray : Job
{
    [SerializeField] PrayPosition prayPos;
    public override void Initialize()
    {
        InitializeJob(JobType._2D);
        prayPos.Init();
    }

    void FixedUpdate()
    {
        CheckWin();
        if (leaveJob)
            ResetProp();
    }
    public override void ResetProp()
    {
        prayPos.ResetPosition();
        QuitJob();
    }
    private void CheckWin()
    {
        if (prayPos.prayCount == prayPos.sumOfPrays)
        {
            JobDone();
        }
    }
   
}
