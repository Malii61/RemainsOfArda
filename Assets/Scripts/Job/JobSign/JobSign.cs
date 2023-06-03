using UnityEngine;
using TMPro;
using System.Linq;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

public class JobSign : MonoBehaviour
{
    [SerializeField] AssignJob assignJob;
    TextMeshProUGUI signText;
    private float resetTextTimer = 0f;
    Transform sign;
    [SerializeField] Transform arrowPanel;
    List<Image> signArrows = new List<Image>();
    void Start()
    {
        for (int i = 0; i < arrowPanel.childCount; i++)
            signArrows.Add(arrowPanel.GetChild(i).GetChild(0).GetComponent<Image>());
        sign = transform.root;
        signText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (resetTextTimer > 2f)
        {
            ResetText();
            ResetPanel();
            AdjustSignText();
            resetTextTimer = 0f;
        }
        else
            resetTextTimer += Time.fixedDeltaTime;
    }

    private void ResetPanel()
    {
        for (int i = 0; i < signArrows.Count; i++)
            signArrows[i].gameObject.SetActive(false);
    }

    private void ResetText()
    {
        signText.text = "";
    }

    private void AdjustSignText()
    {
        for (int i = 0; i < assignJob.jobTriggers.Count; i++)
        {
            var job = assignJob.jobTriggers.ElementAt(i);
            if (!job.Value)
                continue;
            double dist = Math.Round(CalcDistance(job.Value.transform.position), 1);
            signArrows[i].gameObject.SetActive(true);
            Vector3 relativePos = job.Value.transform.position - signArrows[i].transform.position;
            Quaternion rotation = Quaternion.LookRotation(relativePos);
            rotation.z = -rotation.y;
            rotation.x = 0;
            rotation.y = 0;
            Vector3 northDirection = new Vector3(0, 0, 90);
            signArrows[i].transform.localRotation = rotation * Quaternion.Euler(northDirection);
            signText.text += job.Key + "(" + dist + " m)\n";
        }
    }

    private float CalcDistance(Vector3 jobLoc)
    {
        return Vector3.Distance(sign.position, jobLoc);
    }
}
