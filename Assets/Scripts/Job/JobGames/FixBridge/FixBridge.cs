using UnityEngine;
using UnityEngine.EventSystems;

public class FixBridge : Job
{
    public static int pieceCount = 0;
    [SerializeField] GameObject bridgeCanvas;
    public override void Initialize()
    {
        Cursor.lockState = CursorLockMode.None;
        InitializeJob(JobType._2D);
    }
    void FixedUpdate()
    {
        if (pieceCount == 7)
            JobDone();
        if (leaveJob)
            ResetProp();
        else if (!EventSystem.current.currentSelectedGameObject)
            EventSystem.current.SetSelectedGameObject(bridgeCanvas);

    }


    public override void ResetProp()
    {
        EventSystem.current.SetSelectedGameObject(null);
        QuitJob();
    }
}
