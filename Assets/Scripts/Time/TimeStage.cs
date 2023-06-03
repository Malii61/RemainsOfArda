using System;
using UnityEngine;
public enum Stage
{
    gathering,
    discussion,
    voting,
    ready,
    action
}
public class TimeStage : MonoBehaviour
{
    public static TimeStage Instance { get; private set; }

    public event EventHandler OnMorning;
    public event EventHandler OnAfternoon;
    public event EventHandler OnVoting;
    public event EventHandler OnBeforeNight;
    public event EventHandler OnNight;
    TimeSystem timeSystem;
    internal Stage currentStage;
    internal int hour;
    private void Awake()
    {
        Instance = this;
        timeSystem = GetComponent<TimeSystem>();
    }
    public void SetStage(Stage stage)
    {
        if (currentStage != stage)
        {
            currentStage = stage;
            OnStageChanged();
        }
    }

    private void OnStageChanged()
    {
        switch (currentStage)
        {
            case Stage.gathering:
                OnMorning?.Invoke(this, EventArgs.Empty);
                break;
            case Stage.discussion:
                OnAfternoon?.Invoke(this, EventArgs.Empty);
                break;
            case Stage.voting:
                OnVoting?.Invoke(this, EventArgs.Empty);
                break;
            case Stage.ready:
                OnBeforeNight?.Invoke(this, EventArgs.Empty);
                break;
            case Stage.action:
                OnNight?.Invoke(this, EventArgs.Empty);
                break;
        }
    }
    private void SetStage()
    {
        switch (timeSystem.hour)
        {
            case int i when i >= 8 && i < 10:
                SetStage(Stage.gathering);
                break;
            case int i when i >= 10 && i < 16:
                SetStage(Stage.discussion);
                break;
            case int i when i >= 16 && i < 19:
                SetStage(Stage.voting);
                break;
            case int i when i >= 19 && i < 20:
                SetStage(Stage.ready);
                break;
            case int i when i >= 20 && i < 24 || i >= 0 && i < 8:
                SetStage(Stage.action);
                break;
            default:
                break;
        }
    }
    private void FixedUpdate()
    {
        SetStage();
        hour = timeSystem.hour;
    }
}
