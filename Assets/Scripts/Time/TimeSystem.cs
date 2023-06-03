using UnityEngine;
using TMPro;
using Photon.Pun;
using System;

public class TimeSystem : MonoBehaviourPunCallbacks
{
    public static TimeSystem Instance { get; private set; } 

    public static int dayCount = 1;
    public static float dayTime;
    public static float nightTime;

    public int hour = 8;
    public float minute = 0;
    [SerializeField] TextMeshProUGUI timeText;
    //day text
    [SerializeField] TextMeshProUGUI dayText;
    private bool checkDayCount = false;
    //sun
    [SerializeField] Light sun;
    private float xRot;

    private float duration;
    PhotonView PV;

    TimeStage timeStage;
    private bool startTimer;
    private float timeFlowValue;

    private void Awake()
    {
        Instance = this;
        dayText.text = "DAY 1";
        PV = GetComponent<PhotonView>();
        if (PhotonNetwork.IsMasterClient)
        {
            PV.RPC(nameof(RPC_SetDayTime), RpcTarget.All, dayTime, nightTime);
        }
        timeStage = GetComponent<TimeStage>();
    }
    [PunRPC]
    void RPC_SetDayTime(float dayLong, float nightLong)
    {
        dayTime = dayLong;
        nightTime = nightLong;
        timeFlowValue = Time.fixedDeltaTime * 12 / dayTime;
    }

    private void Start()
    {
        sun.transform.rotation = Quaternion.Euler(180, 0, 0);
        xRot = sun.transform.rotation.x;
        timeStage.OnMorning += TimeStage_OnMorning;
        timeStage.OnNight += TimeStage_OnNight;
        GameManager.Instance.OnEveryoneSpawned += GameManager_OnEveryoneSpawned;
    }

    private void TimeStage_OnMorning(object sender, EventArgs e)
    {
        timeFlowValue = Time.fixedDeltaTime * 12 / dayTime;
    }
    private void TimeStage_OnNight(object sender, EventArgs e)
    {
        timeFlowValue = Time.fixedDeltaTime * 12 / nightTime;
    }

    private void GameManager_OnEveryoneSpawned(object sender, EventArgs e)
    {
        startTimer = true;
    }
    private void OnDestroy()
    {
        GameManager.Instance.OnEveryoneSpawned -= GameManager_OnEveryoneSpawned;
        timeStage.OnMorning -= TimeStage_OnMorning;
        timeStage.OnNight -= TimeStage_OnNight;
    }
    private void FixedUpdate()
    {
        if (!startTimer)
            return;
        TimeFlow();
        SunRotation();
        SetTimeText();
    }
    private void TimeFlow()
    {
        xRot += timeFlowValue / 4;
        if (minute >= 60)
        {
            minute -= 60;
            if (hour >= 23)
                hour = 0;
            else
                hour += 1;
        }
        else
            minute += timeFlowValue;
        if (xRot > 360)
            xRot -= 360;
    }
    private void SetTimeText()
    {
        string hourstring = hour < 10 ? "0" + hour : "" + hour;
        string minutestring = minute < 10 ? "0" + Mathf.Floor(minute) : "" + Mathf.Floor(minute);
        if (minute < 0) minutestring = "00";
        timeText.text = hourstring + ":" + minutestring;
        if (timeText.text == "08:00" && checkDayCount)
        {
            dayCount += 1;
            TranslateOnRuntime.Translate(dayText, "DAY " + dayCount, textSize.upperCase);
            checkDayCount = false;
        }
        else if (timeText.text == "20:00")
        {
            TranslateOnRuntime.Translate(dayText, "NIGHT " + dayCount, textSize.upperCase);
            checkDayCount = true;
        }
    }

    private void SunRotation()
    {
        var rotationVector = transform.rotation.eulerAngles;
        rotationVector.x = xRot;
        sun.transform.rotation = Quaternion.Euler(rotationVector);
    }
}
