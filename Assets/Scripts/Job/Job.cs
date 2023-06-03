using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun;
using UnityEngine.InputSystem;
using System;

public enum JobType
{
    _2D,
    _3D
}
public abstract class Job : MonoBehaviour
{
    public static event EventHandler<OnJobFinishedEventArgs> OnJobFinished;
    public class OnJobFinishedEventArgs : EventArgs
    {
        public string jobName;
    }
    public static bool IsJobCanvasActive = false;

    public static void ResetStaticData()
    {
        OnJobFinished = null;
        IsJobCanvasActive = false;
    }
    GameObject jobTrigger;
    internal int reward;
    internal bool leaveJob;
    public abstract void Initialize();
    public abstract void ResetProp();
    public void InitializeJob(JobType jobType)
    {
        if(jobType == JobType._2D)
        {
            IsJobCanvasActive = true;
        }
        leaveJob = false;
        jobTrigger = transform.parent.gameObject;
        reward = jobTrigger.GetComponent<JobInfo>().jobReward;
        jobTrigger.GetComponent<MeshRenderer>().enabled = false;
        jobTrigger.GetComponent<CapsuleCollider>().enabled = false;
    }
    internal void JobDone()
    {
        GetPaid();
        Cursor.lockState = CursorLockMode.Locked;
        IsJobCanvasActive = false;
        OnJobFinished?.Invoke(this, new OnJobFinishedEventArgs
        {
            jobName = transform.root.name
        });
        Destroy(transform.root.gameObject);
    }
    internal void QuitJob()
    {
        jobTrigger.GetComponent<MeshRenderer>().enabled = true;
        jobTrigger.GetComponent<CapsuleCollider>().enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        IsJobCanvasActive = false;
        gameObject.SetActive(false);
    }
    private void GetPaid()
    {
        int currentMoney = (int)PhotonNetwork.LocalPlayer.CustomProperties["money"];
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "money", currentMoney + reward } });
    }
    private void Update()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            leaveJob = true;
        }
    }
}
