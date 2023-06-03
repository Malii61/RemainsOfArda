using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobInfo : MonoBehaviour
{
    [HideInInspector] public string jobName; 
    [HideInInspector] public int jobReward;
    public void Initialize(string _jobName ,int _jobReward)
    {
        jobName = _jobName;
        jobReward = _jobReward;
    }
}
