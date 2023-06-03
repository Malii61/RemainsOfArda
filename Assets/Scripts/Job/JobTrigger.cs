using TMPro;
using UnityEngine;

public class JobTrigger : MonoBehaviour, I_Interactable
{
    //txt
    internal TextMeshProUGUI interactionText;
    JobInfo info;
    GameObject job;
    void Start()
    {
        info = GetComponent<JobInfo>();
    }

    public void OnFaced()
    {
        interactionText.enabled = true;
        TranslateOnRuntime.Translate(interactionText, "Job: " + info.jobName + "\nReward: " + info.jobReward + "\n[" + GameInput.Instance.GetInputActions().Interactions.Interact.bindings[0].ToDisplayString() + "] to Start", textSize.camelCase);
    }

    public void Interact()
    {
        //when we entering the job first time
        if (!job)
        {
            job = (GameObject)Instantiate(Resources.Load("Jobs/" + info.jobName), transform.position, transform.rotation);
            job.transform.parent = transform;
        }
        //when we quit this job earlier and try to enter in again now
        else
        {
            job.SetActive(true);
        }
        job.GetComponent<Job>().Initialize();
    }

    public void OnInteractEnded()
    {
        interactionText.enabled = false;
    }
}
