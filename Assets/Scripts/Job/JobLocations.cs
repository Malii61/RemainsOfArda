using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun;
using System;

[Serializable]
public class JobInfos
{
    public string jobName;
    public int jobReward;
    public string locationName;
    public Vector3[] locationPlaces;
}
public class JobLocations : MonoBehaviour
{
    [Header("Set Job Infos")]
    [SerializeField] internal List<JobInfos> jobInfos = new List<JobInfos>();
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public List<string> GetJobNames()
    {
        List<string> jobNames = new List<string>();
        foreach (var jb in jobInfos)
            jobNames.Add(jb.jobName);
        return jobNames;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
