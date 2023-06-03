using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
public enum jobState
{
    unassigned,
    assigned,
    unfinished,
    finished
}
public class AssignJob : MonoBehaviour
{
    private int jobCount;
    private Dictionary<string, jobState> jobs = new Dictionary<string, jobState>();
    [SerializeField] TextMeshProUGUI txt;
    [SerializeField] GameObject jobTrigger;
    JobLocations jobLocations;
    internal Dictionary<string, GameObject> jobTriggers = new Dictionary<string, GameObject>();
    void Start()
    {
        jobLocations = GetComponent<JobLocations>();
        jobCount = 4; // oyuncu sayýsýna göre düzenle
        GetJobs();
        Job.OnJobFinished += Job_OnJobFinished;
    }

    private void Job_OnJobFinished(object sender, Job.OnJobFinishedEventArgs e)
    {
        RemoveJobFromJobsList(e.jobName);
    }

    private void GetJobs()
    {
        // get all jobs from folder
        Object[] fileInfo = Resources.LoadAll("Jobs");
        foreach (var file in fileInfo)
        {
            // add jobs according to their name to jobs list as unassigned
            string jobName = file.name.Split('.')[0];
            if (!jobs.ContainsKey(jobName))
                jobs.Add(jobName, jobState.unassigned);
        }
        AssignRandomJob();
    }

    private void RemoveJobFromJobsList(string triggerName)
    {
        // remove finished job from jobs list
        jobTriggers.Remove(triggerName);
    }

    private void AssignRandomJob()
    {
        int assignedJobCount = 0;
        while (assignedJobCount < jobCount)
        {
            int randomJobOrder = Random.Range(0, jobs.Count);
            for(int i = 0; i < jobCount; i++)
            {
                var job = jobs.ElementAt(i);
                if (job.Key == jobLocations.jobInfos[randomJobOrder].jobName)
                {
                    if (job.Value == jobState.assigned)
                        break;
                    var jb = jobLocations.jobInfos[randomJobOrder];
                    Vector3[] locs = jb.locationPlaces;
                    Vector3 loc = locs[Random.Range(0, locs.Length)];
                    GameObject jobTrigger = Instantiate(this.jobTrigger, loc, Quaternion.identity);
                    jobTrigger.name = jb.jobName;
                    jobTrigger.GetComponent<JobInfo>().Initialize(jb.jobName, jb.jobReward);
                    jobTrigger.GetComponent<JobTrigger>().interactionText = txt;
                    jobTriggers.Add(jb.jobName,jobTrigger);
                    jobs[job.Key] = jobState.assigned;
                    assignedJobCount++;
                }
            }
        }
    }
}
